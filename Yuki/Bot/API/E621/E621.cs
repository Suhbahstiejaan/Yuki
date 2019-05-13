using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot.Common;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.API.E621
{
    public class E621
    {
        private static readonly string apiUrl = "https://e621.net/post/index.json?limit=500&tags=";

        private static YukiRandom _random = new YukiRandom();

        public static async Task<YukiImage> GetImage(string term = null)
        {
            List<YukiImage> images = await GetImages(term);

            return images[_random.Next(images.Count)];
        }

        public static async Task<List<YukiImage>> GetImages(string term = null)
        {
            List<YukiImage> images = new List<YukiImage>();

            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", "YukiBot");
                string url = apiUrl;
                
                if (term != null)
                    url += term.Replace(" ", "+");

                string[] blacklistedTags = Localizer.Blacklist;

                for (int i = 0; i < blacklistedTags.Length; i++)
                    url += "+-" + blacklistedTags[i];

                using (StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    E621API[] e621 = JsonConvert.DeserializeObject<E621API[]>(reader.ReadToEnd());

                    if (e621 != null)
                    {
                        for (int i = 0; i < e621.Length; i++)
                            images.Add(new YukiImage()
                            {
                                Url = e621[i].file_url,
                                Source = e621[i].source,
                                Rating = e621[i].score,
                                Width = e621[i].width,
                                Height = e621[i].height
                            });
                    }
                }
            }
            if (images.Count > 0)
                return images;

            return null;
        }
    }
}
