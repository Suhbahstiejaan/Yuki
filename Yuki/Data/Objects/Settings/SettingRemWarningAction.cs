using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingRemWarningAction : ISettingPage
    {
        public string Name { get; set; } = "warnings_remove";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_warning_rem_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                if (int.TryParse(result.Value.Content, out int num))
                {
                    GuildSettings.RemWarningAction(num, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("warningaction_removed") + ": " + result.Value.Content);
                }
            }
        }
    }
}
