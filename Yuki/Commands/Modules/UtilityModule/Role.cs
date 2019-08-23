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
        public async Task GiveRoleAsync(string roleString)
        {
            IRole givenRole = default;

            ulong guildRole = 0;

            if (MentionUtils.TryParseRole(roleString, out guildRole))
            {
                givenRole = Context.Guild.GetRole(guildRole);
            }
            else
            {
                foreach (IRole role in Context.Guild.Roles.Where(r => GuildSettings.GetGuild(Context.Guild.Id).AssignableRoles.Contains(r.Id)))
                {
                    if (role.Name.ToLower().Contains(roleString.ToLower()) ||
                        role.Name.ToLower() == roleString.ToLower() ||
                        (ulong.TryParse(roleString, out ulong r) && role.Id == r))
                    {
                        givenRole = role;
                        break;
                    }
                }
            }

            if (givenRole != null && !givenRole.Equals(default))
            {
                if ((Context.User as IGuildUser).RoleIds.Contains(givenRole.Id))
                {
                    await (Context.User as IGuildUser).RemoveRoleAsync(givenRole);
                    await ReplyAsync(Language.GetString("role_taken").Replace("%rolename%", givenRole.Name).Replace("%user%", Context.User.Username));
                }
                else
                {
                    await (Context.User as IGuildUser).AddRoleAsync(givenRole);
                    await ReplyAsync(Language.GetString("role_given").Replace("%rolename%", givenRole.Name).Replace("%user%", Context.User.Username));
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleString).Replace("%user%", Context.User.Username));
            }
        }
    }
}
