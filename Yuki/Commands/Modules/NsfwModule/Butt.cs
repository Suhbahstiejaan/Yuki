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
        [Command("butts", "butt")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ButtsAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    int buttCount = 0;

                    using (StreamReader reader = new StreamReader(await client.GetStreamAsync("http://api.obutts.ru/butts/")))
                    {
                        buttCount = JsonConvert.DeserializeObject<Butts[]>(await reader.ReadToEndAsync())[0].id;
                    }

                    using (StreamReader reader = new StreamReader(await client.GetStreamAsync($"http://api.obutts.ru/butts/{new Random().Next(buttCount)}")))
                    {
                        Butts butt = JsonConvert.DeserializeObject<Butts[]>(await reader.ReadToEndAsync())[0];

                        EmbedBuilder embed = new EmbedBuilder()
                        .WithImageUrl("http://media.obutts.ru/" + butt.preview)
                        .WithFooter("obutts.ru");

                        await ReplyAsync(embed);
                    }
                }
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}
