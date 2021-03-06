﻿using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects;

namespace Yuki.Commands.Modules.NsfwModule
{
    public partial class NsfwModule
    {
        [Command("e621")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task E621Async(params string[] tags)
        {
            YukiImage image = await ImageSearch.GetImage(ImageType.E621, tags, null, true);

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
