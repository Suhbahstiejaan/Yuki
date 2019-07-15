using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingSetGoodbye : ISettingPage
    {
        public string Name { get; set; } = "welcome_set_goodbye";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_goodbye_set_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                GuildSettings.SetGoodbye(result.Value.Content, Context.Guild.Id);
                await Module.ReplyAsync(Module.Language.GetString("goodbye_set_to") + ": " + result.Value.Content);
            }
        }
    }
}
