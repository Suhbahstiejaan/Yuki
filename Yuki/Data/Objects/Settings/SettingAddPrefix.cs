using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingAddPrefix : ISettingPage
    {
        public string Name { get; set; } = "prefix_set";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_prefix_add_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                GuildSettings.AddPrefix(result.Value.Content, Context.Guild.Id);
                await Module.ReplyAsync(Module.Language.GetString("prefix_added") + ": " + GuildSettings.GetGuild(Context.Guild.Id).Prefix);
            }
        }
    }
}
