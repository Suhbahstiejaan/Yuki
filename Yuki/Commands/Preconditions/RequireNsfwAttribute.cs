using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.Preconditions
{
    public class RequireNsfwAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext c, IServiceProvider provider)
        {
            YukiCommandContext context = c as YukiCommandContext;

            CheckResult result = (context.Channel is IDMChannel || ((ITextChannel)context.Channel).IsNsfw) ?
                                    CheckResult.Successful : CheckResult.Unsuccessful("Not an NSFW channel");

            return Task.FromResult(result);
        }
    }
}
