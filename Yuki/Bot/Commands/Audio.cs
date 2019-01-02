using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Yuki.Bot.Services;
using Yuki.Bot.Misc;
using System;
using Discord.Audio;

public class Audio : ModuleBase
{
    private readonly AudioService _service;

    public Audio(AudioService service)
    {
        _service = service;
    }

    /*[Command("play"), Alias("join")]
    public async Task PlayCmd(string songStr = null)
    {
        try
        {
            if(Context.User is IVoiceState voiceState)
            {
                Song song = new Song()
                {
                    url = songStr,
                    user = Context.User.Username + "#" + Context.User.Discriminator
                };

                AudioChannel channel = new AudioChannel()
                {
                    voiceChannel = (Context.User as IVoiceState).VoiceChannel,
                    textChannel = Context.Channel as ITextChannel,
                    guildId = Context.Guild.Id
                };

                await _service.Queue(channel, song);
            }
            else
                await ReplyAsync("You're not in a voice channel, silly!");
        }
        catch(Exception e) { Console.WriteLine(e); }
    }

    [Command("leave")]
    public async Task LeaveCmd()
    {
        if((Context.User as IVoiceState).VoiceChannel != null)
            await _service.LeaveAsync(Context.Guild.Id);
        else
            await ReplyAsync("You're not in a voice channel, silly!");
    }

    [Command("queue")]
    public async Task QueueCmd()
    {
        //await _service.GetQueue(Context.Guild, (ITextChannel)Context.Channel);
    }*/
}