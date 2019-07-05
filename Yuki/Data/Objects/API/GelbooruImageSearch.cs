using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Core;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Data.Objects.API
{
    public class GelbooruImageSearch : IImageSearcherObject
    {
        public const int limit = 500;

        public string url { get; set; } = "https://gelbooru.com/index.php?page=dapi&json=1&s=post&q=index&tags=";

        public async Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            string _url = url;

            _url += string.Join("+", tags);
            _url += $"&limit={limit}";


            List<YukiImage> imgs = ImageSearch.GetImages(tags, ImageType.Gelbooru);

            if (imgs == default || imgs.Count < 1)
            {
                Gelbooru[] gelbooru = await ImageSearch.FetchImages<Gelbooru>(_url);

                for (int i = 0; i < gelbooru.Length; i++)
                {
                    string[] imgTags = gelbooru[i].tags.Split(' ');

                    if (!imgTags.ToList().Any(tag => blacklisted.Contains(tag)))
                    {
                        YukiImage img = new YukiImage();

                        img.type = ImageType.Gelbooru;
                        img.isExplicit = (gelbooru[i].rating == "e" || gelbooru[i].rating == "q");
                        img.url = gelbooru[i].file_url;
                        img.page = $"https://gelbooru.com/index.php?page=post&s=view&id={gelbooru[i].id}";
                        img.tags = imgTags;
                        img.source = gelbooru[i].source;

                        ImageSearch.CacheImage(img);
                    }
                }

                imgs = ImageSearch.GetImages(tags, ImageType.Gelbooru);

                Console.WriteLine(imgs.Count);

                if (imgs == default || imgs.Count < 1)
                {
                    return default;
                }
            }

            return imgs[new YukiRandom().Next(imgs.Count)];
        }
    }
}
