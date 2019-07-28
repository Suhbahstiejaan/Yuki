using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleGoodbye : ISettingPageTogglable
    {
        public string Name { get; set; } = "welcome_toggle_goodbye";

        public string GetState(YukiCommandContext Context)
        {
            return GuildSettings.GetGuild(Context.Guild.Id).EnableGoodbye ? "enabled" : "disabled";
        }

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleGoodbye(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("goodbye_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableWelcome);
        }
    }
}
