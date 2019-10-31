using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki
{
    public class YukiBot
    {
        public const string PatronUrl = "https://www.patreon.com/user?u=7361846";
        public const string PayPalUrl = "https://paypal.me/veenus2247";
        public const string ServerUrl = "https://discord.gg/KwHQzuy";
        public const string BotInvUrl = "https://discordapp.com/oauth2/authorize?client_id=338887651677700098&scope=bot&permissions=271690950";
        public const string GithubUrl = "https://github.com/VeeThree/Yuki/";
        public const string WikiUrl   = "https://github.com/VeeThree/Yuki/wiki/";

        public static DiscordBot Discord { get; private set; }
        
        /* Prevent errors on client disconnect */
        public static bool ShuttingDown;

        public YukiBot()
        {
            Logger.Write(LogLevel.Info, "Loading languages....");
            Localization.LoadLanguages();

            FileDirectories.CheckCreateDirectories();

            Discord = new DiscordBot();
        }

        public async Task RunAsync()
        {
            string token = Config.GetConfig(reload: true).token;

            await Discord.LoginAsync(token);
            Logger.Write(LogLevel.Info, $"Client has been recommended {Discord.ShardCount} shards");

            Discord.Client.Log += Logger.Write;

            await Discord.Client.StartAsync();

            CreateEventManager();

            Logger.Write(LogLevel.Debug, $"Found {Discord.CommandService.GetAllCommands().Count} command(s)");
            Localization.CheckCommands(Discord.CommandService);

            await Task.Delay(-1);
        }

        public void CreateEventManager()
        {
            System.Timers.Timer eventManager = new System.Timers.Timer();

            eventManager.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;

            eventManager.Elapsed += new System.Timers.ElapsedEventHandler((EventHandler)async delegate (object sender, EventArgs e)
            {
                DateTime now = DateTime.UtcNow;

                List<YukiReminder> reminders = UserSettings.GetReminders(now);
                List<GuildConfiguration> configs = GuildSettings.GetGuilds();

                foreach (YukiReminder reminder in reminders.ToArray())
                {
                    await Discord.Client.GetUser(reminder.AuthorId).SendMessageAsync(reminder.Message);

                    UserSettings.RemoveReminder(reminder);
                }

                foreach (GuildConfiguration config in configs)
                {
                    foreach (GuildMutedUser muted in config.MutedUsers.Where(m => m.Time <= now))
                    {
                        await Discord.Client.GetGuild(config.Id).GetUser(muted.Id).RemoveRoleAsync(Discord.Client.GetGuild(config.Id).GetRole(config.MuteRole));

                        GuildSettings.RemoveMute(muted, config.Id);
                    }
                }
            });

            eventManager.Start();
        }

        public void Stop()
        {
            ShuttingDown = true;
            Logger.Write(LogLevel.Status, "Stopping client...");

            Discord.StopAsync().GetAwaiter().GetResult();

            Thread.Sleep(500);
        }
    }
}
