using Discord;
using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Entity;
using Yuki.Bot.Common;

namespace Yuki.Bot.Services
{
    public class AudioService
    {
        public static ConcurrentDictionary<AudioChannel, List<Song>> AudioData = new ConcurrentDictionary<AudioChannel, List<Song>>();
        
        public AudioService(Config _Config)
        {

        }

        public async Task Queue(AudioChannel channel, Song song)
        {
            List<Data> localization = Localizer.GetStrings(Localizer.YukiStrings.default_lang).audio;

            AudioChannel existingKey = AudioData.Keys.Where(x => x.guildId == channel.guildId).FirstOrDefault();

            if(existingKey != null)
            {
                bool inserted = false;
                Song[] songs = AudioData[existingKey].ToArray();

                for(int i = songs.Length - 1; i > 0; i--)
                {
                    if(songs[i].url == null)
                    {
                        songs[i] = song;
                        inserted = true;
                        break;
                    }
                }
                if(!inserted)
                    AudioData[existingKey].Add(song);
            }
            else if(AudioData.TryAdd(channel, new List<Song>() { song }))
            {
                channel.audioClient = await channel.voiceChannel.ConnectAsync();
                existingKey = channel;
            }

            EmbedBuilder songEmbed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { Name = Localizer.GetLocalizedStringFromData(localization, "added_queue") })
                .WithDescription(GetInfo(song.url, "title", '&'))
                .AddField(Localizer.GetLocalizedStringFromData(localization, "uploader"), GetInfo(song.url, "author", '&'), true)
                .AddField(Localizer.GetLocalizedStringFromData(localization, "length"), TimeSpan.FromSeconds(int.Parse(GetInfo(song.url, "length_seconds", '&'))).PrettyTime(), true)
                .WithThumbnailUrl(GetInfo(song.url, "thumbnail_url", '&'))
                .WithFooter(new EmbedFooterBuilder() { Text = Localizer.GetLocalizedStringFromData(localization, "requested_by") + " " + song.user + " | " +
                                                              Localizer.GetLocalizedStringFromData(localization, "queue_position") + (AudioData[channel].IndexOf(song) + 1) });

            await channel.textChannel.SendMessageAsync("", false, songEmbed.Build());

            if(!existingKey.IsPlaying && song.url != null)
                await Play(existingKey, song);
        }

        private async Task Play(AudioChannel channel, Song song)
        {
            List<Data> localization = Localizer.GetStrings(Localizer.YukiStrings.default_lang).audio;

            File.Delete(FileDirectories.AppDataDirectory + channel.guildId + ".mp3"); //remove any existing song so we can dl another one
            Process ffmpeg = CreateAudioStream(channel);
            channel.audioStream = ffmpeg.StandardOutput.BaseStream;
            AudioOutStream discord = channel.audioClient.CreatePCMStream(AudioApplication.Music);

            EmbedBuilder songEmbed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { Name = Localizer.GetLocalizedStringFromData(localization, "now_playing") })
                .WithDescription(GetInfo(song.url, "title", '&'))
                .AddField(Localizer.GetLocalizedStringFromData(localization, "uploader"), GetInfo(song.url, "author", '&'), true)
                .AddField(Localizer.GetLocalizedStringFromData(localization, "length"), TimeSpan.FromSeconds(int.Parse(GetInfo(song.url, "length_seconds", '&'))).PrettyTime(), true)
                .WithThumbnailUrl(GetInfo(song.url, "thumbnail_url", '&'))
                .WithFooter(new EmbedFooterBuilder() { Text = Localizer.GetLocalizedStringFromData(localization, "requested_by") + " " + song.user });

            await channel.textChannel.SendMessageAsync("", false, songEmbed.Build());
            AudioData.Keys.Where(x => x == channel).FirstOrDefault().IsPlaying = true;
            await channel.audioStream.CopyToAsync(discord);

            await discord.FlushAsync();
            AudioData[channel].Remove(song);

            if(AudioData[channel].Count > 0)
                await Play(channel, AudioData[channel][0]); //play the first song
            else
                //dispose of the excess data
                await LeaveAsync(channel.guildId);
        }

        public async Task LeaveAsync(ulong guildId)
        {
            List<Data> localization = Localizer.GetStrings(Localizer.YukiStrings.default_lang).audio;

            AudioChannel channel = AudioData.Keys.Where(x => x.guildId == guildId).FirstOrDefault();
            channel.audioStream.Dispose();
            await channel.audioClient.StopAsync();

            channel.audioClient.Disconnected += delegate
            {
                channel.textChannel.SendMessageAsync(Localizer.GetLocalizedStringFromData(localization, "disconnect"));
                AudioData.Keys.Remove(channel);
                return Task.CompletedTask;
            };
        }

        private Process GetYouTubeAudio(AudioChannel channel, Song song)
        {
            ProcessStartInfo youtube_dl = new ProcessStartInfo()
            {
                FileName = "youtube-dl",
                Arguments = "-f 140 --prefer-ffmpeg --audio-format mp3 " + song.url + " -o " + FileDirectories.AppDataDirectory + channel.guildId + ".mp3",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            return Process.Start(youtube_dl);
        }

        private Process CreateAudioStream(AudioChannel channel)
        {
            GetYouTubeAudio(channel, AudioData[channel][0]).WaitForExit();

            ProcessStartInfo ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i " + FileDirectories.AppDataDirectory + channel.guildId + ".mp3" + " -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            return Process.Start(ffmpeg);
        }

        private string GetInfo(string url, string key, char query)
        {
            var api = $"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}";
            return GetArgs(new WebClient().DownloadString(api), key, query);
        }

        private string GetArgs(string args, string key, char query)
        {
            int iqs = args.IndexOf(query);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1
                    ? args.Substring(iqs + 1) : string.Empty)[key];
        }
    }

    public class Song
    {
        public string url;
        public string title;
        public string duration;
        public string thumbnail;
        public string user;
    }

    public class AudioChannel
    {
        public bool IsPlaying;
        public ulong guildId;
        public IAudioClient audioClient;
        public ITextChannel textChannel;
        public IVoiceChannel voiceChannel;
        public Stream audioStream;
    }
}
