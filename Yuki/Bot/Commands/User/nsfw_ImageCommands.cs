using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot.API;
using Yuki.Bot.API.Danbooru;
using Yuki.Bot.API.E621;
using Yuki.Bot.API.Gelbooru;
using Yuki.Bot.API.Rule34;
using Yuki.Bot.Common;
using Yuki.Bot.Misc;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.Modules.User
{
    public class nsfw_ImageCommands : ModuleBase
    {
        /* Search all the available nsfw sites */
        [RequireNsfw]
        [Command("nsfw")]
        public async Task NSFWAsync([Remainder] string term = "")
        {
            Embed embed = Embeds.EmbedWithSource(YukiImage.GetBatchImages(term, true), Context.Message, term);

            if (embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
        }

        /* H-Hentai..!~ */
        [RequireNsfw]
        [Command("hentai")]
        public async Task HentaiAsync([Remainder] string term = "")
        {
            Embed embed = Embeds.EmbedWithSource(YukiImage.GetBatchAnimeImages(term, true), Context.Message, term);

            if (embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
        }
        
        /* Search e621 */
        [RequireNsfw]
        [Command("e621")]
        public async Task E621Async([Remainder] string term = "")
        {
            Embed embed = Embeds.EmbedWithSource(await E621.GetImages(term), Context.Message, term);

            if (embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
        }

        /* Search rule34.xxx */
        [RequireNsfw]
        [Command("r34")]
        public async Task R34Async([Remainder] string term = "")
        {
            Embed embed = Embeds.EmbedWithSource(await Rule34.GetImages(term), Context.Message, term);

            if (embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
        }

        /* Search gelbooru */
        [RequireNsfw]
        [Command("gelbooru")]
        public async Task GelbooruAsync([Remainder] string term = "")
        {
            Embed embed = Embeds.EmbedWithSource(await Gelbooru.GetImages(term, true), Context.Message, term);

            if (embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
        }

        /* Search danbooru */
        [RequireNsfw]
        [Command("danbooru")]
        public async Task DanbooruAsync([Remainder] string term = "")
        {
            if(term.Split(' ').Length > 2 || term.Split('+').Length > 2)
                await ReplyAsync("Cannot have more than 2 tags");
            else
            {
                Embed embed = Embeds.EmbedWithSource(await Danbooru.GetImages(term, true), Context.Message, term);

                if (embed != null)
                    await ReplyAsync("", false, embed);
                else
                    await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
            }
        }

        /* Get a random butt image */
        [RequireNsfw]
        [Command("butts")]
        public async Task ButtAsync()
        {
            JToken obj;
            using (HttpClient http = new HttpClient())
                obj = JArray.Parse(await http.GetStringAsync("http://api.obutts.ru/butts/" + new YukiRandom().Next(6013)).ConfigureAwait(false))[0];

            if (obj != null)
                await ReplyAsync("", false, Embeds.ImageEmbed("http://media.obutts.ru/" + obj["preview"], Context.Message, null,
                                 "[Source](" + "http://media.obutts.ru/" + obj["preview"] + ")", "obutts"));
        }

        /* Get a random image of boobs */
        [RequireNsfw]
        [Command("boobs")]
        public async Task BoobAsync()
        {
            JToken obj;
            using (HttpClient http = new HttpClient())
                obj = JArray.Parse(await http.GetStringAsync("http://api.oboobs.ru/boobs/" + new YukiRandom().Next(6013)).ConfigureAwait(false))[0];

            if (obj != null)
                await ReplyAsync("", false, Embeds.ImageEmbed("http://media.oboobs.ru/" + obj["preview"], Context.Message, null,
                                 "[Source](" + "http://media.oboobs.ru/" + obj["preview"] + ")", "obutts"));
        }
    }
}
