using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects;

namespace Yuki.Commands.Modules.NsfwModule
{
    public partial class NsfwModule
    {
        [Command("hentai")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task HentaiAsync(params string[] tags)
        {
            YukiImage image = await ImageSearch.GetAnimeImage(tags, null, forceExplicit: true);

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
