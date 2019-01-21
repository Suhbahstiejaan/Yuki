using Discord.Commands;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Common;

namespace Yuki.Bot.Modules.User
{
    public class fun_Commands : ModuleBase
    {
        private YukiRandom random = new YukiRandom();
        
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
    }
}
