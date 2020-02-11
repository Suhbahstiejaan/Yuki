using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Data.Objects.API
{
    public class Rule34ImageSearch : IImageSearcherObject
    {
        public const int limit = 500;

        public string url { get; set; } = "https://rule34.xxx/index.php?page=dapi&json=1&s=post&q=index&tags=";

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

            Rule34[] rule34 = await ImageSearch.FetchImages<Rule34>(_url);

            for (int i = 0; i < rule34.Length; i++)
            {
                string[] imgTags = rule34[i].tags.Split(' ');

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

                img.type = ImageType.Rule34;
                img.isExplicit = true;
                img.url = $"https://us.rule34.xxx/images/{rule34[i].directory}/{rule34[i].image}";
                img.page = "https://rule34.xxx/index.php?page=post&s=view&id=" + rule34[i].id;
                img.tags = imgTags;
                img.source = "https://rule34.xxx/index.php?page=post&s=view&id=" + rule34[i].id;

                images.Add(img);

            }

            return images.ToArray();
        }
    }
}
