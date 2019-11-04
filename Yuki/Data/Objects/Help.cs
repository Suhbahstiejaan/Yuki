using Discord;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using Yuki.Commands;
using Yuki.Commands.Preconditions;
using Yuki.Services.Database;

namespace Yuki.Data.Objects
{
    public class Help
    {
        private EmbedBuilder helpEmbed;

        public async void Get(YukiCommandContext Context, string commandStr)
        {
            helpEmbed = Context.CreateEmbedBuilder(Context.Language.GetString("help_title"))
                       .WithDescription(Context.Language.GetString("help_info_description")
                           .Replace("%botinvite%", YukiBot.BotInvUrl)
                           .Replace("%serverinvite%", YukiBot.ServerUrl)
                           .Replace("%github%", YukiBot.GithubUrl)
                           .Replace("%wiki%", YukiBot.WikiUrl));

            if (commandStr == "")
            {
                await Context.ReplyAsync(helpEmbed);
            }
            else if (commandStr == "list")
            {
                await Context.ReplyAsync(BuildList(Context));
            }
            else
            {
                /* TODO: list similar commands? */
                List<Command> commands = YukiBot.Discord.CommandService.GetAllCommands()
                    .Where(cmd => ((cmd.Module.Parent != null) ? $"{cmd.Module.Name.ToLower()}_{cmd.Name.ToLower()}" : cmd.Name.ToLower()) == commandStr.ToLower()).ToList();

                if (commands.Count() == 1)
                {
                    EmbedBuilder embed = GetSingularCommandHelp(Context, commands[0]);

                    if (embed != null)
                    {
                        await Context.ReplyAsync(embed);
                    }
                }
                else
                {
                    EmbedBuilder embed = GetSubCommandsHelp(Context, commands, commandStr, out bool isNsfw);

                    if(embed != null)
                    {
                        await Context.ReplyAsync(embed);
                    }
                    else
                    {
                        if(!isNsfw)
                        {
                            await Context.ReplyAsync(helpEmbed);
                        }
                    }
                }
            }
        }

        private EmbedBuilder BuildList(YukiCommandContext Context)
        {
            IEnumerable<Module> modules = YukiBot.Discord.CommandService.GetAllModules().Where(mod => mod.Parent == null);

            EmbedBuilder embed = Context.CreateEmbedBuilder(Context.Language.GetString("modules_title")).WithFooter(Context.Language.GetString("modules_help"));

            foreach (Module module in modules)
            {
                int commandCount = module.Commands.Count + module.Submodules.Count;

                embed.AddField(module.Name, Context.Language.GetString("modules_count").Replace("%cmds%", commandCount.ToString()), true);
            }

            return embed;
        }

        private EmbedBuilder GetSingularCommandHelp(YukiCommandContext Context, Command command)
        {
            if (!command.Module.Checks.Any(check => check is RequireNsfwAttribute) || GuildSettings.IsChannelExplicit(Context.Channel.Id, Context.Guild.Id))
            {
                string name = "";

                if (command.Module.Parent != null)
                {
                    name += command.Module.Name + " ";
                }

                name += command.Name;

                string aliases = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));

                EmbedBuilder embed = Context.CreateEmbedBuilder(name)
                    .AddField(Context.Language.GetString("help_aliases"), (string.IsNullOrWhiteSpace(aliases) ? Context.Language.GetString("commands_no_alias") : aliases))
                    .AddField(Context.Language.GetString("help_description"), Context.Language.GetString("command_" + name.ToLower() + "_desc"))
                    .AddField(Context.Language.GetString("help_usage"), Context.Language.GetString("command_" + name.ToLower() + "_usage"));

                return embed;
            }
            else
            {
                Context.ReplyAsync(Context.Language.GetString("help_command_requires_nsfw"));
                return null;
            }
        }

        private EmbedBuilder GetSubCommandsHelp(YukiCommandContext Context, List<Command> commands, string commandStr, out bool isNsfw)
        {
            List<Command> subCommands = YukiBot.Discord.CommandService.GetAllModules()
                                                            .Where(mod => mod.Parent != null && mod.Name.ToLower() == commandStr.ToLower())
                                                            .SelectMany(mod => mod.Commands).ToList();

            /* Is commandStr a command group? */
            if (subCommands != null && subCommands.Count > 0)
            {
                EmbedBuilder embed = Context.CreateEmbedBuilder(subCommands[0].Module.Name);

                int nsfwCommands = 0;

                foreach (Command subCommand in subCommands)
                {
                    if (!subCommand.Checks.Any(attr => attr is RequireNsfwAttribute) || GuildSettings.IsChannelExplicit(Context.Channel.Id, Context.Guild.Id))
                    {
                        string description = Context.Language.GetString("command_" + subCommand.Name.ToLower().Replace(' ', '_') + "_desc") + "\n\n" +
                                             Context.Language.GetString("command_" + subCommand.Name.ToLower().Replace(' ', '_') + "_usage") + "\n";

                        embed.AddField(subCommand.Name, description);
                    }
                    else
                    {
                        nsfwCommands++;
                    }
                }

                if (nsfwCommands > 0)
                {
                    isNsfw = true;
                }
                else
                {
                    isNsfw = false;
                }

                return embed;
            }
            /* If it's not, we want to list the commands in the module commandStr */
            else
            {
                Module module = YukiBot.Discord.CommandService.GetAllModules()
                                                .FirstOrDefault(mod => mod.Name.ToLower() == commandStr.ToLower());

                if (module != null && module.Commands.Count > 0)
                {
                    if(!module.Checks.Any(check => check is RequireNsfwAttribute) || GuildSettings.IsChannelExplicit(Context.Channel.Id, Context.Guild.Id))
                    {
                        EmbedBuilder embed = Context.CreateEmbedBuilder(Context.Language.GetString("commands_title")).WithFooter(Context.Language.GetString("commands_help"));

                        foreach (Command command in module.Commands)
                        {
                            string embedValue = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));

                            embed.AddField(command.Name, (!string.IsNullOrWhiteSpace(embedValue) ? embedValue : Context.Language.GetString("commands_no_alias")), true);
                        }

                        List<string> subModules = YukiBot.Discord.CommandService.GetAllModules()
                                                         .FirstOrDefault(mod => mod.Name.ToLower() == commandStr.ToLower()).Submodules.Select(mod => mod.Name).ToList();

                        foreach (string mod in subModules)
                        {
                            embed.AddField(mod, Context.Language.GetString("command_is_submodule"), true);
                        }

                        isNsfw = false;
                        return embed;
                    }

                    isNsfw = true;

                    Context.ReplyAsync(Context.Language.GetString("help_commands_require_nsfw").Replace("%count%", module.Commands.Count.ToString()));

                    return null;
                }
                /* module doesn't exist */
                else
                {
                    isNsfw = false;
                    return null;
                }
            }
        }
    }
}
