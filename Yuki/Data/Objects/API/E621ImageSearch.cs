using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Data.Objects.API
{
    public class E621ImageSearch : IImageSearcherObject
    {
        public const int limit = 500;

        public string url { get; set; } = "https://e621.net/post/index.json?tags=";

        public async Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            YukiImage[] images = await GetImages(tags, blacklisted, forceExplicit);

            return images[new Random().Next(images.Length)];
        }

        public async Task<YukiImage[]> GetImages(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            string _url = url;

            if(tags != null)
            {
                _url += string.Join("+", tags);
            }

            _url += $"&limit={limit}";

            List<YukiImage> images = new List<YukiImage>();

            E621[] e621 = await ImageSearch.FetchImages<E621>(_url);

            for (int i = 0; i < e621.Length; i++)
            {
                string[] imgTags = e621[i].tags.Split(' ');

                bool skip = false;

                for (int j = 0; j < blacklisted.Length; j++)
                {
                    if (imgTags.Contains(blacklisted[j]))
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                {
                    continue;
                }

                YukiImage img = new YukiImage();

                img.type = ImageType.E621;
                img.isExplicit = (e621[i].rating == "e" || e621[i].rating == "q");
                img.url = e621[i].file_url;
                img.page = $"https://e621.net/post/show/{e621[i].id}";
                img.tags = imgTags;
                img.source = e621[i].source;

                images.Add(img);
            }

            return images.ToArray();
        }
    }
}
