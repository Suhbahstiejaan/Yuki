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
            else if(commandStr == "list")
            {
                IEnumerable<Module> modules = YukiBot.Discord.CommandService.GetAllModules().Where(mod => mod.Parent == null);

                EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("modules_title")).WithFooter(Language.GetString("modules_help"));

                foreach (Module module in modules)
                {
                    int commandCount = module.Commands.Count + module.Submodules.Count;

                    embed.AddField(module.Name, Language.GetString("modules_count").Replace("%cmds%", commandCount.ToString()), true);
                }

                await ReplyAsync(embed);
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
                        List<Command> cmds = YukiBot.Discord.CommandService.GetAllModules()
                                        .FirstOrDefault(mod => mod.Name.ToLower() == commandStr.ToLower()).Commands.ToList();

                        if(cmds != null && cmds.Count > 0)
                        {
                            EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("commands_title")).WithFooter(Language.GetString("commands_help"));

                            foreach (Command command in cmds)
                            {
                                string embedValue = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));

                                embed.AddField(command.Name, (!string.IsNullOrWhiteSpace(embedValue) ? embedValue : Language.GetString("commands_no_alias")), true);
                            }

                            List<string> subModules = YukiBot.Discord.CommandService.GetAllModules()
                                                             .FirstOrDefault(mod => mod.Name.ToLower() == commandStr.ToLower()).Submodules.Select(mod => mod.Name).ToList();

                            foreach (string mod in subModules)
                            {
                                embed.AddField(mod, "Submodule", true);
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
}
