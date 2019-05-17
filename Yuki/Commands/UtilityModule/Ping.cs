using Discord;
using Qmmands;
using System.Diagnostics;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            EmbedBuilder embedBuilder = Context.CreateEmbedBuilder("Pinging....");

            Stopwatch watch = Stopwatch.StartNew();
            IUserMessage msg = await embedBuilder.SendToAsync(Context.Channel);
            watch.Stop();

            await msg.ModifyAsync(emb =>
            {
                embedBuilder.WithDescription(
                    "Pong!\n" +
                    $"Took {watch.ElapsedMilliseconds}ms to respond\n" +
                    $"API latency: {Context.Client.Latency}ms"
                );
                emb.Embed = embedBuilder.Build();
            });
        }
    }
}
