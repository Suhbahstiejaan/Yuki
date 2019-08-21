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
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CryAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "cry", false, null);
        }

        [Command("cuddle")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CuddleAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "cuddle", false, users);
        }

        [Command("hug")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task HugAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "hug", false, users);
        }

        [Command("kiss")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task KissAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "kiss", false, users);
        }

        [Command("lewd")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task LewdAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "lewd", false, users);
        }

        [Command("lick")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task LickAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "lick", false, users);
        }

        [Command("nom")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task NomAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "nom", false, users);
        }

        [Command("nyan")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task NyanAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "nyan", false, null);
        }

        [Command("owo")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task OwoAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "owo", false, null);
        }

        [Command("pat")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task PatAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "pat", false, users);
        }

        [Command("pout")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task PoutAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "pout", false, null);
        }

        [Command("rem")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task RemAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, "rem", false, null);
        }

        [Command("slap")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task SlapAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "slap", false, users);
        }

        [Command("smug")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task SmugAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "smug", false, users);
        }

        [Command("stare")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task StareAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "stare", false, users);
        }

        [Command("tickle")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task TickleAsync(params IUser[] users)
        {
            await RamMoe.SendImageAsync(Context, Language, "tickle", false, users);
        }

        /*[Command("nsfwgtn")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task NsfwGtnAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "nsfw_gtn", false);
        }*/
    }
}