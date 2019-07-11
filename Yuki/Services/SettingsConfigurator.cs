using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuki.Commands;
using Yuki.Extensions;

namespace Yuki.Services
{
    public class SettingsConfigurator
    {
        private YukiModule Module;
        private YukiCommandContext Context;

        public bool Running = true;

        public Dictionary<int, List<string>> Settings = new Dictionary<int, List<string>>()
        {
            { 0, new List<string>()
                {
                    "config_setting_welcome",
                    "config_setting_nsfw",
                    "config_setting_log",
                    "config_setting_cache",
                    "config_setting_mute",
                    "config_setting_prefix",
                    "config_setting_roles",
                    "config_setting_warnings"
                }
            }
        };

        private int Index = 0;

        public SettingsConfigurator(YukiModule module, YukiCommandContext context)
        {
            Module = module;
            Context = context;
        }

        public void Run()
        {
            switch(Index)
            {
                case 0:
                    break;
            }
        }

        private async void SettingsRoot()
        {
            EmbedBuilder embed = Context.CreateEmbedBuilder(Module.Language.GetString("config_title"), true)
                .WithDescription(string.Join("\n", Settings[Index].Select(str => Module.Language.GetString(str))));

            IUserMessage message = await embed.SendToAsync(Context.Channel);
            InteractivityResult<SocketMessage> result;

        }
    }

    public struct Setting
    {
        public List<string> Settings;
    }
}
