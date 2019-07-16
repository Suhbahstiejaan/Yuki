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
        [Command("remwarn", "unwarn")]
        [RequireAdministrator]
        public async Task RemWarnAsync(IGuildUser user)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableWarnings)
            {
                await user.RemoveRoleAsync(Context.Guild.GetRole(config.MuteRole));

                GuildSettings.RemWarning(user.Id, Context.Guild.Id);

                await ReplyAsync(Language.GetString("user_remwarn"));
            }
            else
            {
                await ReplyAsync(Language.GetString("warnings_disabled"));
            }
        }
    }
}
