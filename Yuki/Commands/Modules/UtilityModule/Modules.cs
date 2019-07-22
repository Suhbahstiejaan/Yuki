using Discord;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("modules")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetModulesAsync()
        {
            IEnumerable<Module> modules = YukiBot.Services.GetRequiredService<YukiBot>().CommandService.GetAllModules().Where(mod => mod.Parent == null);

            EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("modules_title")).WithFooter(Language.GetString("modules_help"));

            foreach (Module module in modules)
            {
                embed.AddField(module.Name, Language.GetString("modules_count").Replace("%cmds%", module.Commands.Count.ToString()), true);
            }

            await ReplyAsync(embed);
        }
    }
}
