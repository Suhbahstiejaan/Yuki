using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleWelcome : ISettingPage
    {
        public string Name { get; set; } = "welcome_toggle_welcome";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_welcome_toggle_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleWelcome(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("welcome_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableWelcome);
        }
    }
}
