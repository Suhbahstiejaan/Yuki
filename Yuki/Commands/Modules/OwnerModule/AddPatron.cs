using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.OwnerModule
{
    public partial class OwnerModule
    {
        [RequireOwner]
        [Command("addpatron")]
        public async Task AddPatronAsync(ulong userId)
        {
            UserSettings.AddPatron(userId);

            await ReplyAsync(Language.GetString("patron_added"));
        }
    }
}
