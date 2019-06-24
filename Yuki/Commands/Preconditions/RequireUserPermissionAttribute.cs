using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;

/*
 * Modification of foxbot's RequireUserPermission attribute to work with qmmands
 * Credit: foxbot
 * https://github.com/discord-net/Discord.Net/blob/dev/src/Discord.Net.Commands/Attributes/Preconditions/RequireUserPermissionAttribute.cs
*/

namespace Yuki.Commands.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireUserPermissionAttribute : CheckBaseAttribute
    {
        public GuildPermission? GuildPermission { get; }

        public ChannelPermission? ChannelPermission { get; }

        public RequireUserPermissionAttribute(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }

        public RequireUserPermissionAttribute(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }

        public override Task<CheckResult> CheckAsync(ICommandContext _context, IServiceProvider provider)
        {
            YukiCommandContext context = (YukiCommandContext)_context;

            IGuildUser guildUser = context.User as IGuildUser;

            if (GuildPermission.HasValue)
            {
                if (guildUser == null)
                    return Task.FromResult(CheckResult.Unsuccessful("Command must be used in a guild channel."));
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                    return Task.FromResult(CheckResult.Unsuccessful($"User requires guild permission {GuildPermission.Value}."));
            }

            if (ChannelPermission.HasValue)
            {
                ChannelPermissions perms;
                if (context.Channel is IGuildChannel guildChannel)
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    perms = ChannelPermissions.All(context.Channel);

                if (!perms.Has(ChannelPermission.Value))
                    return Task.FromResult(CheckResult.Unsuccessful($"User requires channel permission {ChannelPermission.Value}."));
            }

            return Task.FromResult(CheckResult.Successful);
        }
    }
}
