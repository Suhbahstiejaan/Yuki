using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Core;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Events
{
    public class CommandHandler
    {
        public static async Task HandleCommand(SocketMessage socketMessage)
        {
            try
            {
                if (!(socketMessage is SocketUserMessage message))
                    return;
                if (message.Source != MessageSource.User)
                    return;

                DiscordSocketClient shard = (message.Channel is IGuildChannel) ?
                                                YukiBot.Discord.Client.GetShardFor(((IGuildChannel)message.Channel).Guild) :
                                                YukiBot.Discord.Client.GetShard(0);

                bool hasPrefix = HasPrefix(message, out string trimmedContent);


                if (!(message.Channel is IDMChannel))
                {
                    await WordFilter.CheckFilter(message);
                }

                if (!hasPrefix)
                {
                    Messages.InsertOrUpdate(new YukiMessage()
                    {
                        Id = message.Id,
                        AuthorId = message.Author.Id,
                        ChannelId = message.Channel.Id,
                        SendDate = DateTime.UtcNow,
                        Content = (socketMessage as SocketUserMessage)
                                .Resolve(TagHandling.FullName, TagHandling.NameNoPrefix, TagHandling.Name, TagHandling.Name, TagHandling.FullNameNoPrefix)
                    });

                    return;
                }

                IResult result = await YukiBot.Discord.CommandService
                           .ExecuteAsync(trimmedContent, new YukiCommandContext(
                                YukiBot.Discord.Client, socketMessage as IUserMessage, YukiBot.Discord.Services));

                if (result is FailedResult failedResult)
                {
                    if (!(failedResult is CommandNotFoundResult) && failedResult is ChecksFailedResult checksFailed)
                    {
                        if (checksFailed.FailedChecks.Count == 1)
                        {
                            await message.Channel.SendMessageAsync(checksFailed.FailedChecks[0].Error);
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync($"The following checks failed:\n\n" +
                                    $"{string.Join("\n", checksFailed.FailedChecks.Select(check => check.Error))}");
                        }
                    }
                }

                if (message.Channel is IDMChannel)
                {
                    return;
                }
 
                GuildCommand execCommand = GuildSettings.GetGuild((message.Channel as IGuildChannel).GuildId).Commands
                                                        .FirstOrDefault(cmd => cmd.Name.ToLower() == trimmedContent.ToLower());

                if (!execCommand.Equals(null) && !execCommand.Equals(default) && !execCommand.Equals(null) && !string.IsNullOrEmpty(execCommand.Response))
                {
                    YukiContextMessage msg = new YukiContextMessage(message.Author, (message.Author as IGuildUser).Guild);

                    await message.Channel.SendMessageAsync(StringReplacements.GetReplacement(execCommand.Response, msg));
                }
            }
            catch(Exception e) { await socketMessage.Channel.SendMessageAsync(e.ToString()); }
        }

        private static bool HasPrefix(SocketUserMessage message, out string output)
        {
            output = string.Empty;
            if (!(message.Channel is IDMChannel))
            {
                GuildConfiguration config = GuildSettings.GetGuild((message.Channel as IGuildChannel).GuildId);

                if(config.EnablePrefix && config.Prefix != null)
                {
                    return CommandUtilities.HasPrefix(message.Content, config.Prefix, out output);
                }
            }

            return CommandUtilities.HasAnyPrefix(message.Content, Config.GetConfig().prefix.AsReadOnly(), out string prefix, out output);
        }
    }
}
