using Discord;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.API.RamMoe
{
    public class RamMoe
    {
        public static async Task<string> GetImage(string type, bool isNsfw = false)
        {
            using(HttpClient http = new HttpClient())
            {
                string url = "https://rra.ram.moe/i/r?type=" + type + "&nsfw=" + isNsfw.ToString().ToLower();

                /* Read the json, return the received string */
                using(StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    RamMoeAPI moe = JsonConvert.DeserializeObject<RamMoeAPI>(reader.ReadToEnd());

                    string ramMoeStr = "https://cdn.ram.moe/" + moe.path.Remove(0, 3);

                    string[] blacklist = Localizer.RamMoeBlacklist;
                    
                    /* Get a new image if the returned one is in our blacklist */
                    for (int i = 0; i < blacklist.Length; i++)
                        if (blacklist[i] == ramMoeStr)
                            return (await GetImage(type, isNsfw));

                    return ramMoeStr;
                }
            }
        }

        public static async Task SendImage(string type, string executor, string user, IUserMessage message)
        {
            string text = type.First().ToString().ToUpper() + type.Substring(1) + (type == "kiss" ? "es" : "s") + " " + executor;

            if (user != null)
            {
                if(!(message.Channel is IGuildChannel))
                {
                    ulong userId = ((IGuildChannel)message.Channel).Guild.GetUserId(user);

                    if (userId != 0)
                        user = YukiClient.Instance.DiscordClient.GetShardFor((message.Channel is IGuildChannel) ? ((IGuildChannel)message.Channel).Guild : null).GetUser(userId).Username;
                }

                text = executor + " " + type + (type == "kiss" ? "es" : "s") + " " + user;
            }

            await message.Channel.SendMessageAsync("", false, Embeds.ImageEmbed(await GetImage(type), message, text));
        }
    }
}
