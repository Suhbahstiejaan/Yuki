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
using System.Diagnostics;
using Yuki.Data;

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

                string uptime = Process.GetCurrentProcess().StartTime.ToUniversalTime().ToPrettyTime(true, false);

                string randomStatus = statuses[new YukiRandom().Next(statuses.Count)];

                await client.SetGameAsync(name: randomStatus.Replace("%shardid%", client.ShardId.ToString())
                                                            .Replace("%usercount%", client.Guilds.Select(guild => guild.MemberCount).Sum().ToString())
                                                            .Replace("%guildcount%", client.Guilds.Count.ToString())
                                                            .Replace("%uptime%", uptime),
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

        public static async Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if (!YukiBot.ShuttingDown)
            {
                Logger.Write(LogLevel.Error, $"Shard {client.ShardId} disconnected. Reason: " + e);

                await YukiBot.Discord.StopAsync();
                Thread.Sleep(500);
                await YukiBot.Discord.LoginAsync(Config.GetConfig().token);
            }
        }

        private static void SetClientEvents(DiscordSocketClient client)
        {
            client.MessageReceived += CommandHandler.HandleCommand;

            client.MessageUpdated += DiscordSocketEventHandler.MessageUpdated;
            client.MessageDeleted += DiscordSocketEventHandler.MessageDeleted;

            /* To uncomment once reaction roles are implemented
             
                client.ReactionAdded += DiscordSocketEventHandler.ReactionAdded;
                client.ReactionRemoved += DiscordSocketEventHandler.ReactionRemoved;
                client.ReactionsCleared += DiscordSocketEventHandler.ReactionsCleared;
            */

            client.UserBanned += DiscordSocketEventHandler.UserBanned;
            client.UserJoined += DiscordSocketEventHandler.UserJoined;
            client.UserLeft += DiscordSocketEventHandler.UserLeft;
            client.UserUnbanned += DiscordSocketEventHandler.UserUnbanned;
        }
    }
}
