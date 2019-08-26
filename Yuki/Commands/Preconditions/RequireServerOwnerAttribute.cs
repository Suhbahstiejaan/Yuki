using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuki.Commands.Preconditions
{
    public class RequireServerOwnerAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext c, IServiceProvider provider)
        {
            YukiCommandContext context = c as YukiCommandContext;

            if (context.Guild.OwnerId == context.User.Id)
            {
                return Task.FromResult(CheckResult.Successful);
            }

            return Task.FromResult(CheckResult.Unsuccessful("You must be the server owner to execute this command."));
        }
    }
}
