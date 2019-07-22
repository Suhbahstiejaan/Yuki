using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
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
            return LocalizationService.GetLanguage(GuildSettings.GetGuild(guildId).LangCode);
        }

        public static async Task JoinedGuild(SocketGuild guild) { }
        public static async Task LeftGuild(SocketGuild guild) { }

        public static async Task ChannelCreated(SocketChannel channel)
        {
            Language lang = GetLanguage(channel);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor(lang.GetString("event_channel_create"))
                .AddField(lang.GetString("event_channel_name"), (channel as IGuildChannel).Name);

            await LogMessageAsync(embed, (channel as IGuildChannel).GuildId);
        }

        public static async Task ChannelDestroyed(SocketChannel channel)
        {
            Language lang = GetLanguage(channel);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_channel_delete"))
                .AddField(lang.GetString("event_channel_name"), (channel as IGuildChannel).Name);

            await LogMessageAsync(embed, (channel as IGuildChannel).GuildId);
        }

        public static async Task ChannelUpdated(SocketChannel channelOld, SocketChannel channel)
        {
            Language lang = GetLanguage(channel);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor(lang.GetString("event_channel_update"))
                .AddField(lang.GetString("event_channel_name"), (channel as IGuildChannel).Name);

            await LogMessageAsync(embed, (channel as IGuildChannel).GuildId);
        }

        public static async Task GuildMemberUpdated(SocketGuildUser userOld, SocketGuildUser user)
        {
            Language lang = GetLanguage(user.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor(lang.GetString("event_member_update"))
                .AddField(lang.GetString("event_member_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessageAsync(embed, user.Guild.Id);
        }

        public static async Task GuildUpdated(SocketGuild guildOld, SocketGuild guild)
        {
            Language lang = GetLanguage(guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor(lang.GetString("event_guild_update"))
                .AddField(lang.GetString("event_guild_name"), guild.Name);

            await LogMessageAsync(embed, guild.Id);
        }
        
        public static Task MessageUpdated(Cacheable<IMessage, ulong> messageOld, SocketMessage current, ISocketMessageChannel channel)
        {
            Messages.InsertOrUpdate(new YukiMessage()
            {
                Id = current.Id,
                AuthorId = current.Author.Id,
                ChannelId = current.Channel.Id,
                SendDate = DateTime.UtcNow,
                Content = (current as SocketUserMessage).Resolve(TagHandling.FullName, TagHandling.NameNoPrefix, TagHandling.Name, TagHandling.Name, TagHandling.FullNameNoPrefix)
            });

            return Task.CompletedTask;
        }

        public static async Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            Messages.Remove(message.Value.Id);

            Language lang = GetLanguage(channel);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_message_deleted"))
                .AddField(lang.GetString("message_author"), $"{message.Value.Author.Username}#{message.Value.Author.Discriminator} ({message.Value.Author.Id})");

            if (message.Value.Content != null)
            { 
                embed.AddField(lang.GetString("event_message_text"), message.Value.Content ?? lang.GetString("none"));
            }

            if(message.Value.Attachments.Count > 0)
            {
                embed.AddField(lang.GetString("event_message_attachments"), string.Join("\n", message.Value.Attachments.Select(a => $"[{a.Filename}]({a.Url})")));
            }

            await LogMessageAsync(embed, ((IGuildChannel)channel).GuildId);
        }


        public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction) { }
        public static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction) { }
        public static async Task ReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel) { }


        public static async Task RoleCreated(SocketRole role)
        {
            Language lang = GetLanguage(role.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor(lang.GetString("event_role_created"))
                .AddField(lang.GetString("event_role_name"), role.Name);

            await LogMessageAsync(embed, role.Guild.Id);
        }

        public static async Task RoleDeleted(SocketRole role)
        {
            Language lang = GetLanguage(role.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_role_deleted"))
                .AddField(lang.GetString("event_role_name"), role.Name);

            await LogMessageAsync(embed, role.Guild.Id);
        }

        public static async Task RoleUpdated(SocketRole roleOld, SocketRole role)
        {
            Language lang = GetLanguage(role.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor(lang.GetString("event_role_updated"))
                .AddField(lang.GetString("event_role_name"), role.Name);

            await LogMessageAsync(embed, role.Guild.Id);
        }


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
                await user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(
                    guild.WelcomeMessage.Replace("%muser%", user.Mention).Replace("%user%", $"{user.Username}#{user.Discriminator}"));
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
                await user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(
                    guild.GoodbyeMessage.Replace("%muser%", user.Mention).Replace("%user%", $"{user.Username}#{user.Discriminator}"));
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

        public static async Task VoiceServerUpdated(SocketVoiceServer voiceServer) { }



        public static async Task MessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;

            DiscordSocketClient shard = (message.Channel is IGuildChannel) ?
                                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetShardFor(((IGuildChannel)message.Channel).Guild) :
                                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetShard(0);


            bool hasPrefix = HasPrefix(message, out string output);

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

            IResult result = await YukiBot.Services.GetRequiredService<YukiBot>().CommandService
                       .ExecuteAsync(output, new YukiCommandContext(
                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient, socketMessage as IUserMessage, YukiBot.Services));

            if (result is FailedResult failedResult)
            {
                if (!(failedResult is CommandNotFoundResult) && failedResult is ChecksFailedResult checksFailed)
                {
                    if(checksFailed.FailedChecks.Count == 1)
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
        }

        private static bool HasPrefix(SocketUserMessage message, out string output)
        {
            output = string.Empty;

            return CommandUtilities.HasAnyPrefix(message.Content, Config.GetConfig().prefix.AsReadOnly(), out string prefix, out output);
        }

        private static async Task LogMessageAsync(EmbedBuilder embed, ulong guildId)
        {
            /*embed.WithFooter(GetLanguage(guildId).GetString("log_event_fired_at").Replace("%time%", DateTime.UtcNow.ToPrettyTime(false, true)));

            GuildConfiguration config = GuildSettings.GetGuild(guildId);

            if(!config.Equals(null) && config.EnableLogging)
            {
                await YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetGuild(guildId)
                    .GetTextChannel(config.LogChannel).SendMessageAsync("", false, embed.Build());
            }*/
        }
    }
}
