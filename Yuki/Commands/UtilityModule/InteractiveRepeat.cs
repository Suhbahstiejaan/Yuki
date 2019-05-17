using Discord.WebSocket;
using InteractivityAddon;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("repeat")]
        public async Task ExampleReplyNextMessageAsync()
        {
            await ReplyAsync("Send a message!");

            InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(x => x.Author == Context.User);

            if (result.IsSuccess)
            {
                await ReplyAsync(Context.CreateEmbed(result.Value.Content));
            }
        }
    }
}
