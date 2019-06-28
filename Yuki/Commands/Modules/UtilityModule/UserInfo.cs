using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("userinfo")]
        public async Task GetUserInfoAsync(IUser userParam = default)
        {
            try
            {
                if (Context.Channel is IDMChannel)
                {
                    return;
                }

                IGuildUser user = userParam as IGuildUser;

                if (user == default)
                {
                    user = Context.User as IGuildUser;
                }

                string perms = (!user.GuildPermissions.ToList().Equals(null) && user.GuildPermissions.ToList().Count > 0) ?
                                string.Join(", ", user.GuildPermissions.ToList()) : Language.GetString("none");

                string roles = (user.RoleIds != null && user.RoleIds.Count > 0) ?
                                string.Join(", ", Context.Guild.Roles.Where(role => user.RoleIds.Contains(role.Id)).Select(role => role.Name))
                                : Language.GetString("none");

                string activity = user.Activity != null ? Language.GetString(user.Activity.Type.ToString().ToLower()) : Language.GetString("activity");
                string game = !user.Activity.Equals(null) ? user.Activity.Name.ToString().ToLower() : Language.GetString("none");

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(user.HighestRole().Color)
                    .WithImageUrl(user.GetAvatarUrl())
                    .WithAuthor(new EmbedAuthorBuilder()
                    {
                        IconUrl = user.GetAvatarUrl(),
                        Name = user.Username + "#" + user.Discriminator
                    })
                    .AddField(Language.GetString("uinf_id"), user.Id, true)
                    .AddField(activity, game, true)
                    .AddField(Language.GetString("uinf_status"), Language.GetString(user.Status.ToString().ToLower()), true)
                    .AddField(Language.GetString("uinf_acc_create"), user.CreatedAt.DateTime.ToPrettyTime(false, false), true)
                    .AddField(Language.GetString("uinf_acc_join"), user.JoinedAt.Value.DateTime.ToPrettyTime(false, false), true)
                    .AddField(Language.GetString("uinf_permissions"), string.Join(", ", perms), true)
                    .AddField(Language.GetString("uinf_roles").Replace("%count%", user.RoleIds.Count.ToString()),
                                                roles, true);

                await ReplyAsync(embed);
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}
