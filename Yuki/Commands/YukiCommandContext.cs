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

        public DiscordSocketClient Client { get; }
        public IServiceProvider ServiceProvider { get; }
        public IGuild Guild { get; }
        public ITextChannel Channel { get; }
        public IGuildUser User { get; }
        public IUserMessage Message { get; }

        public YukiCommandContext(IDiscordClient client, IUserMessage msg, IServiceProvider provider)
        {
            ServiceProvider = provider;
            Guild = (msg.Channel as ITextChannel)?.Guild;
            Channel = msg.Channel as ITextChannel;
            User = msg.Author as IGuildUser;
            Message = msg;

            if (client is DiscordShardedClient)
            {
                Client = ((DiscordShardedClient)client).GetShardFor(Guild);
            }
        }

        public Embed CreateEmbed(string content) => new EmbedBuilder().WithColor(Color.Green).WithAuthor(User)
            .WithDescription(content).Build();

        public EmbedBuilder CreateEmbedBuilder(string content = null) => new EmbedBuilder()
            .WithColor(Color.Green).WithAuthor(User).WithDescription(content ?? string.Empty);

        public Task ReplyAsync(string content) => Channel.SendMessageAsync(content);
        public Task ReplyAsync(Embed embed) => embed.SendToAsync(Channel);
        public Task ReplyAsync(EmbedBuilder embed) => embed.SendToAsync(Channel);
        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}
