using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Discord.Attributes;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Services;
using Yuki.Bot.Misc.Database;
using Discord.WebSocket;
using Yuki.Bot.Misc.Extensions;
using System.Text.RegularExpressions;

namespace Yuki.Bot.Modules
{
    public class Owner : ModuleBase
    {
        [OwnerOnly]
        [Group("set")]
        public class Set : ModuleBase
        {
            YukiRandom yukiRandom = new YukiRandom();
            [Command("game")]
            public async Task SetGameAsync([Remainder] string game = null)
            {
                DiscordSocketClient client = YukiClient.Instance.GetShard(Context.Guild);
                if (!string.IsNullOrEmpty(game))
                    await client.SetGameAsync(game);
                else
                    await client.SetGameAsync(yukiRandom.RandomGame(client));
            }

            [Command("pfp")]
            public async Task SetPFPAsync([Remainder] string avatar = null)
            {
                string pfp = avatar ?? "default.png";
                if (!string.IsNullOrEmpty(avatar))
                    await YukiClient.Instance.GetShard(Context.Guild).CurrentUser.ModifyAsync(x =>
                    {
                        if (StringHelper.IsImage(avatar))
                        {
                            using (WebClient client = new WebClient())
                            {
                                byte[] imgBytes = client.DownloadData(avatar);
                                using (MemoryStream mem = new MemoryStream(imgBytes))
                                {
                                    Stream strm = mem;
                                    x.Avatar = new Image(strm);
                                }
                            }
                        }

                        x.Avatar = new Image(File.OpenRead(pfp));
                    });
            }
        }
        
        [OwnerOnly]
        [Command("backupdb")]
        public async Task ManualBackupAsync()
        {
            List<Data> data = Localizer.GetStrings(Localizer.YukiStrings.default_lang).owner;

            if (!File.Exists(FileDirectories.DatabaseCopyPath))
            {
                string[] saved = Localizer.GetLocalizedStringFromData(data, "saved").Split(' ');
                
                File.Copy(FileDirectories.Database, FileDirectories.DatabaseCopyPath);
                await ReplyAsync(saved[0] + " " + FileDirectories.Database + " " + saved[1] + " " + FileDirectories.DatabaseCopyPath);
            }
            else
                await ReplyAsync(Localizer.GetLocalizedStringFromData(data, "backup_made"));
        }
        
        [OwnerOnly]
        [Command("savecache")]
        public async Task SaveCacheAsync()
        {
            MessageCache.DumpCacheToFile();
            await ReplyAsync(Localizer.GetLocalizedStringFromData(Localizer.GetStrings(Localizer.YukiStrings.default_lang).owner, "cache_saved"));
        }
        
        [OwnerOnly]
        [Command("cachesize")]
        public async Task CacheSizeAsync()
            => await ReplyAsync(Localizer.GetLocalizedStringFromData(Localizer.GetStrings(Localizer.YukiStrings.default_lang).owner, "total_messages").Replace("%s", MessageCache.Size + ""));

        /* Test */
        [OwnerOnly]
        [Command("notif")]
        public async Task NotifyAsync([Remainder] string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                /* order: name, options, end time */
                string[] _params = Regex.Split(parameters, @"\s*[|]\s*");

                Logger.GetLoggerInstance().SendNotificationFromFirebaseCloud(_params[0], _params[1], "INC_NOTIF_TEST", _params[2]);
            }
        }
    }
}