using Discord.WebSocket;
using InteractivityAddon;
using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("repeat")]
        public async Task ExampleReplyNextMessageAsync()
        {
            await ReplyAsync(Language.GetString("interactive_repeat_send_message"));

            InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(x => x.Author == Context.User && x.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                await ReplyAsync(Context.CreateEmbed(result.Value.Content));
            }
        }
    }
}
