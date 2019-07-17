using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleMute : ISettingPage
    {
        public string Name { get; set; } = "mute_toggle";

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleMute(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("mute_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableMute);
        }
    }
}
