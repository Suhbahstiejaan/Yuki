using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleRoles : ISettingPage
    {
        public string Name { get; set; } = "roles_toggle";

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleRoles(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("roles_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableRoles);
        }
    }
}
