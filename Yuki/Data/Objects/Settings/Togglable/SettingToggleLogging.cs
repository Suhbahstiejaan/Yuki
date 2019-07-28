using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleLogging : ISettingPageTogglable
    {
        public string Name { get; set; } = "log_toggle";

        public string GetState(YukiCommandContext Context)
        {
            return GuildSettings.GetGuild(Context.Guild.Id).EnableGoodbye ? "enabled" : "disabled";
        }

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleLogging(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("logging_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableWelcome);
        }
    }
}
