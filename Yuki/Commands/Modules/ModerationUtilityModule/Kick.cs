using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("kick")]
        [RequireModerator]
        public async Task KickAsync(IUser user)
        {
            await (await Context.Guild.GetUserAsync(user.Id)).KickAsync();
            await ReplyAsync(Language.GetString("user_kicked"));
        }
    }
}
