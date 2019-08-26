using Discord;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("help")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task HelpAsync(string commandStr = "")
        {
            EmbedBuilder helpEmbed = Context.CreateEmbedBuilder(Language.GetString("help_title"))
                       .WithDescription(Language.GetString("help_info_description")
                           .Replace("%botinvite%", YukiBot.BotInvUrl)
                           .Replace("%serverinvite%", YukiBot.ServerUrl)
                           .Replace("%github%", YukiBot.GithubUrl)
                           .Replace("%wiki%", YukiBot.WikiUrl));

            if (commandStr == "")
            {
                await ReplyAsync(helpEmbed);
            }
            else
            {
                /* TODO: list similar commands? */
                List<Command> commands = YukiBot.Discord.CommandService.GetAllCommands()
                    .Where(cmd => ((cmd.Module.Parent != null) ? $"{cmd.Module.Name.ToLower()}_{cmd.Name.ToLower()}" : cmd.Name.ToLower()) == commandStr.ToLower()).ToList();

                if (commands.Count() == 1)
                {
                    string name = "";

                    if (commands[0].Module.Parent != null)
                    {
                        name += commands[0].Module.Name + " ";
                    }

                    name += commands[0].Name;

                    string aliases = string.Join(", ", commands[0].Aliases.Where(alias => alias != commands[0].Name));

                    EmbedBuilder embed = Context.CreateEmbedBuilder(name)
                        .AddField(Language.GetString("help_aliases"), (string.IsNullOrWhiteSpace(aliases) ? Language.GetString("commands_no_alias") : aliases))
                        .AddField(Language.GetString("help_description"), Language.GetString("command_" + name.ToLower() + "_desc"))
                        .AddField(Language.GetString("help_usage"), Language.GetString("command_" + name.ToLower() + "_usage"));

                    await ReplyAsync(embed);
                }
                else
                {
                    List<Command> subCommands = YukiBot.Discord.CommandService.GetAllModules()
                                                            .Where(mod => mod.Parent != null && mod.Name.ToLower() == commandStr.ToLower())
                                                            .SelectMany(mod => mod.Commands).ToList();

                    if (subCommands != null && subCommands.Count > 0)
                    {
                        EmbedBuilder embed = Context.CreateEmbedBuilder(subCommands[0].Module.Name);

                        foreach (Command subCommand in subCommands)
                        {
                            string description = Language.GetString("command_" + subCommand.Name.ToLower().Replace(' ', '_') + "_desc") + "\n\n" +
                                                 Language.GetString("command_" + subCommand.Name.ToLower().Replace(' ', '_') + "_usage") + "\n";

                            embed.AddField(subCommand.Name, description);
                        }

                        await ReplyAsync(embed);
                    }
                    else
                    {
                        await ReplyAsync(helpEmbed);
                    }
                }
            }
        }
    }
}
