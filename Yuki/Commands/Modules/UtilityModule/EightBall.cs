using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Core;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("8ball")]
        public async Task MagicEightBall([Remainder] string args = "")
        {
            try
            {
                int num = new YukiRandom().Next(1, 20);

                EmbedAuthorBuilder author = new EmbedAuthorBuilder()
                {
                    Name = Language.GetString("eightball_response_title"),
                    IconUrl = "attachment://8ball.png"
                };

                await SendFileAsync(FileDirectories.ImageRoot + "8ball.png", Context.CreateEmbedBuilder(Language.GetString("eightball_response_" + num), author));
            }
            catch(Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
    }
}
