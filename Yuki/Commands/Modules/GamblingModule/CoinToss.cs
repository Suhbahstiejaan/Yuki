using Qmmands;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.GamblingModule
{
    public partial class GamlingModule
    {
        [Command("toss")]
        public async Task CoinTossAsync([Remainder] string text = "")
        {
            string face = (new YukiRandom().Next(1, 100)) > 50 ?
                            Language.GetString("coin_heads") : Language.GetString("coin_tails");

            await ReplyAsync(Context.CreateEmbed(face, new Discord.EmbedAuthorBuilder()
            {
                IconUrl = Context.User.GetAvatarUrl(),
                Name = Language.GetString("coin_flipped").Replace("%user%", Context.User.Username)
            }));
        }
    }
}
