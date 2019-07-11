using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Extensions;
using Yuki.Services;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("config", "settings")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ConfigureGuildSettingsAsync()
        {
            SettingsConfigurator settings = new SettingsConfigurator(this, Context);

            InteractivityResult<SocketMessage> result;

            while (settings.Running)
            {
                settings.Run();
                result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if (result.IsSuccess)
                {

                }
            }
        }
    }
}
