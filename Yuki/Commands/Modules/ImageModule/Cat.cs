using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("cat")]
        public async Task GetCatImageAsync()
        {
            await ReplyAsync(Context.CreateImageEmbedBuilder(Language.GetString("cat_random_get"), await CatApi.GetImage()));
        }
    }
}
