using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Events
{
    public static class DiscordSocketEventHandler
    {
        public static async Task JoinedGuild(SocketGuild guild) { }
        public static async Task LeftGuild(SocketGuild guild) { }

        public static async Task ChannelCreated(SocketChannel channel) { }
        public static async Task ChannelDestroyed(SocketChannel channel) { }
        public static async Task ChannelUpdated(SocketChannel channelOld, SocketChannel channel) { }

        public static async Task GuildMemberUpdated(SocketGuildUser userOld, SocketGuildUser user) { }

        public static async Task GuildUpdated(SocketGuild guildOld, SocketGuild guild) { }
        
        public static Task MessageUpdated(Cacheable<IMessage, ulong> messageOld, SocketMessage current, ISocketMessageChannel channel)
        {
            Messages.InsertOrUpdate(new YukiMessage()
            {
                Id = current.Id,
                AuthorId = current.Author.Id,
                ChannelId = current.Channel.Id,
                Content = current.Content
            });

            return Task.CompletedTask;
        }
        public static Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            Messages.Remove(new YukiMessage()
            {
                Id = message.Value.Id,
                AuthorId = message.Value.Author.Id,
                ChannelId = message.Value.Channel.Id,
                Content = message.Value.Content
            });

            return Task.CompletedTask;
        }


        public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction) { }
        public static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction) { }
        public static async Task ReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel) { }

        public static async Task RoleCreated(SocketRole role) { }
        public static async Task RoleDeleted(SocketRole role) { }
        public static async Task RoleUpdated(SocketRole roleOld, SocketRole role) { }

        public static async Task UserBanned(SocketUser user, SocketGuild guild) { }
        public static async Task UserJoined(SocketGuildUser user) { }
        public static async Task UserLeft(SocketGuildUser user) { }
        public static async Task UserUnbanned(SocketUser user, SocketGuild guild) { }

        public static async Task VoiceServerUpdated(SocketVoiceServer voiceServer) { }
    }
}
