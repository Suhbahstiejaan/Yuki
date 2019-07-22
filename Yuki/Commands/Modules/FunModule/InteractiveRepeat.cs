using Discord.WebSocket;
using InteractivityAddon;
using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("repeat")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ExampleReplyNextMessageAsync()
        {
            await ReplyAsync(Language.GetString("interactive_repeat_send_message"));

            InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(x => x.Author == Context.User && x.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                await ReplyAsync(result.Value.Content);
            }
        }
    }
}
