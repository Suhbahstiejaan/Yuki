using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Threading.Tasks;
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

        public Embed CreateEmbed(string content) => new EmbedBuilder().WithColor(Color.Green).WithAuthor(User)
            .WithDescription(content).Build();

        public EmbedBuilder CreateImageEmbedBuilder(string title, string url) => new EmbedBuilder().WithColor(Color.Green)
            .WithAuthor(new EmbedAuthorBuilder() { Name = title }).WithImageUrl(url);

        public EmbedBuilder CreateEmbedBuilder(string content = null) => new EmbedBuilder()
            .WithColor(Color.Green).WithAuthor(User).WithDescription(content ?? string.Empty);

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
        public Task<IUserMessage> ReplyAsync(Embed embed) => embed.SendToAsync(Channel);
        public Task<IUserMessage> ReplyAsync(EmbedBuilder embed) => embed.SendToAsync(Channel);
        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}
