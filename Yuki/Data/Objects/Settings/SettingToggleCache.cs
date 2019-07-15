using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleCache : ISettingPage
    {
        public string Name { get; set; } = "log_toggle_cache";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_cache_toggle_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleLogging(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("cache_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableCache);
        }
    }
}
