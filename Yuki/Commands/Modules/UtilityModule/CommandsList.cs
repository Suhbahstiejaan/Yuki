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
        public async Task ShowCommandsAsync(string moduleName)
        {
            IReadOnlyList<Command> commands = YukiBot.Services.GetRequiredService<YukiBot>().CommandService.GetAllModules()
                                        .FirstOrDefault(mod => mod.Name.ToLower() == moduleName.ToLower()).Commands;

            EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("commands_title"), false).WithFooter(Language.GetString("commands_help"));

            foreach(Command command in commands)
            {
                string embedValue = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));



                embed.AddField(command.Name, (!string.IsNullOrWhiteSpace(embedValue) ? embedValue : Language.GetString("commands_no_alias")), true);
            }

            await ReplyAsync(embed);
        }
    }
}
