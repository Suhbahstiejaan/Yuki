using Discord;
using System;
using System.Linq;

namespace Yuki.Extensions
{
    public static class IGuildUserExtensions
    {
        public static IRole HighestRole(this IGuildUser user, IGuild guild)
        {
            return guild.Roles.Where(role => guild.GetUserAsync(user.Id).Result.RoleIds.Contains(role.Id)).OrderByDescending(role => role.Position).First();
        }

        public static bool UserHasPermission(this IGuildUser user, GuildPermission permission)
        {
            return user.GuildPermissions.Has(permission);
        }
    }
}
