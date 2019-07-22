using Qmmands;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("choose")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ChooseItemAsync([Remainder] string args)
        {
            string[] items = Regex.Split(args, @"\s*[|]\s*");

            await ReplyAsync(items[new YukiRandom().Next(items.Length)]);
        }
    }
}
