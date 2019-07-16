using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("mute")]
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
