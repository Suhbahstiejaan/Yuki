using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ServerOwnerModule
{
    public partial class ServerOwnerModule
    {
        [Group("administratorrole", "adminrole")]
        public class AdministratorRole : YukiModule
        {
            [Command("add")]
            public async Task AddAdminRoleAsync([Remainder] string roleName)
            {

                ulong roleId = 0;

                if (!MentionUtils.TryParseRole(roleName, out roleId))
                {
                    roleId = Context.Guild.Roles.FirstOrDefault(role => role.Name.ToLower() == roleName.ToLower()).Id;
                }

                GuildSettings.AddRoleAdministrator(roleId, Context.Guild.Id);

                await ReplyAsync(Language.GetString("administrator_role_added").Replace("%rolename%", roleName));
            }

            [Command("remove", "rem")]
            public async Task RemoveAdminRoleAsync([Remainder] string roleName)
            {
                ulong roleId = 0;

                if (!MentionUtils.TryParseRole(roleName, out roleId))
                {
                    roleId = Context.Guild.Roles.FirstOrDefault(role => role.Name.ToLower() == roleName.ToLower()).Id;
                }

                GuildSettings.RemoveRoleAdministrator(roleId, Context.Guild.Id);

                await ReplyAsync(Language.GetString("administrator_role_removed").Replace("%rolename%", roleName));
            }
        }
    }
}
