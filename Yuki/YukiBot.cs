using Discord;
using Discord.Net;
using Discord.WebSocket;
using InteractivityAddon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Yuki.Commands.TypeParsers;
using Yuki.Core;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Events;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki
{
    public class YukiBot
    {
        public static IServiceProvider Services { get; private set; }

        public DiscordShardedClient DiscordClient;
        public CommandService CommandService;
        
        public int ShardCount;

        public bool IsShuttingDown = false;

        public YukiBot()
        {
            LoggingService.Write(LogLevel.Info, "Loading languages....");
            LocalizationService.LoadLanguages();

            FileDirectories.CheckCreateDirectories();
        }

        public async Task LoginAsync()
        {
            Config Configuration = Config.GetConfig(true);

            if (Configuration != null)
            {
                try
                {
                    DiscordClient = new DiscordShardedClient();

                    await DiscordClient.LoginAsync(TokenType.Bot, Configuration.token);

                    ShardCount = await DiscordClient.GetRecommendedShardCountAsync();

                    LoggingService.Write(LogLevel.Info, "Recommended shards: " + ShardCount);

                    //cleanup
                    await DiscordClient.LogoutAsync();
                    
                    DiscordClient = new DiscordShardedClient(new DiscordSocketConfig()
                    {
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = ShardCount * 1000,
                        TotalShards = ShardCount
                    });

                    DiscordClient.Log += LoggingService.Write;

                    SetupServices();
                    SetShardEvents();

                    await DiscordClient.LoginAsync(TokenType.Bot, Configuration.token);
                    await DiscordClient.StartAsync();
                }
                catch(HttpException http)
                {
                    LoggingService.Write(LogLevel.Warning, http);
                    LoggingService.Write(LogLevel.Info, "Press any key to retry.");

                    Console.Read();

                    await LoginAsync();
                }
                catch(HttpRequestException http)
                {
                    LoggingService.Write(LogLevel.Warning, http);
                    LoggingService.Write(LogLevel.Info, "Press any key to retry.");

                    Console.Read();

                    await LoginAsync();
                }
            }

            await Task.Delay(-1);
        }

        private void SetShardEvents()
        {
            DiscordClient.ShardReady += DiscordShardEventHandler.ShardReady;
            DiscordClient.ShardConnected += DiscordShardEventHandler.ShardConnected;
            DiscordClient.ShardDisconnected += DiscordShardEventHandler.ShardDisconnected;
        }

        public void SetupServices()
        {
            Services = new ServiceCollection()
                .AddSingleton(DiscordClient)
                .AddSingleton(new InteractivityService(DiscordClient, TimeSpan.FromSeconds(Config.GetConfig().command_timeout_seconds)))
                .AddSingleton(this)
                .BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfiguration()
            {
                CaseSensitive = false,
                DefaultRunMode = RunMode.Parallel
            });

            CommandService.AddModules(Assembly.GetEntryAssembly());
            LoggingService.Write(LogLevel.Debug, $"Found {CommandService.GetAllCommands().Count} command(s)");
            CommandService.AddTypeParser(new UserTypeParser<IUser>());

            /* Reminder */
            System.Timers.Timer t = new System.Timers.Timer();

            t.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;

            t.Elapsed += new System.Timers.ElapsedEventHandler((EventHandler)async delegate (object sender, EventArgs e)
            {
               try
                {
                    List<YukiReminder> reminders = UserSettings.GetReminders(DateTime.UtcNow);


                    foreach (YukiReminder reminder in reminders.ToArray())
                    {
                        await DiscordClient.GetUser(reminder.AuthorId).SendMessageAsync(reminder.Message);

                        UserSettings.RemoveReminder(reminder);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            t.Start();
        }

        public void Shutdown()
        {
            IsShuttingDown = true;
            LoggingService.Write(LogLevel.Status, "Stopping client...");

            DiscordClient.LogoutAsync();
            DiscordClient.StopAsync();

            DiscordClient.Dispose();

            /* Wait a little to make sure everything has had enough time to write */
            Thread.Sleep(500);
        }
    }
}
