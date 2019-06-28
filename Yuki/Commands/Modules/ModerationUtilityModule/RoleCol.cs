using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("rolecol")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetRoleColorAsync([Remainder] string args)
        {
            string lastString = args.Split(' ').LastOrDefault();

            Color newCol = lastString.AsColor();

            IRole yukiHighestRole = (await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id)).HighestRole();
            IRole executorHighestRole = ((IGuildUser)Context.User).HighestRole();
            IRole roleToChange = Context.Guild.Roles.FirstOrDefault(role => role.Name.ToLower() == args.Replace(lastString, "").Replace(" ", "").ToLower());

            Color oldCol = roleToChange.Color;

            if (!RoleHasPriority(yukiHighestRole, roleToChange))
            {
                await ReplyAsync(Language.GetString("rolecol_bot_require_higher"));
                return;
            }

            if (!RoleHasPriority(executorHighestRole, roleToChange))
            {
                await ReplyAsync(Language.GetString("rolecol_user_require_higher"));
                return;
            }

            if (!(await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id)).UserHasPermission(GuildPermission.ManageRoles))
            {
                await ReplyAsync(Language.GetString("permission_require_manage_roles"));
                return;
            }

            await roleToChange.ModifyAsync(role => role.Color = newCol);

            await ReplyAsync(Context.CreateColoredEmbed(newCol, new EmbedAuthorBuilder()
            {
                Name = Language.GetString("success"),
            }, Language.GetString("rolecol_set").Replace("%rolename%", roleToChange.Name).Replace("%oldcol%", oldCol.ToString()).Replace("%newcol%", newCol.ToString())));
        }
    }
}
