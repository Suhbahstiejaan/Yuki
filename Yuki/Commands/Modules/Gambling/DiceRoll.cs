using Qmmands;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.Gambling
{
    public partial class GamlingModule
    {
        [Command("roll")]
        public async Task DiceRollAsync([Remainder] string text = "")
        {
            int dice1 = new YukiRandom().Next(1, 6);
            int dice2 = new YukiRandom().Next(1, 6);

            await ReplyAsync(Language.GetString("roll_rolled").Replace("%dice%", (dice1 + dice2).ToString()));
        }
    }
}
