using Discord.Commands;
using System;
using System.Threading.Tasks;
using Yuki.Bot.Entity;

namespace Yuki.Bot.Common
{
    public class OwnerOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            => Task.FromResult(Config.Get().IsOwner(context.User) || context.Client.CurrentUser.Id == context.User.Id ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("User is not owner"));
    }
}
