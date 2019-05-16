using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
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
        public Config Configuration;

        public int ShardCount;

        public List<YukiShard> Shards;

        public bool IsShuttingDown = false;

        private LoggingService logger;
        private LocalizationService localizationService;

        public YukiBot()
        {
            Shards = new List<YukiShard>();

            logger = new LoggingService();
            localizationService = new LocalizationService();

            logger.Write(LogLevel.Info, "Loading languages....");
            localizationService.LoadLanguages();
        }

        public async Task LoginAsync()
        {
            Configuration = Config.GetConfig();

            if (Configuration != null)
            {
                try
                {
                    DiscordClient = new DiscordShardedClient();

                    await DiscordClient.LoginAsync(TokenType.Bot, Configuration.token);

                    ShardCount = await DiscordClient.GetRecommendedShardCountAsync();

                    logger.Write(LogLevel.Info, "Recommended shards: " + ShardCount);

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
                    logger.Write(LogLevel.Warning, http);
                    logger.Write(LogLevel.Info, "Press any key to retry.");

                    Console.Read();

                    await LoginAsync();
                }
                catch(HttpRequestException http)
                {
                    logger.Write(LogLevel.Warning, http);
                    logger.Write(LogLevel.Info, "Press any key to retry.");

                    Console.Read();

                    await LoginAsync();
                }
            }

            await Task.Delay(-1);
        }

        private Task Log(LogMessage logMessage)
        {
            Services.GetRequiredService<LoggingService>().Write(LogLevel.DiscordNet, logMessage.Message);

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
                .AddSingleton<InteractiveService>()
                .AddSingleton<ConfigDB>()
                .AddSingleton<MessageDB>()
                .AddSingleton(Configuration)
                .AddSingleton(DiscordClient)
                .AddSingleton(logger)
                .AddSingleton(localizationService)
                .AddSingleton(this)
                .BuildServiceProvider();

            CommandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
            });

            CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

        public void Shutdown()
        {
            IsShuttingDown = true;
            Services.GetRequiredService<LoggingService>().Write(LogLevel.Status, "Stopping client...");

            DiscordClient.LogoutAsync();
            DiscordClient.StopAsync();

            DiscordClient.Dispose();

            /* Wait a little to make sure everything has had enough time to write */
            Thread.Sleep(500);
        }
    }
}
