using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("langs", "languages")]
        public async Task ListLanguagesAsync()
        {
            await ReplyAsync(Context.CreateEmbedBuilder(Language.GetString("langs_title"))
                    .WithDescription(string.Join("\n", LocalizationService.Languages.Where(lang => lang.Key != "none").Select(lang => lang.Key)));
        }
    }
}
