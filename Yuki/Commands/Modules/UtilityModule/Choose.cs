using Qmmands;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("choose")]
        public async Task ChooseItemAsync([Remainder] string args)
        {
            string[] items = Regex.Split(args, @"\s*[|]\s*");

            await ReplyAsync(items[new YukiRandom().Next(items.Length)]);
        }
    }
}
