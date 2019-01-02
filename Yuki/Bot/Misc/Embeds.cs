using Discord;
using System.Collections.Generic;
using Yuki.Bot.API;
using Yuki.Bot.Misc.Extensions;

namespace Yuki.Bot.Misc
{
    public class Embeds
    {
        public static Embed ImageEmbed(string url, IUserMessage message, string title = null, string description = null, string footer = null)
        {
            EmbedBuilder embed = new EmbedBuilder().WithImageUrl(url)
                                                   .WithAuthor(new EmbedAuthorBuilder() { IconUrl = message.Author.GetAvatarUrl() })
                                                   .WithColor(Colors.pink);

            if (title != null)
                embed.Author.Name = ((IGuildChannel)message.Channel).Guild.SanitizeMentions(title);
            else
                embed.Author.Name = message.Author.Username + "#" + message.Author.Discriminator;

            if (description != null)
                embed.Description = description;

            if (footer != null)
                embed.WithFooter(new EmbedFooterBuilder() { Text = footer });

            return embed.Build();
        }

        public static Embed EmbedWithSource(List<YukiImage> images, IUserMessage message, string terms, string footer = null)
        {
            if (images != null && images.Count > 0)
            {
                YukiRandom random = new YukiRandom();

                YukiImage image = images[random.Next(images.Count)];

                string rating = (image.Rating > 0 ? "Rating: " + image.Rating + " | " : "");
                string resolution = (image.Width > 0 && image.Height > 0 ? "Resolution: " + image.Width + "x" + image.Height + (footer != null ? " | " : "") : "");
                string source = (StringHelper.IsUrl(image.Source) ? "[Image Source](" + image.Source + ")" : image.Source);

                return ImageEmbed(image.Url, message, terms, source, rating + resolution + footer);
            }
            else
                return null;
        }
    }
}
