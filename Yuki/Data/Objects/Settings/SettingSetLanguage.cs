using Discord.WebSocket;
using InteractivityAddon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingSetLanguage : ISettingPage
    {
        public string Name { get; set; } = "set_language";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(Module.Language.GetString("set_language_text"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author.Id == Context.User.Id && msg.Channel.Id == Context.Channel.Id);

            if(result.IsSuccess)
            {
                string code = LocalizationService.GetLanguage(result.Value.Content).Code;

                if (code == "none")
                {
                    await Module.ReplyAsync(Module.Language.GetString("language_not_found"));
                }
                else
                {
                    GuildSettings.SetLanguage(code, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("language_set").Replace("%lang%", code));
                }
            }
        }
    }
}
