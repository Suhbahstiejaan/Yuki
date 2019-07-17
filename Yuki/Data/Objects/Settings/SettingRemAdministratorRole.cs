using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingRemAdministratorRole : ISettingPage
    {
        public string Name { get; set; } = "roles_administrator_remove";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(Module.Language.GetString("setting_administrator_rem_role_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                if (MentionUtils.TryParseRole(result.Value.Content, out ulong roleId))
                {
                    GuildSettings.RemRoleAdministrator(roleId, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("administrator_removed") + ": " + Context.Guild.GetRole(roleId).Name);
                }
            }
        }
    }
}
