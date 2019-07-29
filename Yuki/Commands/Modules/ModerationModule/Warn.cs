using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("warn")]
        [RequireModerator]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task WarnUserAsync(IGuildUser user, [Remainder] string reason = null)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableWarnings)
            {
                await user.AddRoleAsync(Context.Guild.GetRole(config.MuteRole));

                GuildSettings.AddWarning(user.Id, reason, Context.Guild.Id);

                await ReplyAsync(Language.GetString("user_warned"));
            }
            else
            {
                await ReplyAsync(Language.GetString("warnings_disabled"));
            }
        }
    }
}
