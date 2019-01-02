using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Services;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Misc.Extensions;

namespace Yuki.Bot.Modules.User
{
    public class fun_Commands : ModuleBase
    {
        private YukiRandom random = new YukiRandom();
        private RussianRoulette roulette = new RussianRoulette();

        [Command("scramblr")]
        public async Task ScrambleMessage([Remainder] string user2 = null)
        {
            Scramblr scramblr = new Scramblr();

            using (UnitOfWork uow = new UnitOfWork())
            {
                DataOptIn existing = uow.DataOptInRepository.GetUser(Context.User.Id);

                if (user2 != null)
                {
                    if (user2.ToLower() == "agree")
                    {
                        if (existing != null)
                            existing.optedIn = true;
                        else
                            uow.DataOptInRepository.Add(new DataOptIn() { optedIn = true, UserId = Context.User.Id });
                        uow.Save();
                        await ReplyAsync("Thank you for agreeing!\n\nI'll start gathering a little data starting now. Check back with me after a few messages to see if there's anything cool I can do!\n\nIf you ever wish to opt out, run \"y!scramblr optout\"");
                        return;
                    }
                    else if (user2.ToLower() == "optout")
                    {
                        if (existing != null)
                            existing.optedIn = false;
                        else
                            uow.DataOptInRepository.Add(new DataOptIn() { optedIn = false, UserId = Context.User.Id });
                        uow.Save();
                        await ReplyAsync("You've opted out of data collection. We'll clear up any saved data now!\n\nIf you ever wish to opt back in, run \"y!scramblr accept\"");

                        if (MessageCache.HasUser(Context.User.Id))
                            MessageCache.DeleteUser(Context.User.Id);
                        return;
                    }
                }

                if (existing == null || !existing.optedIn)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor("Yuki Data Collection Agreement")
                    .WithColor(Colors.pink)
                    .WithDescription("Hello!\n\n"
                        + "Because of how this command works, and in compliance with Discord's Developer Terms of Service section 2.4, we need your permission for us to store some data from you. By typing \"y!scramblr agree\", you agree:\n\n"
                        + "1. To allow us to store the message content from your most recent " + MessageCache.maxMessagesPerUser + " messages\n"
                        + "2. To allow us to store message IDs and the ID of the channel a message was sent in\n"
                        + "3. To allow us to store your user ID\n\n"
                        + "Don't worry, though! Message content is used **only** by scramblr. Your data will be encrypted and ***cannot*** be accessed by us. As you delete or edit messages, your messages will be deleted or edited accordingly. In order to save space, if we detect no activity from you in seven days, your data will automatically be cleared.");

                    await ReplyAsync("", false, embed.Build());
                }
                else
                {
                    IGuildUser _user2 = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(user2));
                    await ReplyAsync(scramblr.GetMessage(((IGuildUser)(SocketUser)Context.User), _user2));
                }
            }
        }

        [Command("8ball")]
        public async Task _8BallAsync([Remainder] string question = null)
        {
            if (!string.IsNullOrEmpty(question))
                await ReplyAsync(random.EightBall(Localizer.YukiStrings.default_lang));
            else
                await ReplyAsync(random.MessageEmpty(Localizer.YukiStrings.default_lang));
        }

        [Command("roll")]
        public async Task RollAsync(int maxAmt = 6)
        {
            if(maxAmt >= 3)
                await ReplyAsync("You rolled a **" + (random.Next(maxAmt) + 1) + "**");
            else
                await ReplyAsync("Invalid number!");
        }

        [Command("flip")]
        public async Task FlipAsync()
        {
            int coin = random.Next(100) + 1;

            string flipped = (coin <= 50) ? "**heads**" : "**tails**";

            await ReplyAsync("You flip a coin. It lands on " + flipped);
        }

        [Command("pick")]
        public async Task PickAsync([Remainder] string optionsStr)
        {
            /* Split their message up at the |'s */
            string[] options = Context.Guild.SanitizeMentions(Regex.Split(optionsStr, @"\s*[|]\s*", RegexOptions.Singleline));
            
            /* Choose a random string, send it to the channel */
            await ReplyAsync("Hmmm..... I choose **" + options[random.Next(options.Length)] + "**!");
        }

        [Command("roulette")]
        public async Task RRouletteAsync([Remainder] string option = "")
        {
            try
            {
                switch (option.ToLower())
                {
                    case "join":
                        await ReplyAsync(roulette.Add(Context.Guild.Id, Context.User.Id));
                        break;
                    case "leave":
                        await ReplyAsync(roulette.Leave(Context.Guild.Id, Context.User.Id));
                        break;
                    case "start":
                        await ReplyAsync(roulette.Start(Context.Guild.Id, Context.User.Id));
                        break;
                    case "players":
                        string[] split = option.Split(' ');
                        int pageNum = 1;

                        if (split.Length > 1)
                            pageNum = int.Parse(split[1]);

                        await ReplyAsync(roulette.GetPlayers(Context.Guild.Id, pageNum));
                        break;
                    default:
                        await ReplyAsync(roulette.Play(Context.Guild.Id, Context.User.Id));
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
    }
}
