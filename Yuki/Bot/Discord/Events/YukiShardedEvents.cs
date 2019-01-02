using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Timers;
using Yuki.Bot.Misc;

namespace Yuki.Bot.Discord.Events
{
    public class YukiShardedEvents
    {
        private Logger _log = Logger.GetLoggerInstance();

        private GuildEvents events = new GuildEvents();

        public Task ShardReady(DiscordSocketClient client)
        {
            if (!YukiClient.Instance.ShardReady(client.ShardId))
            {
                Timer playing = new Timer(300000);
                playing.Elapsed += new ElapsedEventHandler((EventHandler)delegate (object sender, EventArgs e)
                {
                    client.SetGameAsync(new YukiRandom().RandomGame(client)).GetAwaiter().GetResult();
                });

                YukiClient.Instance.ShardReady(client);
                SetupGuildEvents(client);

                playing.Start();
                client.SetGameAsync(new YukiRandom().RandomGame(client)).GetAwaiter().GetResult();
            }

            return Task.CompletedTask;
        }

        public Task ShardConnected(DiscordSocketClient client)
        {
            ConsoleColor color = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Logger.GetLoggerInstance().Write(Misc.LogSeverity.Info, "Shard " + client.ShardId + " connected!");

            Console.ForegroundColor = color;

            if(!YukiClient.Instance.Connected)
                YukiClient.Instance.ShardConnected();

            return Task.CompletedTask;
        }

        public Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            _log.Write(LogSeverity.Error, "Shard " + client.ShardId + " disconnected. Reason: " + e.Message);
            return Task.CompletedTask;
        }

        private void SetupGuildEvents(DiscordSocketClient client)
        {
            client.UserVoiceStateUpdated += events.VoiceState;
            client.MessageReceived += new MessageEvents().MessageRecieved;
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
