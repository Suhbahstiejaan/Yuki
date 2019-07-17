using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingRemRole : ISettingPage
    {
        public string Name { get; set; } = "roles_remove";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(Module.Language.GetString("setting_role_rem_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                if (MentionUtils.TryParseRole(result.Value.Content, out ulong roleId))
                {
                    GuildSettings.RemRole(roleId, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("role_removed") + ": " + Context.Guild.GetRole(roleId).Name);
                }
            }
        }
    }
}
