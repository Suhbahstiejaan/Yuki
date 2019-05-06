using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Yuki.Data;
using Yuki.Services.Localization;

namespace Yuki.Modules.UserModule
{
    public partial class User : ModuleBase
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            Language lang = YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage(Context);

            IUserMessage sent = await ReplyAsync(lang.GetString("ping_pong"));

            await sent.ModifyAsync(msg =>
                {
                    msg.Content = lang.GetString("ping_waiting");
                }); //heartbeat

            double pingMs = sent.EditedTimestamp.Value.DateTime.Subtract(sent.CreatedAt.DateTime).TotalMilliseconds;

            await sent.ModifyAsync(msg => { msg.Content = lang.GetString("ping_response").Replace("<ms>", pingMs.ToString()); });
        }
    }
}
