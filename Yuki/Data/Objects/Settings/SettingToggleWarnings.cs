using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleWarnings : ISettingPage
    {
        public string Name { get; set; } = "warnings_toggle";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_warning_toggle_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleWarnings(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("warnings_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableWarnings);
        }
    }
}
