using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

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
        public async Task CuddleAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "cuddle", false, str);
        }

        [Command("hug")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task HugAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "hug", false, str);
        }

        [Command("kiss")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task KissAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "kiss", false, str);
        }

        [Command("lewd")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task LewdAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "lewd", false, str);
        }

        [Command("lick")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task LickAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "lick", false, str);
        }

        [Command("nom")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task NomAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "nom", false, str);
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
        public async Task PatAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "pat", false, str);
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
        public async Task SlapAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "slap", false, str);
        }

        [Command("smug")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task SmugAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "smug", false, str);
        }

        [Command("stare")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task StareAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "stare", false, str);
        }

        [Command("tickle")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task TickleAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "tickle", false, str);
        }

        /*[Command("nsfwgtn")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task NsfwGtnAsync()
        {
            await RamMoe.SendImageAsync(Context, Language, null, "nsfw_gtn", false);
        }*/
    }
}