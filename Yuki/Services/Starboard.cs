using Discord;
using Discord.WebSocket;
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
        public static async void Manage(IUserMessage message, ITextChannel channel, SocketReaction reaction, bool isDeleteCheck = false)
        {
            if(channel is IDMChannel)
            {
                return;
            }


            IGuild guild = (channel as IGuildChannel).Guild;

            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);

            Language lang = Localization.GetLanguage(config.LangCode);

            /* Starboard */
            if (!config.Equals(null) && config.EnableStarboard && !(message.Author.IsBot) || message.Author.Id == reaction.UserId)
            {
                int starCount = message.Reactions.Keys.Select(r => r.Name == "⭐") != null ? message.Reactions.Select(r => r.Key.Name == "⭐").Count() : 0;

                if (starCount >= config.StarRequirement)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(lang.GetString("starboard_title"))
                        .WithDescription(message.Content)
                        .AddField(lang.GetString("starboard_field_author"), message.Author.Mention, true)
                        .AddField(lang.GetString("starboard_field_channel"), ((ITextChannel)message.Channel).Mention, true)
                        .WithFooter($"⭐ {starCount} {lang.GetString("starboard_stars")} ({message.Id})").WithCurrentTimestamp()
                        .WithColor(Color.Gold);

                    if (message.Attachments != null && message.Attachments.Count > 0)
                    {
                        string attachments = string.Empty;
                        string imageUrl = null;

                        IAttachment[] _attachments = message.Attachments.ToArray();

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

                    bool starUpdated = false;
                    foreach (IMessage imessage in (await AsyncEnumerable.ToList((await guild.GetTextChannelAsync(config.StarboardChannel)).GetMessagesAsync(100))).SelectMany(mlist => mlist))
                    {
                        if (imessage.Author.Id == YukiBot.Discord.Client.CurrentUser.Id && imessage.Embeds != null && imessage.Embeds.Count > 0)
                        {
                            IEmbed messageEmbed = imessage.Embeds.ToArray()[0];

                            if (messageEmbed.Footer.HasValue)
                            {
                                if (messageEmbed.Footer.Value.Text.StartsWith('⭐') && messageEmbed.Footer.Value.Text.Contains(message.Id.ToString()))
                                {
                                    await((IUserMessage)imessage).ModifyAsync(a =>
                                    {
                                        a.Embed = embed.Build();
                                    });

                                    starUpdated = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!starUpdated)
                    {
                        await(await guild.GetTextChannelAsync(config.StarboardChannel)).SendMessageAsync("", false, embed.Build());
                    }
                }

                if(isDeleteCheck && starCount == 0)
                {
                    await message.DeleteAsync();
                }
            }
        }
    }
}
