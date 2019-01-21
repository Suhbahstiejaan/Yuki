using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Yuki.Bot.Common;
using Yuki.Bot.Common.Events;
using Yuki.Bot.Entity;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Services;

namespace Yuki.Bot
{
    public class YukiClient
    {
        /* Static */
        private static YukiClient _instance;

        public static YukiClient Instance {
            get
            {
                if (_instance == null)
                    _instance = new YukiClient();

                return _instance;
            }
        }


        /* Public */
        public DiscordShardedClient Client { get; private set; }
        public YukiShardedEvents Events { get; private set; }
        public Config Config { get; private set; }

        public int MaxShards { get; private set; }

        public IServiceProvider Services { get; private set; }
        public CommandService CommandService { get; private set; }

        public List<YukiShard> ConnectedShards { get; private set; }

        public bool ShardConnected(int shardId)
            => !ConnectedShards.FirstOrDefault(shard => shard.ShardId == shardId).Equals(default(YukiShard));

        public bool IsShuttingDown { get; private set; }

        public int TotalUsers {
            get
            {
                int total = 0;

                for (int i = 0; i < ConnectedShards.Count; i++)
                    total += ConnectedShards[i].Members.Count;

                return total;
            }
        }

        
        /* Private */
        private bool isLoggedIn;




        public YukiClient()
        {
            ConnectedShards = new List<YukiShard>();

            Client = new DiscordShardedClient();
            Events = new YukiShardedEvents();
            Config = Config.Get();

            Console.CancelKeyPress += (s, ev) => Shutdown();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => Shutdown();
        }

        public async Task LoginAsync()
        {
            /* Make sure we aren't logged in before continuing */
            if (isLoggedIn)
            {
                Logger.Instance.Write(LogLevel.Error, "Already logged in!");
                return;
            }


            Logger.Instance.Write(LogLevel.Info, "Logging in...");


            if (Config.Token != null)
            {
                try
                {
                    /* Get the amount of shards needed */
                    
                    await Client.LoginAsync(TokenType.Bot, Config.Token);

                    MaxShards = await Client.GetRecommendedShardCountAsync();

                    Logger.Instance.Write(LogLevel.Info, "Shards set to: " + MaxShards);

                    await Client.LogoutAsync();

                    /* Recreate our Client and login */
                    Client = new DiscordShardedClient(new DiscordSocketConfig()
                    {
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 10000,
                        TotalShards = MaxShards,
                        LogLevel = Discord.LogSeverity.Info
                    });

                    Client.Log += Log;

                    await Client.LoginAsync(TokenType.Bot, Config.Token);
                    await Client.StartAsync();


                    SetupServices();

                    SetupShardEvents();
                }
                catch (HttpException e)
                {
                    Logger.Instance.Write(LogLevel.Error, e);
                    return;
                }
                catch (HttpRequestException e)
                {
                    Logger.Instance.Write(LogLevel.Error, e);
                    return;
                }
                
                isLoggedIn = true;
            }
            else
                Logger.Instance.Write(LogLevel.Error, "Cannot login: Token not set!");
        }


        private Task Log(LogMessage message)
        {
            Logger.Instance.Write(LogLevel.DiscordNet, message.Message);
            return Task.CompletedTask;
        }

        private void SetupShardEvents()
        {
            Client.ShardReady += Events.ShardReady;
            Client.ShardConnected += Events.ShardConnected;
            Client.ShardDisconnected += Events.ShardDisconnected;
        }

        private void SetupServices()
        {
            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Config)
                .AddSingleton(new AudioService(Config))
                /*.AddSingleton<InteractiveService>()*/
                .AddDbContext<YukiContext>()
                .BuildServiceProvider();

            CommandService = new CommandService();
            CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

        private void Shutdown()
        {
            IsShuttingDown = true;

            Client.LogoutAsync();
            Client.StopAsync();

            Client.Dispose();

            Logger.Instance.Write(LogLevel.Info, "Client stopped.");
            Logger.Instance.Write(LogLevel.Info, "Backing up database...");

            if (File.Exists(FileDirectories.DatabaseCopyPath))
                File.Delete(FileDirectories.DatabaseCopyPath);

            File.Copy(FileDirectories.Database, FileDirectories.DatabaseCopyPath);

            Logger.Instance.Write(LogLevel.Info, "Writing message cache to file...");
            MessageCache.DumpCacheToFile();

            Logger.Instance.SendNotificationFromFirebaseCloud("Yuki, offline", "Yuki has been shut down");

            /* Wait a little to make sure everything has had enough time to write */
            Thread.Sleep(1000);
        }
    }
}
