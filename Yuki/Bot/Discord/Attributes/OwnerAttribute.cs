using Discord.Commands;
using System;
using System.Threading.Tasks;
using Yuki.Bot.Entities;

namespace Yuki.Bot.Discord.Attributes
{
    public class OwnerOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            BotCredentials creds = (BotCredentials)services.GetService(typeof(BotCredentials));

            return Task.FromResult((creds.IsOwner(context.User) || context.Client.CurrentUser.Id == context.User.Id ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("User is not owner")));
        }
    }
}
