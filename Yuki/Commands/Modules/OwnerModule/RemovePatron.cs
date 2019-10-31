using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.OwnerModule
{
    public partial class OwnerModule
    {
        [RequireOwner]
        [Command("removepatron")]
        public async Task RemovePatronAsync(ulong userId)
        {
            UserSettings.RemovePatron(userId);

            await ReplyAsync(Language.GetString("patron_removed"));
        }
    }
}
