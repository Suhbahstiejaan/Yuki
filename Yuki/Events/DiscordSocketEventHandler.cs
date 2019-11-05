using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands;
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

        private static bool HasPrefix(SocketUserMessage message, out string output)
        {
            output = string.Empty;
            if (!(message.Channel is IDMChannel))
            {
                GuildConfiguration config = GuildSettings.GetGuild((message.Channel as IGuildChannel).GuildId);

                if (config.EnablePrefix && config.Prefix != null)
                {
                    return CommandUtilities.HasPrefix(message.Content, config.Prefix, out output);
                }
            }

            return CommandUtilities.HasAnyPrefix(message.Content, Config.GetConfig().prefix.AsReadOnly(), out string prefix, out output);
        }



        public static Task MessageReceived(SocketMessage socketMessage)
        {
            _ = Task.Run(() => {
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
                    WordFilter.CheckFilter(message);
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

                IResult result = YukiBot.Discord.CommandService
                                    .ExecuteAsync(trimmedContent, new YukiCommandContext(
                                            YukiBot.Discord.Client, socketMessage as IUserMessage, YukiBot.Discord.Services)).Result;

                if (result is FailedResult failedResult)
                {
                    if (!(failedResult is CommandNotFoundResult) && failedResult is ChecksFailedResult checksFailed)
                    {
                        if (checksFailed.FailedChecks.Count == 1)
                        {
                            message.Channel.SendMessageAsync(checksFailed.FailedChecks[0].Result.Reason);
                        }
                        else
                        {
                            message.Channel.SendMessageAsync($"The following checks failed:\n\n" +
                                    $"{string.Join("\n", checksFailed.FailedChecks.Select(check => check.Result.Reason))}");
                        }
                    }
                }

                if (UserSettings.IsPatron(message.Author.Id))
                {
                    PatronCommand cmd = Patreon.GetCommand(message.Author.Id, trimmedContent.Split(' ')[0].ToLower());

                    if (!cmd.Equals(default))
                    {
                        YukiContextMessage msg = new YukiContextMessage(message.Author, (message.Author as IGuildUser).Guild);

                        message.Channel.SendMessageAsync(StringReplacements.GetReplacement(trimmedContent, cmd.Response, msg));

                        return;
                    }
                }

                if (message.Channel is IDMChannel)
                {
                    return;
                }

                GuildCommand execCommand = GuildSettings.GetGuild((message.Channel as IGuildChannel).GuildId).Commands
                                                        .FirstOrDefault(cmd => cmd.Name.ToLower() == trimmedContent.Split(' ')[0].ToLower());

                if (!execCommand.Equals(null) && !execCommand.Equals(default) && !execCommand.Equals(null) && !string.IsNullOrEmpty(execCommand.Response))
                {
                    YukiContextMessage msg = new YukiContextMessage(message.Author, (message.Author as IGuildUser).Guild);

                    message.Channel.SendMessageAsync(StringReplacements.GetReplacement(trimmedContent, execCommand.Response, msg));
                }
            });

            return Task.CompletedTask;
        }

        public static Task MessageUpdated(Cacheable<IMessage, ulong> messageOld, SocketMessage current, ISocketMessageChannel channel)
        {
            _ = Task.Run(() => {
                try
                {
                    if(current.Content != null)
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

                            LogMessage(embed, (channel as IGuildChannel).GuildId);
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
            });

            return Task.CompletedTask;
        }

        public static Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            _ = Task.Run(() => {
                message.DownloadAsync();

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

                        if (imageUrl != null)
                        {
                            embed.WithImageUrl(imageUrl);
                        }

                        embed.AddField(lang.GetString("message_attachments"), attachments);
                    }

                    LogMessage(embed, ((IGuildChannel)channel).GuildId);
                }
            });

            return Task.CompletedTask;
        }

        public static Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            _ = Task.Run(() => {
                try
                {
                    IUserMessage msg = message.GetOrDownloadAsync().Result;

                    if (!message.HasValue || msg.Equals(null) || !reaction.User.IsSpecified)
                    {
                        return;
                    }


                    if (channel is IDMChannel)
                    {
                        return;
                    }

                    IGuild guild = (channel as IGuildChannel).Guild;

                    IGuildUser user = guild.GetUserAsync(msg.Author.Id).Result;

                    GuildConfiguration config = GuildSettings.GetGuild(guild.Id);


                    Starboard.Manage(msg, channel as ITextChannel, reaction);
                    ReactionRoles.Manage(msg, channel as ITextChannel, reaction, isUnreact: false);

                }
                catch (Exception e)
                {
                    channel.SendMessageAsync(e.ToString());
                }
            });

            return Task.CompletedTask;
        }

        public static Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    IUserMessage msg = message.GetOrDownloadAsync().Result;

                    if (channel is IDMChannel)
                    {
                        return;
                    }

                    IGuild guild = (channel as IGuildChannel).Guild;

                    IGuildUser user = guild.GetUserAsync(msg.Author.Id).Result;

                    GuildConfiguration config = GuildSettings.GetGuild(guild.Id);


                    Starboard.Manage(msg, channel as ITextChannel, reaction, true);
                    ReactionRoles.Manage(msg, channel as ITextChannel, reaction, isUnreact: true);
                }
                catch (Exception e)
                {
                    channel.SendMessageAsync(e.ToString());
                }
            });

            return Task.CompletedTask;
        }

        /*public static async Task ReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel) { }*/


        public static Task UserBanned(SocketUser user, SocketGuild guild)
        {
            _ = Task.Run(() =>
            {
                Language lang = GetLanguage(guild.Id);

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithAuthor(lang.GetString("event_user_banned"))
                    .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

                LogMessage(embed, guild.Id);
            });

            return Task.CompletedTask;
        }

        public static Task UserJoined(SocketGuildUser user)
        {
            _ = Task.Run(() => {
                Language lang = GetLanguage(user.Guild.Id);

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor(lang.GetString("event_user_join"))
                    .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

                LogMessage(embed, user.Guild.Id);

                GuildConfiguration guild = GuildSettings.GetGuild(user.Guild.Id);

                if (guild.EnableWelcome && !string.IsNullOrWhiteSpace(guild.WelcomeMessage) && user.Guild.Channels.ToList().Any(ch => ch.Id == guild.WelcomeChannel))
                {
                    YukiContextMessage msg = new YukiContextMessage(user, user.Guild);

                    user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(StringReplacements.GetReplacement(null, guild.WelcomeMessage, msg));
                }

                if (guild.EnableWarnings)
                {
                    GuildWarnedUser warnedUser = GuildSettings.GetWarnedUser(user.Id, user.Guild.Id);

                    if (!warnedUser.Equals(default))
                    {
                        List<GuildWarningAction> warnings = GuildSettings.GetGuild(user.Guild.Id).WarningActions;

                        for (int i = 0; i < warnedUser.Warning; ++i)
                        {
                            if (warnings.Count <= (i - 1) && warnings[i].WarningAction == WarningAction.GiveRole)
                            {
                                user.AddRoleAsync(user.Guild.GetRole(warnings[i].RoleId));
                            }
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }

        public static Task UserLeft(SocketGuildUser user)
        {
            _ = Task.Run(() => {
                Language lang = GetLanguage(user.Guild.Id);

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithAuthor(lang.GetString("event_user_leave"))
                    .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

                LogMessage(embed, user.Guild.Id);

                GuildConfiguration guild = GuildSettings.GetGuild(user.Guild.Id);

                if (guild.EnableGoodbye && !string.IsNullOrWhiteSpace(guild.GoodbyeMessage) && user.Guild.Channels.ToList().Any(ch => ch.Id == guild.WelcomeChannel))
                {
                    YukiContextMessage msg = new YukiContextMessage(user, user.Guild);

                    user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(StringReplacements.GetReplacement(null, guild.GoodbyeMessage, msg));
                }
            });

            return Task.CompletedTask;
        }

        public static Task UserUnbanned(SocketUser user, SocketGuild guild)
        {
            _ = Task.Run(() =>
            {
                Language lang = GetLanguage(guild.Id);

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(Color.Orange)
                    .WithAuthor(lang.GetString("event_user_unban"))
                    .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

                LogMessage(embed, guild.Id);
            });

            return Task.CompletedTask;
        }

        public static Task LogMessage(EmbedBuilder embed, ulong guildId)
        {
            _ = Task.Run(() => {
                embed.WithFooter(DateTime.UtcNow.ToPrettyTime(false, true));

                GuildConfiguration config = GuildSettings.GetGuild(guildId);

                if (!config.Equals(null) && config.EnableLogging && config.LogChannel != 0 && YukiBot.Discord.Client.GetGuild(guildId).TextChannels.Any(c => c.Id == config.LogChannel))
                {
                    YukiBot.Discord.Client.GetGuild(guildId).GetTextChannel(config.LogChannel).SendMessageAsync("", false, embed.Build());
                }
            });

            return Task.CompletedTask;
        }
    }
}
