using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot.Common;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.API.Rule34
{
    public class Rule34
    {
        private static readonly string apiUrl = "https://rule34.xxx/index.php?page=dapi&json=1&s=post&q=index&tags=";

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
                string url = apiUrl;
                
                if (term != null)
                    url += term.Replace(" ", "+");

                string[] blacklistedTags = Localizer.Blacklist;

                for (int i = 0; i < blacklistedTags.Length; i++)
                    url += "+-" + blacklistedTags[i];

                using (StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    Rule34API[] r34 = JsonConvert.DeserializeObject<Rule34API[]>(reader.ReadToEnd());

                    if (r34 != null)
                    {
                        for (int i = 0; i < r34.Length; i++)
                            images.Add(new YukiImage()
                            {
                                Url = "https://us.rule34.xxx/images/" + r34[i].directory + "/" + r34[i].image,
                                Source = "https://rule34.xxx/index.php?page=post&s=view&id=" + r34[i].id,
                                Rating = r34[i].score ?? 0,
                                Width = r34[i].width ?? 0,
                                Height = r34[i].height ?? 0
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
