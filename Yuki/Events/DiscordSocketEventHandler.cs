using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Events
{
    public static class DiscordSocketEventHandler
    {
        private static Language GetLanguage(SocketChannel channel)
        {
            return GetLanguage((channel as IGuildChannel).GuildId);
        }

        private static Language GetLanguage(ISocketMessageChannel channel)
        {
            return GetLanguage((channel as IGuildChannel).GuildId);
        }

        private static Language GetLanguage(ulong guildId)
        {
            return Localization.GetLanguage(GuildSettings.GetGuild(guildId).LangCode);
        }

        public static async Task MessageUpdated(Cacheable<IMessage, ulong> messageOld, SocketMessage current, ISocketMessageChannel channel)
        {
            try
            {
                Messages.InsertOrUpdate(new YukiMessage()
                {
                    Id = current.Id,
                    AuthorId = current.Author.Id,
                    ChannelId = current.Channel.Id,
                    SendDate = DateTime.UtcNow,
                    Content = (current as SocketUserMessage).Resolve(TagHandling.FullName, TagHandling.NameNoPrefix, TagHandling.Name, TagHandling.Name, TagHandling.FullNameNoPrefix)
                });

                if (!current.Author.IsBot && current.Content != messageOld.Value.Content)
                {
                    string oldContent = messageOld.Value.Content;
                    string newContent = current.Content;

                    if (oldContent.Length > 1000)
                    {
                        oldContent = oldContent.Substring(0, 997) + "...";
                    }

                    if (newContent.Length > 1000)
                    {
                        newContent = newContent.Substring(0, 997) + "...";
                    }

                    Language lang = GetLanguage(channel);

                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(lang.GetString("event_message_updated"), current.Author.GetAvatarUrl())
                        .WithDescription($"{lang.GetString("event_message_id")}: {current.Id}\n" +
                                         $"{lang.GetString("event_message_channel")}: {MentionUtils.MentionChannel(channel.Id)} ({channel.Id})\n" +
                                         $"{lang.GetString("event_message_author")}: {MentionUtils.MentionUser(current.Author.Id)}")
                        .AddField(lang.GetString("event_message_old"), oldContent)
                        .AddField(lang.GetString("event_message_new"), newContent)
                        .WithColor(Color.Gold);

                    await LogMessageAsync(embed, (channel as IGuildChannel).GuildId);
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        public static async Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            await message.DownloadAsync();

            Messages.Remove(message.Value.Id);

            if (!message.Value.Author.IsBot)
            {
                string content = message.Value.Content;

                if (content.Length > 1000)
                {
                    content = content.Substring(0, 997) + "...";
                }

                Language lang = GetLanguage(channel);

                EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor(lang.GetString("event_message_deleted"), message.Value.Author.GetAvatarUrl())
                    .WithDescription($"{lang.GetString("event_message_id")}: {message.Value.Id}\n" +
                                     $"{lang.GetString("event_message_channel")}: {MentionUtils.MentionChannel(channel.Id)} ({channel.Id})\n" +
                                     $"{lang.GetString("event_message_author")}: {MentionUtils.MentionUser(message.Value.Author.Id)}")
                    .WithColor(Color.Red);

                if (!string.IsNullOrEmpty(content))
                {
                    embed.AddField(lang.GetString("message_content"), content);
                }

                if (message.Value.Attachments != null && message.Value.Attachments.Count > 0)
                {
                    string attachments = string.Empty;
                    string imageUrl = null;

                    IAttachment[] _attachments = message.Value.Attachments.ToArray();

                    imageUrl = _attachments.FirstOrDefault(img => img.ProxyUrl.IsImage())?.ProxyUrl;

                    for (int i = 0; i < _attachments.Length; i++)
                    {
                        attachments += $"[{_attachments[i].Filename}]({_attachments[i].ProxyUrl})\n";
                    }

                    if(imageUrl != null)
                    {
                        embed.WithImageUrl(imageUrl);
                    }

                    embed.AddField(lang.GetString("message_attachments"), attachments);
                }

                await LogMessageAsync(embed, ((IGuildChannel)channel).GuildId);
            }
        }

        public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                IUserMessage msg = await message.GetOrDownloadAsync();

                if (channel is IDMChannel)
                {
                    return;
                }

                IGuild guild = (channel as IGuildChannel).Guild;

                IGuildUser user = await guild.GetUserAsync(msg.Author.Id);

                GuildConfiguration config = GuildSettings.GetGuild(guild.Id);


                Starboard.Manage(msg, channel as ITextChannel, reaction);


                /* Reaction roles */
                if (config.Equals(null) || !config.EnableReactionRoles)
                {
                    return;
                }

                ReactionMessage reactionMessage = config.ReactableMessages.FirstOrDefault(_message => _message.Id == msg.Id);

                if (reactionMessage.Equals(default) || reactionMessage.Reactions == null)
                {
                    return;
                }

                foreach (MessageReaction r in reactionMessage.Reactions)
                {
                    Emote emote = null;

                    Emoji emoji = null;

                    if (!(Emote.TryParse(r.Emote, out emote)))
                    {
                        emoji = new Emoji(r.Emote);
                    }
                    if (!(emoji == null && emote == null))
                    {
                        await user.AddRoleAsync(guild.GetRole(r.RoleId));
                        return;
                    }
                }
            }
            catch(Exception e)
            {
                await channel.SendMessageAsync(e.ToString());
            }
        }

        public static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {

                /* Reaction roles */
                IUserMessage msg = await message.GetOrDownloadAsync();

                if (channel is IDMChannel)
                {
                    return;
                }

                IGuild guild = (channel as IGuildChannel).Guild;

                IGuildUser user = await guild.GetUserAsync(msg.Author.Id);

                GuildConfiguration config = GuildSettings.GetGuild(guild.Id);


                Starboard.Manage(msg, channel as ITextChannel, reaction, true);


                /* Reaction roles */
                if (config.Equals(null) || !config.EnableReactionRoles)
                {
                    return;
                }

                ReactionMessage reactionMessage = config.ReactableMessages.FirstOrDefault(_msg => _msg.Id == msg.Id);

                if (reactionMessage.Equals(default) || (reactionMessage.Reactions == null || reactionMessage.Reactions.Count < 1))
                {
                    return;
                }

                foreach (MessageReaction r in reactionMessage.Reactions)
                {
                    Emote emote = null;

                    Emoji emoji = null;

                    if (!(Emote.TryParse(r.Emote, out emote)))
                    {
                        emoji = new Emoji(r.Emote);
                    }
                    if (!(emoji == null && emote == null))
                    {
                        if(!user.RoleIds.Contains(guild.GetRole(r.RoleId).Id))
                        {
                            await user.RemoveRoleAsync(guild.GetRole(r.RoleId));
                        }

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(e.ToString());
            }
        }

        /*public static async Task ReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel) { }*/


        public static async Task UserBanned(SocketUser user, SocketGuild guild)
        {
            Language lang = GetLanguage(guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_user_banned"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessageAsync(embed, guild.Id);
        }

        public static async Task UserJoined(SocketGuildUser user)
        {
            Language lang = GetLanguage(user.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor(lang.GetString("event_user_join"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessageAsync(embed, user.Guild.Id);

            GuildConfiguration guild = GuildSettings.GetGuild(user.Guild.Id);

            if(guild.EnableWelcome && !string.IsNullOrWhiteSpace(guild.WelcomeMessage) && user.Guild.Channels.ToList().Any(ch => ch.Id == guild.WelcomeChannel))
            {
                YukiContextMessage msg = new YukiContextMessage(user, user.Guild);

                await user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(StringReplacements.GetReplacement(guild.WelcomeMessage, msg));
            }
        }

        public static async Task UserLeft(SocketGuildUser user)
        {
            Language lang = GetLanguage(user.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_user_leave"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessageAsync(embed, user.Guild.Id);

            GuildConfiguration guild = GuildSettings.GetGuild(user.Guild.Id);

            if (guild.EnableGoodbye && !string.IsNullOrWhiteSpace(guild.GoodbyeMessage) && user.Guild.Channels.ToList().Any(ch => ch.Id == guild.WelcomeChannel))
            {
                YukiContextMessage msg = new YukiContextMessage(user, user.Guild);

                await user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(StringReplacements.GetReplacement(guild.GoodbyeMessage, msg));
            }
        }

        public static async Task UserUnbanned(SocketUser user, SocketGuild guild)
        {
            Language lang = GetLanguage(guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor(lang.GetString("event_user_unban"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessageAsync(embed, guild.Id);
        }

        public static async Task LogMessageAsync(EmbedBuilder embed, ulong guildId)
        {
            embed.WithFooter(DateTime.UtcNow.ToPrettyTime(false, true));
            
            GuildConfiguration config = GuildSettings.GetGuild(guildId);

            if(!config.Equals(null) && config.EnableLogging && config.LogChannel != 0 && YukiBot.Discord.Client.GetGuild(guildId).TextChannels.Any(c => c.Id == config.LogChannel))
            {
                await YukiBot.Discord.Client.GetGuild(guildId).GetTextChannel(config.LogChannel).SendMessageAsync("", false, embed.Build());
            }
        }
    }
}
