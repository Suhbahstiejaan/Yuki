﻿using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public class Starboard
    {
        public const string Emote = "⭐";

        public static async void Manage(IUserMessage message, ITextChannel channel, SocketReaction reaction, bool isDeleteCheck = false)
        {
            IGuild guild = (channel as IGuildChannel).Guild;

            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);

            IMessage msg = default;

            if (!config.Equals(null) && config.EnableStarboard && !(reaction.User.Value.IsBot || message.Author.Id == reaction.UserId) &&
                config.StarboardIgnoredChannels != null && !config.StarboardIgnoredChannels.Contains(message.Channel.Id) &&
                DateTime.Now.TimeOfDay.TotalDays - message.Timestamp.TimeOfDay.TotalDays < 7) // make sure the message wasn't sent more than a week ago
            {

                int starCount = message.Reactions.Keys.Select(r => r.Name == Emote) != null ? message.Reactions.FirstOrDefault(r => r.Key.Name == Emote).Value.ReactionCount : 0;

                bool starUpdated = false;

                foreach (IMessage imessage in (await AsyncEnumerable.ToListAsync((await guild.GetTextChannelAsync(config.StarboardChannel)).GetMessagesAsync(100))).SelectMany(mlist => mlist))
                {
                    if (imessage.Author.Id == YukiBot.Discord.Client.CurrentUser.Id && (imessage.Timestamp.Date - DateTime.UtcNow).TotalDays <= 14   && imessage.Embeds != null && imessage.Embeds.Count > 0)
                    {
                        IEmbed messageEmbed = imessage.Embeds.ToArray()[0];

                        if (messageEmbed.Footer.HasValue)
                        {
                            if (messageEmbed.Footer.Value.Text.StartsWith(Emote) && messageEmbed.Footer.Value.Text.Contains(message.Id.ToString()))
                            {
                                msg = imessage;

                                starUpdated = true;
                                break;
                            }
                        }
                    }
                }

                if (msg != default)
                {
                    await ((IUserMessage)msg).ModifyAsync(a =>
                    {
                        a.Embed = CreateEmbed(message, config, starCount).Build();
                    });
                }

                if (!starUpdated && starCount >= config.StarRequirement)
                {
                    await (await guild.GetTextChannelAsync(config.StarboardChannel)).SendMessageAsync("", false, CreateEmbed(message, config, starCount).Build());
                }

                if (isDeleteCheck && starCount < 1 && msg != default)
                {
                    await msg.DeleteAsync();
                }
            }
        }

        private static EmbedBuilder CreateEmbed(IUserMessage message, GuildConfiguration config, int starCount)
        {
            Language lang = Localization.GetLanguage(config.LangCode);

            string content = message.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                content = lang.GetString("starboard_jump_to");
            }

            EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor(lang.GetString("starboard_title"))
                    .WithDescription($"[{content}]({message.GetJumpUrl()})")
                    .AddField(lang.GetString("starboard_field_author"), message.Author.Mention, true)
                    .AddField(lang.GetString("starboard_field_channel"), ((ITextChannel)message.Channel).Mention, true)
                    .WithFooter($"{Emote} {starCount} {lang.GetString("starboard_stars")} ({message.Id})").WithCurrentTimestamp()
                    .WithColor(Color.Gold);

            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                string attachments = string.Empty;
                string imageUrl = null;

                IAttachment[] _attachments = message.Attachments.ToArray();

                foreach (string str in message.Content.Split(' '))
                {
                    if (str.IsMedia())
                    {
                        if (imageUrl == null)
                        {
                            imageUrl = str;
                        }

                        attachments += $"[{Path.GetFileName(message.Content)}]({str})\n";
                    }
                }

                if (imageUrl == null)
                {
                    imageUrl = _attachments.FirstOrDefault(img => img.ProxyUrl.IsMedia())?.ProxyUrl;
                }

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

            return embed;
        }
    }
}