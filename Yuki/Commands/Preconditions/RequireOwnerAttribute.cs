using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data;

namespace Yuki.Commands.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireOwnerAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext context, IServiceProvider provider)
        {
            if (Config.GetConfig().owners.Any(o => o == ((YukiCommandContext)context).User.Id))
            {
                return Task.FromResult(CheckResult.Successful);
            }
            else
            {
                return Task.FromResult(CheckResult.Unsuccessful("You must be a bot owner to run this command."));
            }
        }
    }
}