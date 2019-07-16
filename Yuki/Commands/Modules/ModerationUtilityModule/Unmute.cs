using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("unmute")]
        [RequireModerator]
        public async Task UnmuteUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableMute)
            {
                await user.RemoveRoleAsync(Context.Guild.GetRole(config.MuteRole));
                await ReplyAsync(Language.GetString("user_unmuted"));
            }
            else
            {
                await ReplyAsync(Language.GetString("mute_disabled"));
            }
        }
    }
}
