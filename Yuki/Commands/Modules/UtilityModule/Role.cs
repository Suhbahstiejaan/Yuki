using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
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
                foreach(IRole role in Context.Guild.Roles)
                {
                    if(role.Name.ToLower() == roleString.ToLower())
                    {
                        queriedRole = role;
                        break;
                    }
                }

                if(queriedRole == default)
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleString).Replace("%user%", Context.User.Username));
                    return;
                }
            }

            GuildRole assignedRole = GuildSettings.GetGuild(Context.Guild.Id).GuildRoles.FirstOrDefault(role => role.Id == queriedRole.Id);

            if (!assignedRole.Equals(default(GuildRole)))
            {
                if (!((IGuildUser)Context.User).RoleIds.Contains(queriedRole.Id))
                {
                    await ((IGuildUser)Context.User).AddRoleAsync(Context.Guild.GetRole(queriedRole.Id));

                    await ReplyAsync(Language.GetString("role_given").Replace("%rolename%", queriedRole.Name).Replace("%user%", Context.User.Username));

                    if(assignedRole.IsTeamRole)
                    {
                        foreach(ulong roleId in (await Context.Guild.GetUserAsync(Context.User.Id)).RoleIds)
                        {
                            GuildRole role = GuildSettings.GetGuild(Context.Guild.Id).GuildRoles.FirstOrDefault(_role => _role.Id == roleId);
                            
                            if(!role.Equals(default) && role.IsTeamRole&& role.Id != assignedRole.Id)
                            {
                                IRole _role = Context.Guild.GetRole(role.Id);
                                await ((IGuildUser)Context.User).RemoveRoleAsync(_role);

                                await ReplyAsync(Language.GetString("role_taken").Replace("%rolename%", _role.Name).Replace("%user%", Context.User.Username));
                            }
                        }
                    }
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
