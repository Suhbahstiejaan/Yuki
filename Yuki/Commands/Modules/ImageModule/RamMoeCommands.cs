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
            await RamMoe.SendImageAsync(Context, Language, "cry", false, null);
        }

        [Command("cuddle")]
        public async Task CuddleAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "cuddle", false, users);
        }

        [Command("hug")]
        public async Task HugAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "hug", false, users);
        }

        [Command("kiss")]
        public async Task KissAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "kiss", false, users);
        }

        [Command("lewd")]
        public async Task LewdAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "lewd", false, users);
        }

        [Command("lick")]
        public async Task LickAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "lick", false, users);
        }

        [Command("nom")]
        public async Task NomAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "nom", false, users);
        }

        [Command("nyan")]
        public async Task NyanAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "nyan", false, null);
        }

        [Command("owo")]
        public async Task OwoAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "owo", false, null);
        }

        [Command("pat")]
        public async Task PatAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "pat", false, users);
        }

        [Command("pout")]
        public async Task PoutAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "pout", false, null);
        }

        [Command("rem")]
        public async Task RemAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "rem", false, null);
        }

        [Command("slap")]
        public async Task SlapAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "slap", false, users);
        }

        [Command("smug")]
        public async Task SmugAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "smug", false, users);
        }

        [Command("stare")]
        public async Task StareAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "stare", false, users);
        }

        [Command("tickle")]
        public async Task TickleAsync(IUser[] users = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "tickle", false, users);
        }

        /*[Command("nsfwgtn")]
        public async Task NsfwGtnAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "nsfw_gtn", false);
        }*/
    }
}