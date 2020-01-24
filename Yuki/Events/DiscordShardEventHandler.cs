using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;
using Yuki.Core;
using Discord;

namespace Yuki.Events
{
    public static class DiscordShardEventHandler
    {
        private static int connectedShards = 0;

        public static Task ShardReady(DiscordSocketClient client)
        {
            connectedShards++;

            /* Disable the Ready event */
            if(connectedShards == YukiBot.Discord.ShardCount)
            {
                YukiBot.Discord.Client.ShardReady -= ShardReady;
            }

            SetClientEvents(client);

            string message = System.IO.File.ReadAllLines(FileDirectories.StatusMessages)[0];

            client.SetGameAsync(name: message.Replace("{shard}", client.ShardId.ToString())
                                             .Replace("{users}", client.Guilds.Select(guild => guild.MemberCount).Sum().ToString())
                                             .Replace("{guildcount}", client.Guilds.Count.ToString()),
                                             streamUrl: null, type: ActivityType.Playing);

            return Task.CompletedTask;
        }

        public static Task ShardConnected(DiscordSocketClient client)
        {
            Logger.Write(LogLevel.Status, $"Shard {client.ShardId} connected");

            return Task.CompletedTask;
        }

        public static Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if (!YukiBot.ShuttingDown)
            {
                Logger.Write(LogLevel.Error, $"Shard {client.ShardId} disconnected. Reason: " + e);

                //await YukiBot.Discord.StopAsync();
                //Thread.Sleep(500);
                //await YukiBot.Discord.LoginAsync(Config.GetConfig().token);
            }

            return Task.CompletedTask;
        }

        private static void SetClientEvents(DiscordSocketClient client)
        {
            client.MessageReceived += DiscordSocketEventHandler.MessageReceived;

            client.MessageUpdated += DiscordSocketEventHandler.MessageUpdated;
            client.MessageDeleted += DiscordSocketEventHandler.MessageDeleted;


            client.ReactionAdded += DiscordSocketEventHandler.ReactionAdded;
            client.ReactionRemoved += DiscordSocketEventHandler.ReactionRemoved;

            /* client.ReactionsCleared += DiscordSocketEventHandler.ReactionsCleared; */

            client.UserBanned += DiscordSocketEventHandler.UserBanned;
            client.UserJoined += DiscordSocketEventHandler.UserJoined;
            client.UserLeft += DiscordSocketEventHandler.UserLeft;
            client.UserUnbanned += DiscordSocketEventHandler.UserUnbanned;

            client.RoleDeleted += DiscordSocketEventHandler.RoleDeleted;
        }
    }
}
