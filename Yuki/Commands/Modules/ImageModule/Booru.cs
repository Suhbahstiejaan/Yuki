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
        [Command("booru")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task BooruAsync(params string[] tags)
        {
            bool isExplicit = false;

            YukiImage image = await ImageSearch.GetImage(ImageType.Gelbooru, tags, null, forceExplicit: isExplicit);

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
