using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("langs", "languages")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ListLanguagesAsync(int page = 0)
        {
            PageManager manager = new PageManager(Localization.Languages.Where(lang => lang.Key != "none").Select(lang => lang.Key).ToArray(), "languages");

            await ReplyAsync(manager.GetPage(page));
        }
    }
}
