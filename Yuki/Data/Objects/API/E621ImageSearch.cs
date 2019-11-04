using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Core;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Data.Objects.API
{
    public class E621ImageSearch : IImageSearcherObject
    {
        public const int limit = 500;

        public string url { get; set; } = "https://e621.net/post/index.json?tags=";

        public async Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            string _url = url;

            _url += string.Join("+", tags);
            _url += $"&limit={limit}";

            List<YukiImage> imgs = ImageSearch.GetImages(tags, ImageType.E621);

            if (imgs == default || imgs.Count < 1)
            {
                E621[] e621 = await ImageSearch.FetchImages<E621>(_url);

                for (int i = 0; i < e621.Length; i++)
                {
                    string[] imgTags = e621[i].tags.Split(' ');

                    if (!imgTags.ToList().Any(tag => blacklisted.Contains(tag)))
                    {
                        YukiImage img = new YukiImage();

                        img.type = ImageType.E621;
                        img.isExplicit = (e621[i].rating == "e" || e621[i].rating == "q");
                        img.url = e621[i].file_url;
                        img.page = $"https://e621.net/post/show/{e621[i].id}";
                        img.tags = imgTags;
                        img.source = e621[i].source;

                        ImageSearch.CacheImage(img);
                    }
                }

                imgs = ImageSearch.GetImages(tags, ImageType.E621);

                if (imgs == default || imgs.Count < 1)
                {
                    return default;
                }
            }

            return imgs[new Random().Next(imgs.Count)];
        }
    }
}
