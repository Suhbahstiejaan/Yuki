using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings.Togglable
{
    public class SettingToggleCache : ISettingPageTogglable
    {
        public string Name { get; set; } = "cache_toggle";

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public string GetState(YukiCommandContext Context)
        {
            return GuildSettings.GetGuild(Context.Guild.Id).EnableCache ? "enabled" : "disabled";
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleLogging(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("cache_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableCache);
        }
    }
}
