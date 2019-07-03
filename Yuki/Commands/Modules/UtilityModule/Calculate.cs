using Qmmands;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("calculate", "calc")]
        public async Task CalculateAsync([Remainder] string expression)
        {
            await ReplyAsync(expression.Calculate());
        }
    }
}
