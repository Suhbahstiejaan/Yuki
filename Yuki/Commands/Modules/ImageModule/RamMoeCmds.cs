using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

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
        public async Task CuddleAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "cuddle", false, user);
        }

        [Command("hug")]
        public async Task HugAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "hug", false, user);
        }

        [Command("kiss")]
        public async Task KissAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "kiss", false, user);
        }

        [Command("lewd")]
        public async Task LewdAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "lewd", false, user);
        }

        [Command("lick")]
        public async Task LickAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "lick", false, user);
        }

        [Command("nom")]
        public async Task NomAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "nom", false, user);
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
        public async Task PatAsync(params IUser[] user)
        {
            await RamMoe.SendImageAsync(Context, Language, "pat", false, user);
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
        public async Task SlapAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "slap", false, user);
        }

        [Command("smug")]
        public async Task SmugAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "smug", false, user);
        }

        [Command("stare")]
        public async Task StareAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "stare", false, user);
        }

        [Command("tickle")]
        public async Task TickleAsync(IUser user = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "tickle", false, user);
        }

        /*[Command("nsfwgtn")]
        public async Task NsfwGtnAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "nsfw_gtn", false);
        }*/
    }
}
