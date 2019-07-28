using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleRoles : ISettingPageTogglable
    {
        public string Name { get; set; } = "roles_toggle";

        public string GetState(YukiCommandContext Context)
        {
            return GuildSettings.GetGuild(Context.Guild.Id).EnableRoles ? "enabled" : "disabled";
        }

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleRoles(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("roles_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableRoles);
        }
    }
}
