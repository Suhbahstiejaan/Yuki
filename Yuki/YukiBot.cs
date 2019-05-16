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
using Yuki.Core;
using Yuki.Data;
using Yuki.Events;
using Yuki.Services;

namespace Yuki
{
    public class YukiBot
    {
        public static IServiceProvider Services { get; private set; }

        public DiscordShardedClient DiscordClient;
        public CommandService CommandService;
        
        public int ShardCount;

        public List<YukiShard> Shards;

        public bool IsShuttingDown = false;

        public YukiBot()
        {
            Shards = new List<YukiShard>();

            LoggingService.Write(LogLevel.Info, "Loading languages....");
            LocalizationService.LoadLanguages();
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

                    DiscordClient.Log += Log;

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

        private Task Log(LogMessage logMessage)
        {
            LoggingService.Write(LogLevel.DiscordNet, logMessage.Message);

            return Task.CompletedTask;
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
            });

            CommandService.AddModules(Assembly.GetEntryAssembly());
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
