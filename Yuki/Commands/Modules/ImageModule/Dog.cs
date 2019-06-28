using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("dog")]
        public async Task GetDogImageAsync()
        {
            await ReplyAsync(Context.CreateImageEmbedBuilder(Language.GetString("dog_random_get"), await DogApi.GetImage()));
        }
    }
}
