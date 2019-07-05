using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("gelbooru")]
        public async Task GelbooruAsync(string[] tags = null)
        {
            YukiImage image = await new ImageSearch().GetImage(ImageType.Gelbooru, tags, null, forceExplicit: false);

            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                {
                    Name = string.Join(", ", image.tags.Take(5))
                })
                .WithImageUrl(image.url)
                .WithDescription($"[Source]({image.source}) | [Page]({image.page})")
                .WithFooter($"{image.type.ToString()} | {(image.isExplicit ? "Explicit" : "Safe")}");


            await ReplyAsync(embed);
        }
    }
}
