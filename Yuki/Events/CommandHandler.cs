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

                bool hasPrefix = HasPrefix(message, out string output);


                if (!(message.Channel is IDMChannel))
                {
                    IGuildChannel guildChannel = (message.Channel as IGuildChannel);

                    GuildConfiguration guild = GuildSettings.GetGuild(guildChannel.GuildId);

                    Language lang = Localization.GetLanguage(guild.LangCode);

                    if (guild.EnableFilter)
                    {
                        List<string> filter = guild.WordFilter;

                        foreach (string wordFilter in filter)
                        {
                            if (Regex.IsMatch(message.Content, $@"{wordFilter}", RegexOptions.IgnoreCase))
                            {
                                if (guildChannel.Guild.GetUserAsync(message.Author.Id).Result.RoleIds.Any(guild.ModeratorRoles.Contains) ||
                                    guildChannel.Guild.GetUserAsync(message.Author.Id).Result.RoleIds.Any(guild.ModeratorRoles.Contains) ||
                                    guildChannel.Guild.OwnerId == message.Author.Id)
                                {
                                    /* Add exclamation mark emote */
                                    await message.AddReactionAsync(new Emoji("❗"));
                                }
                                else
                                {
                                    EmbedBuilder embed = new EmbedBuilder()
                                    .WithAuthor(lang.GetString("event_filter_triggered"), message.Author.GetAvatarUrl())
                                    .WithDescription($"{lang.GetString("event_message_id")}: {message.Id}\n" +
                                                     $"{lang.GetString("event_message_channel")}: {MentionUtils.MentionChannel(guildChannel.Id)} ({guildChannel.Id})\n" +
                                                     $"{lang.GetString("event_message_author")}: {MentionUtils.MentionUser(message.Author.Id)}")
                                    .AddField(lang.GetString("message_content"), message.Content)
                                    .WithColor(Color.Purple);

                                    await message.DeleteAsync();
                                    await DiscordSocketEventHandler.LogMessageAsync(embed, guildChannel.GuildId);
                                }
                                break;
                            }
                        }
                    }
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
                           .ExecuteAsync(output, new YukiCommandContext(
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
                                                        .FirstOrDefault(cmd => cmd.Name.ToLower() == message.Content.ToLower());

                if (!execCommand.Equals(default) && !execCommand.Equals(null) && !string.IsNullOrEmpty(execCommand.Response) && HasPrefix(message, out output))
                {
                    await message.Channel.SendMessageAsync(execCommand.Response);
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
