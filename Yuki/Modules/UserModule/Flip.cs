using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data;
using Yuki.Services.Localization;

namespace Yuki.Modules.UserModule
{
    public partial class User
    {
        [Command("flip")]
        public async Task FlipAsync()
        {
            Language lang = YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage(Context);

            int num = new YukiRandom().Next(99) + 1;

            string face = num > 50
                        ? lang.GetString("coinflip_heads") :
                          lang.GetString("coinflip_tails");

            await ReplyAsync(lang.GetString("coinflip_flipped").Replace("<face>", face));
        }
    }
}
