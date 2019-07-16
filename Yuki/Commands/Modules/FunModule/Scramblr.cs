using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("scramblr", "scrambler")]
        public async Task ScramblrAsync(IUser user = null)
        {
            Scramblr scramblr = new Scramblr();

            if(UserSettings.CanGetMsgs(Context.User.Id))
            {
                if (user != null)
                {
                    if (UserSettings.CanGetMsgs(user.Id))
                    {
                        await ReplyAsync(scramblr.GetMessage(Context.User, user));
                    }
                    else
                    {
                        await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("%user%", $"{user.Username}#{user.Discriminator}"));
                    }
                }
                else
                {
                    await ReplyAsync(scramblr.GetMessage(Context.User));
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("%user%", $"{Context.User.Username}#{Context.User.Discriminator}"));
            }
        }
    }
}
