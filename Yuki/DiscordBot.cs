using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Yuki.Commands.TypeParsers;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Events;

namespace Yuki
{
    public class DiscordBot
    {
        public IServiceProvider Services { get; private set; }

        public DiscordShardedClient Client;
        public CommandService CommandService;

        public int ShardCount { get; private set; }

        public async Task LoginAsync(string token)
        {
            Client = new DiscordShardedClient();

            await Client.LoginAsync(TokenType.Bot, token);

            ShardCount = await Client.GetRecommendedShardCountAsync();

            //cleanup
            await Client.LogoutAsync();

            Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = ShardCount * 1000,
                TotalShards = ShardCount,
            });

            CreateCommandService();
            CreateServices();
            SetEvents();
        }

        private void CreateCommandService()
        {
            CommandService = new CommandService(new CommandServiceConfiguration()
            {
                CaseSensitive = false,
                DefaultRunMode = RunMode.Parallel,
                CooldownBucketKeyGenerator = BucketKey.Generate
            });

            CommandService.AddModules(Assembly.GetEntryAssembly());
            CommandService.AddTypeParser(new UserTypeParser<IUser>());
        }

        private void CreateServices()
        {
            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(new InteractivityService(Client, TimeSpan.FromSeconds(Config.GetConfig().command_timeout_seconds)))
                .BuildServiceProvider();
        }

        private void SetEvents()
        {
            Client.ShardReady += DiscordShardEventHandler.ShardReady;
            Client.ShardConnected += DiscordShardEventHandler.ShardConnected;
            Client.ShardDisconnected += DiscordShardEventHandler.ShardDisconnected;
        }

        public async Task StopAsync()
        {
            await Client.LogoutAsync();
            await Client.StopAsync();

            Client.Dispose();
        }
    }
}
