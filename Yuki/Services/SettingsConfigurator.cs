using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Data.Objects.Settings;
using Yuki.Extensions;

namespace Yuki.Services
{
    public class SettingsConfigurator
    {
        private YukiModule Module;
        private YukiCommandContext Context;

        public bool Running = true;

        private Dictionary<string, List<string>> SettingsRootOrder = new Dictionary<string, List<string>>()
        {
            {
                "root",
                new List<string>()
                {
                    "welcome",
                    "nsfw",
                    "log",
                    "cache",
                    "mute",
                    "prefix",
                    "roles",
                    "warnings",
                    "set_language"
                }
            },
            {
                "welcome",
                new List<string>()
                {
                    "welcome_set_welcome",
                    "welcome_set_goodbye",
                    "welcome_toggle_welcome",
                    "welcome_toggle_goodbye",
                }
            },
            {
                "nsfw",
                new List<string>()
                {
                    "nsfw_toggle",
                    "nsfw_add_channel",
                    "nsfw_rem_channel"
                }
            },
            {
                "log",
                new List<string>()
                {
                    "log_toggle",
                    "log_set"
                }
            },
            {
                "cache",
                new List<string>()
                {
                    "cache_toggle",
                    "cache_add_channel",
                    "cache_rem_channel"
                }
            },
            {
                "mute",
                new List<string>()
                {
                    "mute_toggle",
                    "mute_set_role",
                }
            },
            {
                "prefix",
                new List<string>()
                {
                    "prefix_toggle",
                    "prefix_set"
                }
            },
            {
                "roles",
                new List<string>()
                {
                    "roles_toggle",
                    "roles_set",
                    "roles_remove",
                    "roles_moderator",
                    "roles_administrator"
                }
            },
            {
                "warnings",
                new List<string>()
                {
                    "warnings_toggle",
                    "warnings_add",
                    "warnings_remove"
                }
            },
            {
                "roles_moderator",
                new List<string>()
                {
                    "roles_moderator_add",
                    "roles_moderator_remove",
                }
            },
            {
                "roles_administrator",
                new List<string>()
                {
                    "roles_administrator_add",
                    "roles_administrator_remove",
                }
            },
        };

        private Stack<string> SettingStack = new Stack<string>();

        private List<ISettingPage> SettingPages = new List<ISettingPage>()
        {
            /* welcome */
            new SettingSetGoodbye(),
            new SettingSetWelcome(),
            new SettingToggleWelcome(),
            new SettingToggleGoodbye(),

            /* nsfw */
            new SettingToggleNsfw(),
            new SettingAddChannelNsfw(),
            new SettingRemChannelNsfw(),

            /* log */
            new SettingToggleLogging(),
            new SettingAddChannelLogging(),

            /* cache */
            new SettingToggleCache(),
            new SettingAddChannelCache(),
            new SettingRemChannelCache(),

            /* mute */
            new SettingToggleMute(),
            new SettingSetMute(),

            /* prefix */
            new SettingTogglePrefix(),
            new SettingAddPrefix(),
            
            /* roles */
            new SettingToggleRoles(),
            new SettingAddRole(),
            new SettingRemRole(),

            /* warnings */
            new SettingToggleWarnings(),
            new SettingAddWarningAction(),
            new SettingRemWarningAction(),

            /* roles_moderator */
            new SettingAddModeratorRole(),
            new SettingRemModeratorRole(),

            /* roles_administrator */
            new SettingAddAdministratorRole(),
            new SettingRemAdministratorRole(),

            new SettingSetLanguage(),
        };

        private ISettingPage currentPage;

        public SettingsConfigurator(YukiModule module, YukiCommandContext context)
        {
            Running = true;
            Module = module;
            Context = context;

            SettingStack.Push("root");
        }

        public async Task Run()
        {

            EmbedBuilder embed = Context.CreateEmbedBuilder(Module.Language.GetString("config_title"))
                .WithDescription(string.Join("\n", SettingsRootOrder[SettingStack.Peek()]
                        .Select(str => "[" + (SettingsRootOrder[SettingStack.Peek()].IndexOf(str) + 1) + "] " + Module.Language.GetString(str))))
                .WithFooter(Module.Language.GetString("config_footer"));


            await embed.SendToAsync(Context.Channel);

            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if(result.IsSuccess)
            {
                if (int.TryParse(result.Value.Content, out int index))
                {
                    if(index > 0 && index < SettingsRootOrder[SettingStack.Peek()].Count)
                    {
                        currentPage = SettingPages.FirstOrDefault(page => page.Name == SettingsRootOrder[SettingStack.Peek()][index - 1]);

                        if(currentPage == null)
                        {
                            SettingStack.Push(SettingsRootOrder.ElementAt(index).Key);
                            
                            return;
                        }

                        currentPage.Display(Module, Context);

                        await currentPage.Run(Module, Context);
                    }
                }
                if (result.Value.Content.ToLower() == Module.Language.GetString("back").ToLower())
                {
                    if (SettingStack.Count > 1)
                    {
                        SettingStack.Pop();
                    }
                }
                else if (result.Value.Content.ToLower() == Module.Language.GetString("exit").ToLower())
                {
                    await Context.Channel.SendMessageAsync(Module.Language.GetString("settings_exit"));
                    Running = false;
                }
            }
        }
    }
}
