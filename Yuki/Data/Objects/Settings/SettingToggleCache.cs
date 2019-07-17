using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleCache : ISettingPage
    {
        public string Name { get; set; } = "cache_toggle";

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleLogging(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("cache_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableCache);
        }
    }
}
