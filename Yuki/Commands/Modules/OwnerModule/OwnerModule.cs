using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.OwnerModule
{
    [Name("Owner")]
    public partial class OwnerModule : YukiModule
    {
        [Command("reset")]
        public async Task ResetAsync()
        {
            await Context.Client.CurrentUser.ModifyAsync(u =>
            {
                u.Username = "Yuki Dev";
                u.Avatar = new Discord.Optional<Discord.Image?>(new Discord.Image(@"C:\Users\Vee\Downloads\b03d0723c43a9694d4ae92319fdea6a4.png"));
            });
        }
    }
}
