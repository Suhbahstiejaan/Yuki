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
        [Command("danbooru")]
        public async Task DanbooruAsync(string[] tags = null)
        {
            YukiImage image = await new ImageSearch().GetImage(ImageType.Danbooru, tags, null, forceExplicit: false);

            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                {
                    Name = string.Join(", ", image.tags.Take(5))
                })
                .WithImageUrl(image.url)
                .WithDescription($"[{Language.GetString("source")}]({image.source}) | [{Language.GetString("page")}]({image.page})")
                .WithFooter($"{image.type.ToString()} | {(image.isExplicit ? Language.GetString("_explicit") : Language.GetString("safe"))}");


            await ReplyAsync(embed);
        }
    }
}
