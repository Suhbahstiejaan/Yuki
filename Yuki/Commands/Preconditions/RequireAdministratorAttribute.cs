using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Preconditions
{
    public class RequireAdministratorAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext c)
        {
            YukiCommandContext context = c as YukiCommandContext;

            CheckResult result = CheckResult.Unsuccessful("You must be an administrator to execute this command.");

            if (context.Guild.OwnerId == context.User.Id)
            {
                return CheckResult.Successful;
            }

            foreach (ulong role in GuildSettings.GetGuild(context.Guild.Id).AdministratorRoles)
            {
                if ((context.User as IGuildUser).RoleIds.Contains(role))
                {
                    result = CheckResult.Successful;
                    break;
                }
            }

            return result;
        }
    }
}
