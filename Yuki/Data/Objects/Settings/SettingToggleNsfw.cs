using Discord;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingToggleNsfw : ISettingPage
    {
        public string Name { get; set; } = "nsfw_toggle_nsfw";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_nsfw_toggle_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            GuildSettings.ToggleNsfw(Context.Guild.Id);
            await Module.ReplyAsync(Module.Language.GetString("nsfw_toggled") + ": " + GuildSettings.GetGuild(Context.Guild.Id).EnableNsfw);
        }
    }
}
