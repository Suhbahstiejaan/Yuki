using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Services;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("cry")]
        public async Task CryAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "cry", false);
        }

        [Command("cuddle")]
        public async Task CuddleAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "cuddle", false);
        }

        [Command("hug")]
        public async Task HugAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "hug", false);
        }

        [Command("kiss")]
        public async Task KissAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "kiss", false);
        }

        [Command("lewd")]
        public async Task LewdAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "lewd", false);
        }

        [Command("lick")]
        public async Task LickAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "lick", false);
        }

        [Command("nom")]
        public async Task NomAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "nom", false);
        }

        [Command("nyan")]
        public async Task NyanAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "nyan", false);
        }

        [Command("owo")]
        public async Task OwoAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "owo", false);
        }

        [Command("pat")]
        public async Task PatAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "pat", false);
        }

        [Command("pout")]
        public async Task PoutAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "pout", false);
        }

        [Command("rem")]
        public async Task RemAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "rem", false);
        }

        [Command("slap")]
        public async Task SlapAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "slap", false);
        }

        [Command("smug")]
        public async Task SmugAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "smug", false);
        }

        [Command("stare")]
        public async Task StareAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "stare", false);
        }

        [Command("tickle")]
        public async Task TickleAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, user, "tickle", false);
        }

        /*[Command("nsfwgtn")]
        public async Task NsfwGtnAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "nsfw_gtn", false);
        }*/
    }
}
