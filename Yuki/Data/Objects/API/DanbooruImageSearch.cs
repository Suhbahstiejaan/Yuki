using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Core;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Data.Objects.API
{
    public class DanbooruImageSearch : IImageSearcherObject
    {
        public const int limit = 500;

        public string url { get; set; } = "https://danbooru.donmai.us/posts.json?utf8=✓&tags=";

        public async Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            YukiImage[] images = await GetImages(tags, blacklisted, forceExplicit);

            return images[new Random().Next(images.Length)];
        }

        public async Task<YukiImage[]> GetImages(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            string _url = url;

            /* Danbooru limits searches to 2 tags */
            if(tags != null)
            {
                _url += string.Join("+", tags.Take(2));
            }

            _url += $"&limit={limit}";

            List<YukiImage> images = new List<YukiImage>();

            Danbooru[] danbooru = await ImageSearch.FetchImages<Danbooru>(_url);
            
            for (int i = 0; i < danbooru.Length; i++)
            {
                string[] imgTags = danbooru[i].tag_string.Split(' ');

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

                img.type = ImageType.Danbooru;
                img.isExplicit = (danbooru[i].rating == "e" || danbooru[i].rating == "q");
                img.url = danbooru[i].large_file_url;
                img.page = $"https://danbooru.donmai.us/posts/{danbooru[i].id}";
                img.tags = imgTags;
                img.source = danbooru[i].source;

                images.Add(img);

            }

            return images.ToArray();
        }
    }
}
