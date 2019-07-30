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
        [Command("commands")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ShowCommandsAsync(string moduleName)
        {
            List<Command> commands = YukiBot.Discord.CommandService.GetAllModules()
                                        .FirstOrDefault(mod => mod.Name.ToLower() == moduleName.ToLower()).Commands.ToList();

            EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("commands_title")).WithFooter(Language.GetString("commands_help"));

            foreach(Command command in commands)
            {
                string embedValue = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));
                
                embed.AddField(command.Name, (!string.IsNullOrWhiteSpace(embedValue) ? embedValue : Language.GetString("commands_no_alias")), true);
            }

            List<string> subModules = YukiBot.Discord.CommandService.GetAllModules()
                                             .FirstOrDefault(mod => mod.Name.ToLower() == moduleName.ToLower()).Submodules.Select(mod => mod.Name).ToList();

            foreach (string mod in subModules)
            {
                embed.AddField(mod, "Submodule", true);
            }

            await ReplyAsync(embed);
        }
    }
}
