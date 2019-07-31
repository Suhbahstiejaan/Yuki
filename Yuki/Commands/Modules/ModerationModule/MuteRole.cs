using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("muterole")]
        public class MuteRole : YukiModule
        {
            [Command("set")]
            public async Task SetMuteRoleAsync([Remainder] string roleName)
            {
                ulong roleId = 0;

                if (MentionUtils.TryParseRole(roleName, out ulong id))
                {
                    roleId = id;
                }
                else
                {
                    foreach (IRole irole in Context.Guild.Roles)
                    {
                        if (irole.Name.ToLower() == roleName.ToLower())
                        {
                            roleId = irole.Id;
                            break;
                        }
                    }
                }

                if (roleId == 0)
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleName).Replace("%user%", Context.User.Username));
                }
                else
                {
                    GuildSettings.SetMuteRole(roleId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("muterole_set").Replace("%rolename%", roleName));
                }
            }


            [Command("remove", "rem")]
            public async Task RemoveMuteRoleAsync([Remainder] string roleName)
            {
                ulong roleId = 0;

                if (MentionUtils.TryParseRole(roleName, out ulong id))
                {
                    roleId = id;
                }
                else
                {
                    foreach (IRole irole in Context.Guild.Roles)
                    {
                        if (irole.Name.ToLower() == roleName.ToLower())
                        {
                            roleId = irole.Id;
                            break;
                        }
                    }
                }

                if (roleId == 0)
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleName).Replace("%user%", Context.User.Username));
                }
                else
                {
                    GuildSettings.SetMuteRole(0, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("muterole_removed").Replace("%rolename%", roleName));
                }
            }
        }
    }
}
