using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingAddRole : ISettingPage
    {
        public string Name { get; set; } = "role_add";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_role_add_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                if (MentionUtils.TryParseRole(result.Value.Content, out ulong roleId))
                {
                    GuildSettings.AddRole(roleId, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("role_added") + ": " + Context.Guild.GetRole(roleId).Name);
                }
            }
        }
    }
}
