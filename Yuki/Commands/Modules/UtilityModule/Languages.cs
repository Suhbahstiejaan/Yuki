using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("langs", "languages")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ListLanguagesAsync()
        {
            await ReplyAsync(Context.CreateEmbedBuilder(Language.GetString("langs_title"))
                    .WithDescription(string.Join("\n", Localization.Languages.Where(lang => lang.Key != "none").Select(lang => lang.Key))));
        }
    }
}
