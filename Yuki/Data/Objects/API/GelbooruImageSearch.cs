using System;
using System.Collections.Generic;
using System.Linq;
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
            YukiImage[] images = await GetImages(tags, blacklisted, forceExplicit);

            return images[new Random().Next(images.Length)];
        }

        public async Task<YukiImage[]> GetImages(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            string _url = url;
            
            if (tags != null)
            {
                _url += string.Join("+", tags);
            }

            _url += $"&limit={limit}";

            List<YukiImage> images = new List<YukiImage>();

            Gelbooru[] gelbooru = await ImageSearch.FetchImages<Gelbooru>(_url);

            for (int i = 0; i < gelbooru.Length; i++)
            {
                string[] imgTags = gelbooru[i].tags.Split(' ');

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

                img.type = ImageType.Gelbooru;
                img.isExplicit = (gelbooru[i].rating == "e" || gelbooru[i].rating == "q");
                img.url = gelbooru[i].file_url;
                img.page = $"https://gelbooru.com/index.php?page=post&s=view&id={gelbooru[i].id}";
                img.tags = imgTags;
                img.source = gelbooru[i].source;

                images.Add(img);
            }

            return images.ToArray();
        }
    }
}
