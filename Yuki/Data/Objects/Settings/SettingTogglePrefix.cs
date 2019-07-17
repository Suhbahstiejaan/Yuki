using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingTogglePrefix : ISettingPage
    {
        public string Name { get; set; } = "prefix_toggle";

        public void Display(YukiModule Module, YukiCommandContext Context) { }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.TogglePrefix(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("prefix_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnablePrefix);
        }
    }
}
