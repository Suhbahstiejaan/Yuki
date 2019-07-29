using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;
using Yuki.Core;
using System.Collections.Generic;
using Yuki.Data.Objects;
using Nett;
using Discord;
using Yuki.Extensions;
using System.Threading;

namespace Yuki.Events
{
    public static class DiscordShardEventHandler
    {
        public static Task ShardReady(DiscordSocketClient client)
        {
            SetClientEvents(client);

            System.Timers.Timer status = new System.Timers.Timer();
            status.Interval = 1;

            status.Elapsed += new System.Timers.ElapsedEventHandler((EventHandler)async delegate (object sender, EventArgs e)
            {
                int statusType = new YukiRandom().Next(1, 100);

                List<string> statuses;
                ActivityType activity;

                StatusMessages messages = Toml.ReadFile<StatusMessages>(FileDirectories.StatusMessages);

                if(statusType <= 33)
                {
                    statuses = messages.Listening;
                    activity = ActivityType.Listening;
                }
                else if(statusType > 33 && statusType <= 66)
                {
                    statuses = messages.Watching;
                    activity = ActivityType.Watching;
                }
                else
                {
                    statuses = messages.Playing;
                    activity = ActivityType.Playing;
                }

                string randomStatus = statuses[new YukiRandom().Next(statuses.Count)];
                await client.SetGameAsync(name: randomStatus.Replace("%shardid%", client.ShardId.ToString())
                                                            .Replace("%usercount%", client.Guilds.Select(guild => guild.MemberCount).Sum().ToString())
                                                            .Replace("%guildcount%", client.Guilds.Count.ToString())
                                                            .Replace("%uptime%", new DateTime((DateTime.Now - YukiBot.StartTime).Ticks).ToPrettyTime(true, false)),
                                          streamUrl: null, type: activity);

                status.Interval = TimeSpan.FromMinutes(new YukiRandom().Next(1, 5)).TotalMilliseconds;
            });

            status.Start();

            return Task.CompletedTask;
        }

        public static Task ShardConnected(DiscordSocketClient client)
        {
            Logger.Write(LogLevel.Status, $"Shard {client.ShardId} connected");

            return Task.CompletedTask;
        }

        public static Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if (YukiBot.ShuttingDown) { return Task.CompletedTask; }

            Logger.Write(LogLevel.Error, $"Shard {client.ShardId} disconnected. Reason: " + e);

            client.StopAsync();
            Thread.Sleep(500);
            client.StartAsync();

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
