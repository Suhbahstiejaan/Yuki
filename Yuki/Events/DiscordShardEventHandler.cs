using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Yuki.Core;
using System.Timers;
using Yuki.Data;
using Yuki.Services;

namespace Yuki.Events
{
    public static class DiscordShardEventHandler
    {
        public static Task ShardReady(DiscordSocketClient client)
        {
            //used as a reference
            List<YukiShard> shards = YukiBot.Services.GetRequiredService<YukiBot>().Shards;

            if (shards != null && shards.FirstOrDefault(shard => shard.Id == client.ShardId).Equals(default(YukiShard)))
            {
                YukiShard shard = new YukiShard();

                shard.Id = client.ShardId;
                //shard.Members = client.Guilds.SelectMany(guild => guild.Users).Select(user => user.Id).ToList();
                shard.Members = client.Guilds.SelectMany(guild => guild.Users).Select(user => user.Id).Where(user => shards.FirstOrDefault(sh => sh.Members.Contains(user)).Equals(default(YukiShard))).ToList();

                shard.Playing = new Timer(Config.GetConfig().playing_seconds * 1000);
                shard.Playing.Elapsed += new ElapsedEventHandler((EventHandler)delegate (object sender, EventArgs e)
                {
                    client.SetGameAsync("Shard " + shard.Id  + "  (" + shard.Members.Count  + " members)");
                    //client.SetGameAsync(new YukiRandom().RandomGame()).GetAwaiter().GetResult();
                });

                client.SetGameAsync("Shard " + shard.Id + "  (" + shard.Members.Count + " members)");
                //client.SetGameAsync(new YukiRandom().RandomGame()).GetAwaiter().GetResult();

                SetClientEvents(client);

                shard.Playing.Start();
                
                YukiBot.Services.GetRequiredService<YukiBot>().Shards.Add(shard);
            }
            else
            {
                YukiBot.Services.GetRequiredService<LoggingService>().Write(LogLevel.Warning, "SocketClient with ID " + client.ShardId + " already connected!");
            }

            return Task.CompletedTask;
        }

        public static Task ShardConnected(DiscordSocketClient client)
        {
            YukiBot.Services.GetRequiredService<LoggingService>().Write(LogLevel.Status, "Shard " + client.ShardId + ": connected");

            return Task.CompletedTask;
        }

        public static Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if(!YukiBot.Services.GetRequiredService<YukiBot>().IsShuttingDown)
            {
                YukiBot.Services.GetRequiredService<LoggingService>().Write(LogLevel.Error, "Shard " + client.ShardId + ": disconnected. Reason: " + e);

                client.StopAsync();
                client.StartAsync();
            }

            return Task.CompletedTask;
        }

        private static void SetClientEvents(DiscordSocketClient client)
        {
            client.JoinedGuild += DiscordSocketEventHandler.JoinedGuild;
            client.LeftGuild += DiscordSocketEventHandler.LeftGuild;

            client.ChannelCreated += DiscordSocketEventHandler.ChannelCreated;
            client.ChannelDestroyed += DiscordSocketEventHandler.ChannelDestroyed;
            client.ChannelUpdated += DiscordSocketEventHandler.ChannelUpdated;

            client.GuildMemberUpdated += DiscordSocketEventHandler.GuildMemberUpdated;

            client.GuildUpdated += DiscordSocketEventHandler.GuildUpdated;

            client.MessageReceived += DiscordSocketMessageEventHandler.MessageReceived;
            client.MessageUpdated += DiscordSocketEventHandler.MessageUpdated;
            client.MessageDeleted += DiscordSocketEventHandler.MessageDeleted;

            client.ReactionAdded += DiscordSocketEventHandler.ReactionAdded;
            client.ReactionRemoved += DiscordSocketEventHandler.ReactionRemoved;
            client.ReactionsCleared += DiscordSocketEventHandler.ReactionsCleared;

            client.RoleCreated += DiscordSocketEventHandler.RoleCreated;
            client.RoleDeleted += DiscordSocketEventHandler.RoleDeleted;
            client.RoleUpdated += DiscordSocketEventHandler.RoleUpdated;

            client.UserBanned += DiscordSocketEventHandler.UserBanned;
            client.UserJoined += DiscordSocketEventHandler.UserJoined;
            client.UserLeft += DiscordSocketEventHandler.UserLeft;
            client.UserUnbanned += DiscordSocketEventHandler.UserUnbanned;

            client.VoiceServerUpdated += DiscordSocketEventHandler.VoiceServerUpdated;
        }
    }
}
