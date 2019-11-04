using Discord;
using Newtonsoft.Json;
using Qmmands;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Commands.Modules.NsfwModule
{
    public partial class NsfwModule
    {
        [Command("boobs", "boob")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task BoobsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                int boobCount = 0;

                using (StreamReader reader = new StreamReader(await client.GetStreamAsync("http://api.oboobs.ru/boobs/")))
                {
                    boobCount = JsonConvert.DeserializeObject<Butts[]>(await reader.ReadToEndAsync())[0].id;
                }

                using (StreamReader reader = new StreamReader(await client.GetStreamAsync($"http://api.oboobs.ru/boobs/{new Random().Next(boobCount)}")))
                {
                    Butts boob = JsonConvert.DeserializeObject<Butts[]>(await reader.ReadToEndAsync())[0];

                    EmbedBuilder embed = new EmbedBuilder()
                    .WithImageUrl("http://media.oboobs.ru/" + boob.preview)
                    .WithFooter("obutts.ru");

                    await ReplyAsync(embed);
                }
            }
        }
    }
}
