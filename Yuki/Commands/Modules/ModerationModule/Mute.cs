using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("mute")]
        [RequireModerator]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task MuteUserAsync(IGuildUser user, string time, [Remainder] string reason = null)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            DateTime _time = time.ToDateTime();

            if (config.EnableMute)
            {
                await user.AddRoleAsync(Context.Guild.GetRole(config.MuteRole));
            }
            else
            {
                await ReplyAsync(Language.GetString("mute_disabled"));
            }
        }
    }
}
