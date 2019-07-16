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
            client.SetGameAsync("Shard " + client.ShardId + "  (" + client.Guilds.Select(guild => guild.MemberCount).Sum() + " members)");
            SetClientEvents(client);

            return Task.CompletedTask;
        }

        public static Task ShardConnected(DiscordSocketClient client)
        {
            LoggingService.Write(LogLevel.Status, "Shard " + client.ShardId + ": connected");

            return Task.CompletedTask;
        }

        public static Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if(!YukiBot.Services.GetRequiredService<YukiBot>().IsShuttingDown)
            {
                LoggingService.Write(LogLevel.Error, "Shard " + client.ShardId + ": disconnected. Reason: " + e);

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

            client.MessageReceived += DiscordSocketEventHandler.MessageReceived;
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
