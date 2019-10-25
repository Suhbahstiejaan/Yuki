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

            if(reason == null)
            {
                reason = Language.GetString("no_reason");
            }

            if (config.EnableWarnings)
            {
                GuildSettings.AddWarning(user.Id, reason, Context.Guild.Id);

                int warningIndex = GuildSettings.GetWarnedUser(user.Id, Context.Guild.Id).Warning - 1;
                GuildWarningAction userWarning = config.WarningActions[warningIndex];

                switch (userWarning.WarningAction)
                {
                    case WarningAction.GiveRole:
                        await user.AddRoleAsync(Context.Guild.GetRole(userWarning.RoleId));

                        if(warningIndex > 0)
                        {
                            await user.RemoveRoleAsync(Context.Guild.GetRole(config.WarningActions[warningIndex - 1].RoleId));
                        }

                        await ReplyAsync(Language.GetString("user_warned").Replace("%user%", user.Mention).Replace("reason", reason));
                        break;
                    case WarningAction.Kick:
                        await user.KickAsync(reason);

                        await ReplyAsync(Language.GetString("user_kicked").Replace("%user%", user.Mention).Replace("reason", reason));
                        break;
                    case WarningAction.Ban:
                        await user.BanAsync(0, reason);

                        await ReplyAsync(Language.GetString("user_banned").Replace("%user%", user.Mention).Replace("reason", reason));
                        break;
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("warnings_disabled"));
            }
        }
    }
}
