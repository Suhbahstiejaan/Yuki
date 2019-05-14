using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Yuki.Bot.Entity;
using Yuki.Bot.Misc;

namespace Yuki.Bot.Common.Events
{
    public class YukiShardedEvents
    {
        private GuildEvents events = new GuildEvents();

        public Task ShardReady(DiscordSocketClient client)
        {
            if (!YukiClient.Instance.ShardConnected(client.ShardId))
            {
                Timer playing = new Timer(YukiClient.Instance.Config.StatusMessageSeconds * 1000);

                playing.Elapsed += new ElapsedEventHandler((EventHandler)delegate (object sender, EventArgs e)
                {
                    client.SetGameAsync(new YukiRandom().RandomGame(client)).GetAwaiter().GetResult();
                });


                List<ulong> members = new List<ulong>();

                foreach (SocketGuild guild in client.Guilds)
                    foreach (SocketGuildUser member in guild.Users)
                        if (!members.Contains(member.Id))
                            members.Add(member.Id);

                YukiClient.Instance.ConnectedShards.Add(new YukiShard(client.ShardId, members));
                SetupGuildEvents(client);


                playing.Start();
                client.SetGameAsync(new YukiRandom().RandomGame(client)).GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        public Task ShardConnected(DiscordSocketClient client)
        {
            Logger.Instance.Write(LogLevel.Success, "Shard " + client.ShardId + " connected!");

            
            if (YukiClient.Instance.ConnectedShards.Count == YukiClient.Instance.MaxShards)
            {
                Logger.Instance.Write(LogLevel.Debug, "Yuki, online!");
            }

            return Task.CompletedTask;
        }

        public Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if(!YukiClient.Instance.IsShuttingDown)
            {
                Logger.Instance.Write(LogLevel.Error, "Shard " + client.ShardId + " disconnected. Reason: " + e.Message);

                /* Remove shard from connected list */
                YukiClient.Instance.ConnectedShards.Remove(YukiClient.Instance.ConnectedShards.First(shard => shard.ShardId == client.ShardId));

                YukiClient.Instance.Restart(1);
            }

            return Task.CompletedTask;
        }

        private void SetupGuildEvents(DiscordSocketClient client)
        {
            client.UserVoiceStateUpdated += events.VoiceState;
            client.MessageReceived += MessageEvents.Instance.MessageRecieved;
            client.MessageDeleted += events.MessageDeleted;
            client.MessageUpdated += events.MessageEdited;
            client.UserUnbanned += events.UserUnbanned;
            client.JoinedGuild += events.JoinedGuild;
            client.UserJoined += events.UserJoined;
            client.UserBanned += events.UserBanned;
            client.LeftGuild += events.LeftGuild;
            client.UserLeft += events.UserLeft;
        }
    }
}