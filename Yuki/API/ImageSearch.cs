using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public class ImageSearch
    {
        public static List<YukiImage> CachedImages = new List<YukiImage>();

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

        public static void CacheImage(YukiImage image)
        {
            if(!CachedImages.Any(img => img.url == image.url))
            {
                if(string.IsNullOrWhiteSpace(image.source))
                {
                    image.source = image.page;
                }

                CachedImages.Add(image);
            }
        }

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

        public static List<YukiImage> GetImages(string[] associatedTags, ImageType imgType = ImageType.None)
        {
            List<YukiImage> imgs = new List<YukiImage>();

            string[] tags = GetTags(associatedTags);

            if (associatedTags != null && associatedTags.Length > 0)
            {
                imgs = CachedImages.Where(img => img.tags.Any(tag => tags.Contains(tag))).ToList();

                foreach(YukiImage img in imgs.ToList())
                {
                    if(!tags.Any(tag => img.tags.Contains(tag)))
                    {
                        imgs.Remove(img);
                    }
                }
            }
            else
            {
                imgs = CachedImages;
            }

            if (imgType == ImageType.None)
            {
                return imgs;
            }

            return imgs.Where(img => img.type == imgType).ToList();
        }

        public async Task<YukiImage> GetImage(ImageType type, string[] tags, string[] blacklistedTags, bool forceExplicit)
        {
            IImageSearcherObject search = default;

            List<string> blacklist = new List<string>();

            switch(type)
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

            if(blacklistedTags != null)
            {
                blacklist.AddRange(blacklistedTags);
            }

            return await search.GetImage(tags, blacklist.ToArray(), forceExplicit);
        }

        public async Task<YukiImage> GetHentaiImage(string[] searchedTags, string[] blacklistedTags, bool forceExplicit)
        {
            List<string> blacklist = new List<string>();

            blacklist.AddRange(defaultBlackList);

            if (blacklistedTags != null)
            {
                blacklist.AddRange(blacklistedTags);
            }

            string[] tags = GetTags(searchedTags);

            YukiImage[] images = CachedImages.Where(img => (img.type == ImageType.Danbooru || img.type == ImageType.Gelbooru) &&
                                    img.tags.Any(tag => tags.Contains(tag) && !blacklist.Contains(tag))).ToArray();

            if(images == null || images.Length < 50)
            {
                await GetImage(ImageType.Danbooru, searchedTags, blacklistedTags, forceExplicit);
                await GetImage(ImageType.Gelbooru, searchedTags, blacklistedTags, forceExplicit);

                await GetHentaiImage(searchedTags, blacklistedTags, forceExplicit);
            }

            images = CachedImages.Where(img => (img.type == ImageType.Danbooru || img.type == ImageType.Gelbooru) &&
                                    img.tags.Any(tag => tags.Contains(tag) && !blacklist.Contains(tag))).ToArray();

            return images[new YukiRandom().Next(images.Length)];
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
