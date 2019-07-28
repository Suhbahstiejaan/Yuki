using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleWelcome : ISettingPageTogglable
    {
        public string Name { get; set; } = "welcome_toggle_welcome";

        public string GetState(YukiCommandContext Context)
        {
            return GuildSettings.GetGuild(Context.Guild.Id).EnableWelcome ? "enabled" : "disabled";
        }

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleWelcome(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("welcome_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableWelcome);
        }
    }
}
