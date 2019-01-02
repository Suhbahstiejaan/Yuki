using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.API.Danbooru
{
    public class Danbooru
    {
        private static readonly string apiUrl = "https://danbooru.donmai.us/posts.json?utf8=✓&tags=";

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
                
                if (term != null)
                    url += term.Replace(" ", "+");

                using (StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    DanbooruAPI[] danbooru = JsonConvert.DeserializeObject<DanbooruAPI[]>(reader.ReadToEnd());

                    if (danbooru != null)
                    {
                        for (int i = 0; i < danbooru.Length; i++)
                            //check if the rating matches the search type, if it does add it to the list
                            if(((danbooru[i].rating == "e" || danbooru[i].rating == "q") && isNsfwSearch) ||
                                danbooru[i].rating == "s" && !isNsfwSearch)
                            {
                                bool isAllowed = true;
                                string[] tags = danbooru[i].tag_string.Split('+');
                                string[] blacklistedTags = Localizer.Blacklist;

                                for (int j = 0; j < tags.Length; j++)
                                    for (int k = 0; k < blacklistedTags.Length; k++)
                                        if (tags[j] == blacklistedTags[k])
                                            isAllowed = false;
                                
                                if(isAllowed)
                                    images.Add(new YukiImage()
                                    {
                                        Url = danbooru[i].file_url,
                                        Source = danbooru[i].source,
                                        Rating = danbooru[i].score,
                                        Width = danbooru[i].image_width,
                                        Height = danbooru[i].image_height
                                    });
                            }
                    }
                }
            }
            if (images.Count > 0)
                return images;

            return null;
        }
    }
}
