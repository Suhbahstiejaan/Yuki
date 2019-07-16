using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("role", "r")]
        public async Task GiveRoleAsync(string roleString)
        {
            if(!(Context.Channel is IDMChannel))
            {
                List<ulong> roles = GuildSettings.GetGuild(Context.Guild.Id).AssignableRoles;

                ulong guildRole = MentionUtils.ParseRole(roleString);

                IRole givenRole = default;

                if(roles.Contains(guildRole))
                {
                    givenRole = Context.Guild.GetRole(guildRole);
                    await (Context.User as IGuildUser).AddRoleAsync(givenRole);
                }
                else
                {
                    foreach (ulong role in roles)
                    {
                        IRole irole = Context.Guild.GetRole(role);

                        if(irole.Name.ToLower().Contains(roleString.ToLower()) || irole.Name.ToLower() == roleString.ToLower())
                        {
                            await (Context.User as IGuildUser).AddRoleAsync(irole);
                            givenRole = irole;
                        }
                    }
                }

                if(!givenRole.Equals(default))
                {
                    await ReplyAsync(Language.GetString("role_given").Replace("%rolename%", givenRole.Name));
                }
                else
                {
                    await ReplyAsync(Language.GetString("role_not_found"));
                }
            }
        }
    }
}
