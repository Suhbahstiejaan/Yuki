using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.API;
using Yuki.Data.Objects.API.Tags;

namespace Yuki.API
{
    public enum ImageType
    {
        Danbooru,
        Gelbooru,
        E621,
        Rule34,
        None
    }

    public static class ImageSearch
    {
        private static List<string> defaultBlackList = new List<string>()
        {
            "loli",
            "lolicon",
            "shota",
            "shotacon",
            "child",
            "kid",
            "cub"
        };

        public static string[] GetTags(string[] tag)
        {
            List<string> tags = new List<string>();

            for (int i = 0; i < tag.Length; i++)
            {
                using (HttpClient client = new HttpClient())
                {
                    using (StreamReader reader = new StreamReader(client.GetStreamAsync($"https://danbooru.donmai.us/tag_aliases.json?search[name_matches]={tag[i]}").Result))
                    {
                        DanbooruTags[] foundTags = JsonConvert.DeserializeObject<DanbooruTags[]>(reader.ReadToEndAsync().Result);

                        for (int j = 0; j < foundTags.Length; j++)
                        {
                            tags.Add(foundTags[j].consequent_name);
                            tags.Add(foundTags[j].antecedent_name);
                        }
                    }
                }
            }

            return tags.ToArray();
        }

        public static async Task<YukiImage[]> GetImages(ImageType type, string[] tags, string[] blacklistedTags, bool forceExplicit)
        {
            IImageSearcherObject search = default;

            List<string> blacklist = new List<string>();

            switch (type)
            {
                case ImageType.Danbooru:
                    search = new DanbooruImageSearch();
                    break;
                case ImageType.Gelbooru:
                    search = new GelbooruImageSearch();
                    break;
                case ImageType.E621:
                    search = new E621ImageSearch();
                    break;
                case ImageType.Rule34:
                    search = new Rule34ImageSearch();
                    break;
            }

            blacklist.AddRange(defaultBlackList);

            if (blacklistedTags != null)
            {
                blacklist.AddRange(blacklistedTags);
            }

            return await search.GetImages(tags, blacklist.ToArray(), forceExplicit);
        }

        public static async Task<YukiImage> GetImage(ImageType type, string[] tags, string[] blacklistedTags, bool forceExplicit)
        {
            YukiImage[] images = await GetImages(type, tags, blacklistedTags, forceExplicit);
            
            return images[new Random().Next(images.Length)];
        }

        public static async Task<YukiImage> GetAnimeImage(string[] searchedTags, string[] blacklistedTags, bool forceExplicit)
        {
            List<string> blacklist = new List<string>();

            blacklist.AddRange(defaultBlackList);

            if (blacklistedTags != null)
            {
                blacklist.AddRange(blacklistedTags);
            }

            string[] tags = GetTags(searchedTags);

            List<YukiImage> images = new List<YukiImage>();

            images.AddRange(await GetImages(ImageType.Danbooru, searchedTags, blacklistedTags, forceExplicit));
            images.AddRange(await GetImages(ImageType.Gelbooru, searchedTags, blacklistedTags, forceExplicit));
            
            return images[new Random().Next(images.Count)];
        }

        public static async Task<T[]> FetchImages<T>(string pageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "YukiBot");

                using (StreamReader reader = new StreamReader(await client.GetStreamAsync(pageUrl)))
                {
                    return JsonConvert.DeserializeObject<T[]>(await reader.ReadToEndAsync());
                }
            }
        }
    }
}
