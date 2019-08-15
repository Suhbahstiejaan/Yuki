using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("warnings")]
        [RequireModerator]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetWarningsAsync(IGuildUser user, int page = 0)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableWarnings)
            {
                GuildWarnedUser wUser = GuildSettings.GetWarnedUser(user.Id, Context.Guild.Id);

                PageManager manager = new PageManager(wUser.WarningReasons.ToArray(), "warnings");

                await ReplyAsync(manager.GetPage(page));
            }
            else
            {
                await ReplyAsync(Language.GetString("warnings_disabled"));
            }
        }
    }
}
