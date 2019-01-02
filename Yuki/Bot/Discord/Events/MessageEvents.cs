using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Services;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Discord.Events
{
    public class MessageEvents
    {
        public async Task MessageRecieved(SocketMessage messageParam)
        { int argPos = 0;
            SocketUserMessage message = (SocketUserMessage)messageParam;

            MessageCache.CacheMessage(message);
            await Responses.Check(message);

            if ((message.Channel is IGuildChannel))
                //await Levels.DoLevelChecking(message);
                await Slowmode.Check(message);

            if (message == null || !HasPrefix(message, ref argPos))
                return;
            if ((message.Channel is IGuildChannel) && RateLimiter.Limited((IGuildUser)message.Author, (ITextChannel)message.Channel))
                return;

            await CustomCommands.Check(message);

            SocketCommandContext context;

            context = new SocketCommandContext(YukiClient.Instance.GetShard(0), message);

            IResult result = await YukiClient.Instance.CommandService.ExecuteAsync(context, argPos, YukiClient.Instance.Services);
        }

        public static bool HasPrefix(SocketUserMessage message, ref int argPos)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                string customPrefix = null;

                if (!(message.Channel is IDMChannel))
                {
                    ulong guildId = ((IGuildChannel)message.Channel).GuildId;
                    if (uow.CustomPrefixRepository != null && uow.CustomPrefixRepository.GetPrefix(guildId) != null)
                        customPrefix = uow.CustomPrefixRepository.GetPrefix(guildId).prefix;
                }

                return (((message.HasStringPrefix(Localizer.YukiStrings.prefix_string, ref argPos) ||
                          message.HasStringPrefix(Localizer.YukiStrings.prefix, ref argPos)) ||
                         ((message.Channel is IGuildChannel) && customPrefix != null && message.HasStringPrefix(customPrefix, ref argPos))) &&
                         !message.Author.IsBot);
            }
        }
    }
}