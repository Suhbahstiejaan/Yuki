using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingSetWelcome : ISettingPage
    {
        public string Name { get; set; } = "welcome_set_welcome";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(Module.Language.GetString("setting_welcome_set_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if(result.IsSuccess)
            {
                GuildSettings.SetWelcome(result.Value.Content, Context.Guild.Id);
                await Module.ReplyAsync(Module.Language.GetString("welcome_set_to") + ": " + result.Value.Content);
            }
        }
    }
}
