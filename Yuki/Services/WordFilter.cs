using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Events;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public static class WordFilter
    {
        public static async Task CheckFilter(SocketUserMessage message)
        {
            IGuildChannel guildChannel = (message.Channel as IGuildChannel);

            GuildConfiguration guild = GuildSettings.GetGuild(guildChannel.GuildId);

            Language lang = Localization.GetLanguage(guild.LangCode);

            if (guild.EnableFilter)
            {
                List<string> filter = guild.WordFilter;

                foreach (string wordFilter in filter)
                {
                    if (Regex.IsMatch(message.Content, $@"\b{wordFilter}\b", RegexOptions.IgnoreCase) || message.Content.LevenshteinDistance(wordFilter) <= 1)
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
    }
}
