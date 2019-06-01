using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("8ball")]
        public async Task MagicEightBall([Remainder] string args = "")
        {
            int num = new YukiRandom().Next(1, 20);

            await ReplyAsync(Context.CreateEmbed(Language.GetString("eightball_response_" + num), new EmbedAuthorBuilder() { Name = Language.GetString("eightball_response_title") }));
        }
    }
}
