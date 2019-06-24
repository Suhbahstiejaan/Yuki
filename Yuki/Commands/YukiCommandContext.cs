using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Extensions;

namespace Yuki.Commands
{
    public sealed class YukiCommandContext : ICommandContext
    {

        public DiscordShardedClient Client { get; }
        public IServiceProvider ServiceProvider { get; }
        public IGuild Guild { get; }
        public IMessageChannel Channel { get; }
        public IUser User { get; }
        public IUserMessage Message { get; }

        public YukiCommandContext(DiscordShardedClient client, IUserMessage msg, IServiceProvider provider)
        {
            ServiceProvider = provider;

            Client = client;
            Guild = (msg.Channel as IGuildChannel)?.Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }

        public EmbedAuthorBuilder Author
            => new EmbedAuthorBuilder()
                {
                    IconUrl = User.GetAvatarUrl(),
                    Name = User.Username
                };

        public Embed CreateEmbed(string content, EmbedAuthorBuilder author = null) => CreateEmbedBuilder(content, author).Build();

        public EmbedBuilder CreateImageEmbedBuilder(string title, string url) => CreateEmbedBuilder("", new EmbedAuthorBuilder() { Name = title }).WithImageUrl(url);

        public EmbedBuilder CreateEmbedBuilder(string content = null, EmbedAuthorBuilder author = null) => new EmbedBuilder()
            .WithColor(Colors.Pink).WithAuthor(author ?? Author).WithDescription(content ?? string.Empty);

        public async Task<bool> TryDeleteAsync(IUserMessage message)
        {
            try
            {
                await message.DeleteAsync();
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool UserHasPermission(GuildPermission permision)
        {
            if(Channel is IDMChannel)
            {
                return false;
            }
            
            return (User as IGuildUser).GuildPermissions.Has(permision);
        }

        public async Task<bool> TryDeleteAsync(SocketMessage message)
            => await TryDeleteAsync((IUserMessage)message);


        public Task<IUserMessage> ReplyAsync(string content, Embed embed) => Channel.SendMessageAsync(content, false, embed);
        public Task<IUserMessage> ReplyAsync(string content, EmbedBuilder embed) => Channel.SendMessageAsync(content, false, embed.Build());
        public Task<IUserMessage> ReplyAsync(string content) => Channel.SendMessageAsync(content);
        public Task<IUserMessage> ReplyAsync(object content) => Channel.SendMessageAsync(content.ToString());
        public Task<IUserMessage> ReplyAsync(Embed embed) => embed.SendToAsync(Channel);
        public Task<IUserMessage> ReplyAsync(EmbedBuilder embed) => embed.SendToAsync(Channel);
        public Task<IUserMessage> SendFileAsync(string fileName, EmbedBuilder embed) => SendFileAsync(fileName, embed.Build());
        public Task<IUserMessage> SendFileAsync(string fileName, Embed embed) => Channel.SendFileAsync(fileName, null, false, embed, null, false);
        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}
