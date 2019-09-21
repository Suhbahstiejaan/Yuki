using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("role", "r")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GiveRoleAsync([Remainder] string roleString)
        {
            IRole queriedRole = default;

            if(MentionUtils.TryParseRole(roleString, out ulong guildRole))
            {
                queriedRole = Context.Guild.GetRole(guildRole);
            }
            else
            {
                await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleString).Replace("%user%", Context.User.Username));
                return;
            }

            if(GuildSettings.GetGuild(Context.Guild.Id).AssignableRoles.Contains(queriedRole.Id))
            {
                if (!(await Context.Guild.GetUserAsync(Context.User.Id)).RoleIds.Contains(queriedRole.Id))
                {
                    await (await Context.Guild.GetUserAsync(Context.User.Id)).AddRoleAsync(Context.Guild.GetRole(queriedRole.Id));

                    await ReplyAsync(Language.GetString("role_given").Replace("%rolename%", queriedRole.Name).Replace("%user%", Context.User.Username));
                }
                else
                {
                    await (await Context.Guild.GetUserAsync(Context.User.Id)).RemoveRoleAsync(Context.Guild.GetRole(queriedRole.Id));

                    await ReplyAsync(Language.GetString("role_taken").Replace("%rolename%", queriedRole.Name).Replace("%user%", Context.User.Username));
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleString).Replace("%user%", Context.User.Username));
            }
        }
    }
}
