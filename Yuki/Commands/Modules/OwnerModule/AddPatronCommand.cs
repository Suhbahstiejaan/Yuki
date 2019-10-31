using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.OwnerModule
{
    public partial class OwnerModule
    {
        [Command("addpatroncmd")]
        public async Task AddPatronCmdAsync(ulong userId, string cmdName, [Remainder]string result)
        {

        }
    }
}
