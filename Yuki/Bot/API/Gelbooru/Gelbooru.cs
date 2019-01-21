using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot.Common;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.API.Gelbooru
{
    public class Gelbooru
    {
        private static readonly string apiUrl = "https://gelbooru.com/index.php?page=dapi&json=1&s=post&q=index&limit=500&tags=rating%3A";

        private static YukiRandom _random = new YukiRandom();

        /* Return a random image with the received search terms */
        public static async Task<YukiImage> GetImage(string term = null, bool isNsfwSearch = false)
        {
            List<YukiImage> images = await GetImages(term, isNsfwSearch);

            return images[_random.Next(images.Count)];
        }

        /* Return a List of images that match our search terms */
        public static async Task<List<YukiImage>> GetImages(string term = null, bool isNsfwSearch = false)
        {
            List<YukiImage> images = new List<YukiImage>();

            using (HttpClient http = new HttpClient())
            {
                string url = apiUrl;

                if (isNsfwSearch)
                    url += "explicit";
                else
                    url += "safe";

                if (term != null)
                    url += "%20" + term.Replace(" ", "%20");
                
                string[] blacklistedTags = Localizer.Blacklist;

                for (int i = 0; i < blacklistedTags.Length; i++)
                    url += "%20-" + blacklistedTags[i];
                
                using (StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    GelbooruAPI[] gelbooru = JsonConvert.DeserializeObject<GelbooruAPI[]>(reader.ReadToEnd());
                    
                    if (gelbooru != null)
                    {
                        for (int i = 0; i < gelbooru.Length; i++)
                            images.Add(new YukiImage()
                            {
                                Url = gelbooru[i].file_url,
                                Source = gelbooru[i].source,
                                Rating = gelbooru[i].score,
                                Width = gelbooru[i].width,
                                Height = gelbooru[i].height
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
