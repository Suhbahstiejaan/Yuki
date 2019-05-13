using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.API;
using Yuki.Bot.API.Danbooru;
using Yuki.Bot.API.Gelbooru;
using Yuki.Bot.API.RamMoe;
using Yuki.Bot.Common;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.Modules.User
{
    public class user_ImageCommands : ModuleBase
    {
        private YukiRandom random = new YukiRandom();

        [Command("dog")]
        public async Task DogAsync()
            => await ReplyAsync("", false, Embeds.ImageEmbed(await Dog.GetImage(), Context.Message, null, null, "Woof woof"));

        [Command("cat")]
        public async Task CatAsync()
            => await ReplyAsync("", false, Embeds.ImageEmbed(Cat.GetImage(), Context.Message, null, null, "Nya~!"));

        [Command("hug")]
        public async Task HugAsync([Remainder] string user = null)
        {
            string text = "Hugs " + Context.User.Username + "";

            if (user != null)
                text = Context.User.Username + " hugs " + Context.Guild.SanitizeMentions(user);

            await ReplyAsync("", false, Embeds.ImageEmbed(await RamMoe.GetImage("hug"), Context.Message, text));
        }

        [Command("goodnight")]
        public async Task GoodNightAsync()
            => await ReplyAsync("", false, Embeds.ImageEmbed(random.GoodNight(Localizer.YukiStrings.default_lang), Context.Message, "Goodnight, " + Context.User.Username + "!"));

        [Command("lewd")]
        public async Task LewdAsync()
            => await ReplyAsync("", false, Embeds.ImageEmbed(await RamMoe.GetImage("lewd"), Context.Message));

        [Command("sad")]
        public async Task SadAsync()
            => await ReplyAsync("", false, Embeds.ImageEmbed(await RamMoe.GetImage("cry"), Context.Message));

        [Command("kiss")]
        public async Task KissAsync([Remainder] string user = null)
            => await RamMoe.SendImage("kiss", Context.User.Username, user, Context.Message);

        [Command("pat")]
        public async Task PatAsync([Remainder] string user = null)
            => await RamMoe.SendImage("pat", Context.User.Username, user, Context.Message);

        [Command("slap")]
        public async Task SlapAsync([Remainder] string user = null)
            => await RamMoe.SendImage("slap", Context.User.Username, user, Context.Message);

        [Command("tickle")]
        public async Task TickleAsync([Remainder] string user = null)
            => await RamMoe.SendImage("tickle", Context.User.Username, user, Context.Message);

        [Command("booru")]
        public async Task RedditSearchAsync([Remainder] string term = null)
        {
            Embed embed = Embeds.EmbedWithSource(YukiImage.GetBatchAnimeImages(Context.Channel, term), Context.Message, term);

            if(embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`", false, embed);
        }

        /* Search gelbooru */
        [Command("gelbooru")]
        public async Task GelbooruAsync([Remainder] string term = "")
        {
            bool isNsfw = false;

            using (UnitOfWork uow = new UnitOfWork())
            {
                if(Context.Channel is IDMChannel)
                {
                    isNsfw = false;
                }

                isNsfw = uow.NsfwChannelRepository.GetChannels(((IGuildChannel)Context.Channel).GuildId).FirstOrDefault() != null;
            }

            Embed embed = Embeds.EmbedWithSource(await Gelbooru.GetImages(Context.Channel, term, isNsfw), Context.Message, term);

            if (embed != null)
                await ReplyAsync("", false, embed);
            else
                await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
        }

        /* Search danbooru */
        [Command("danbooru")]
        public async Task DanbooruAsync([Remainder] string term = "")
        {
            bool isNsfw = false;

            using (UnitOfWork uow = new UnitOfWork())
            {
                if (Context.Channel is IDMChannel)
                {
                    isNsfw = false;
                }

                isNsfw = uow.NsfwChannelRepository.GetChannels(((IGuildChannel)Context.Channel).GuildId).FirstOrDefault() != null;
            }

            if (term.Split(' ').Length > 2 || term.Split('+').Length > 2)
                await ReplyAsync("Cannot have more than 2 tags");
            else
            {
                Embed embed = Embeds.EmbedWithSource(await Danbooru.GetImages(Context.Channel, term, isNsfw), Context.Message, term);

                if (embed != null)
                    await ReplyAsync("", false, embed);
                else
                    await ReplyAsync(Localizer.GetStrings(Localizer.YukiStrings.default_lang).no_results + ": `" + term + "`");
            }
        }
    }
}