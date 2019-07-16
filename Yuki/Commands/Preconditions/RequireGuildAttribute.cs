using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.Preconditions
{
    public class RequireGuildAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext c, IServiceProvider provider)
        {
            YukiCommandContext context = c as YukiCommandContext;

            if (context.Channel is IDMChannel)
            {
                return Task.FromResult(CheckResult.Unsuccessful("This command can only be executed inside a guild channel."));
            }
            else
            {
                return Task.FromResult(CheckResult.Successful);
            }
        }
    }
}
