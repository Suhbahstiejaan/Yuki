using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Yuki.Bot.Discord.Events;
using Yuki.Bot.Entities;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Services;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot
{
    public class YukiClient
    {
        /* Static */
        private static YukiClient client;
        
        public static YukiClient Instance
        {
            get
            {
                if (client == null)
                    client = new YukiClient();

                return client;
            }
        }


        /* Private */
        private Dictionary<int, List<ulong>> members = new Dictionary<int, List<ulong>>();
        private System.Timers.Timer daily;
        private Logger _log = Logger.GetLoggerInstance();
        private YukiShardedEvents shardedEvents;
        private MessageEvents _messages = new MessageEvents();
        private bool initialized;


        /* Public */
        public int ShardsReady { get; private set; }
        public int TotalMembers {
            get {
                List<ulong> grabbedIds = new List<ulong>();
                
                foreach (List<ulong> value in members.Values)
                    grabbedIds.AddRange(value.Where(id => !grabbedIds.Contains(id)));

                return grabbedIds.Count;
            }
        }
        
        public DiscordShardedClient DiscordClient { get; private set; }
        public BotCredentials Credentials { get; private set; }
        public CommandService CommandService { get; private set; }
        public IServiceProvider Services { get; private set; }

        public DiscordSocketClient GetShard(int id)
            => Instance.DiscordClient.GetShard(id);

        public DiscordSocketClient GetShard(IGuild guild)
            => guild != null ? Instance.DiscordClient.GetShardFor(guild) : Instance.GetShard(0);

        public int TotalShards
            => Instance.DiscordClient.Shards.Count;

        public int MembersOnShard(int shard)
            => !members.Keys.Contains(shard) ? 0 : members[shard].Count;

        public int ConnectedShards { get; private set; }

        public bool Connected
            => TotalShards == ConnectedShards;

        public bool ShardReady(int shardid)
            => MembersOnShard(shardid) > 0;



        public YukiClient()
        {
            DiscordClient = new DiscordShardedClient();
            shardedEvents = new YukiShardedEvents();
            Credentials = new BotCredentials();
        }

        public async Task Initialize()
        {
            if(!initialized)
            {
                /* Setup daily events timer */
                Instance.daily = new System.Timers.Timer(TimeSpan.FromDays(1).TotalMilliseconds);
                Instance.daily.Elapsed += new ElapsedEventHandler((EventHandler)CheckDaily);

                await SetupServices();
                initialized = true;
            }
        }

        public async Task Login()
        {
            Console.CancelKeyPress += (s, ev) => Shutdown();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => Shutdown();

            if (!initialized)
                Initialize().Wait();

            _log.Write(Misc.LogSeverity.Info, "Logging in...");

            if (Credentials.Token != null)
            {
                try
                {
                    /* Get the recommended amount of shards */
                    await Instance.DiscordClient.LoginAsync(TokenType.Bot, Credentials.Token);
                    
                    int shards = await Instance.DiscordClient.GetRecommendedShardCountAsync();
                    _log.Write(Misc.LogSeverity.Info, "Recommended shards: " + shards);
                    await Instance.DiscordClient.LogoutAsync();
                    
                    /* Recreate our client with the recommended amount of shards and login */
                    Instance.DiscordClient = new DiscordShardedClient(new DiscordSocketConfig()
                                                                            {
                                                                                AlwaysDownloadUsers = true,
                                                                                MessageCacheSize = 10000,
                                                                                TotalShards = shards
                                                                             });
                    
                    await Instance.DiscordClient.LoginAsync(TokenType.Bot, Credentials.Token);
                    await Instance.DiscordClient.StartAsync();


                    /* Setup the events for our shards */
                    Instance.DiscordClient.ShardReady += shardedEvents.ShardReady;
                    Instance.DiscordClient.ShardConnected += shardedEvents.ShardConnected;
                    Instance.DiscordClient.ShardDisconnected += shardedEvents.ShardDisconnected;

                    daily.Start();
                }
                catch (HttpException e)
                {
                    _log.Write(Misc.LogSeverity.Error, e.Message);
                    return;
                }
                catch(HttpRequestException e)
                {
                    _log.Write(Misc.LogSeverity.Error, e.Message);
                    return;
                }
            }
            else
                _log.Write(Misc.LogSeverity.Error, "Token not set!");

            _log.Write(Misc.LogSeverity.Info, "Yuki, online!");
            _log.SendNotificationFromFirebaseCloud("Yuki, online!", "Yuki connected", "INC_CLIENT_CONNECT", "Yuki has successfully connected.");
        }


        private void GetMembers(DiscordSocketClient client)
        {
            /* for loops on arrays are about 5x cheaper than foreach loops on lists. */
            IGuild[] guilds = client.Guilds.ToArray();
            
            for (int j = 0; j < guilds.Length; j++)
            {
                IGuildUser[] users = guilds[j].GetUsersAsync().Result.ToArray();

                for (int k = 0; k < users.Length; k++)
                    Instance.AddTo(client.ShardId, users[k].Id);
            }
        }

        private async Task SetupServices()
        {
            Instance.Services = new ServiceCollection()
                .AddSingleton(DiscordClient)
                .AddSingleton(Credentials)
                .AddSingleton(new AudioService(Credentials))
                /*.AddSingleton<InteractiveService>()*/
                .AddDbContext<YukiContext>()
                .BuildServiceProvider();


            Instance.CommandService = new CommandService();
            await Instance.CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), Instance.Services);


            _log.Write(Misc.LogSeverity.Info, "Verifying commands....");
            Localizer.VerifyCommands();
            _log.Write(Misc.LogSeverity.Info, "Command verification complete!");
        }

        private void CheckDaily(object sender, EventArgs e)
        {
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                    if (uow.PurgeableGuildsRepository != null)
                        PurgeService.CheckForPurge();

                if (File.Exists(FileDirectories.DatabaseCopyPath))
                    File.Delete(FileDirectories.DatabaseCopyPath);

                File.Copy(FileDirectories.Database, FileDirectories.DatabaseCopyPath);

                CachedUser[] users = MessageCache.Users;
                for (int i = 0; i < users.Length; i++)
                    if (DateTime.Now.Subtract(users[i].LastSeenOn).TotalDays >= 7)
                        MessageCache.DeleteUser(users[i].UserId);

                MessageCache.DumpCacheToFile();
            }
            catch(Exception ex)
            {
                _log.Write(Misc.LogSeverity.Error, ex);
            }
        }

        private void Shutdown()
        {
            Instance.DiscordClient.LogoutAsync();
            Instance.DiscordClient.StopAsync();
            Instance.DiscordClient.Dispose();

            Logger.GetLoggerInstance().Write(Misc.LogSeverity.Info, "Backing up database...");

            if (File.Exists(FileDirectories.DatabaseCopyPath))
                File.Delete(FileDirectories.DatabaseCopyPath);
            File.Copy(FileDirectories.Database, FileDirectories.DatabaseCopyPath);

            Logger.GetLoggerInstance().Write(Misc.LogSeverity.Info, "Backing up message cache...");
            MessageCache.DumpCacheToFile();

            Logger.GetLoggerInstance().SendNotificationFromFirebaseCloud("Yuki, offline", "Process terminated", "INC_YUKI_TERMINATED", "Process terminated.");

            Thread.Sleep(1000); //Sleep for 1s to give things enough time to back up.
        }


        public void AddTo(int shard, ulong userId)
        {
            if (!Instance.members.ContainsKey(shard))
                Instance.members.Add(shard, new List<ulong>());

            if(!Instance.members[shard].Contains(userId))
                Instance.members[shard].Add(userId);
        }

        public void RemoveFrom(int shard, ulong userId)
        {
            if (Instance.members == null)
                Instance.members = new Dictionary<int, List<ulong>>();

            if (Instance.members.ContainsKey(shard) && members[shard].Contains(userId))
                Instance.members[shard].Remove(userId);
        }
        
        public void ShardReady(DiscordSocketClient client)
        {
            Instance.ShardsReady++;
            Instance.GetMembers(client);
        }

        public void ShardConnected()
            => Instance.ConnectedShards++;
    }
}
