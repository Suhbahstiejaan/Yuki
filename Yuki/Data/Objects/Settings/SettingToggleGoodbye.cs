using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleGoodbye : ISettingPage
    {
        public string Name { get; set; } = "welcome_toggle_goodbye";

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleGoodbye(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("goodbye_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableWelcome);
        }
    }
}
