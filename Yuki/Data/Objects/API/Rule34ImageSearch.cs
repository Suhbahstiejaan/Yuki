using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yuki.API;
using Yuki.Core;
using Yuki.Data.Objects.API.ImageObject;

namespace Yuki.Data.Objects.API
{
    public class Rule34ImageSearch : IImageSearcherObject
    {
        public const int limit = 500;

        public string url { get; set; } = "https://rule34.xxx/index.php?page=dapi&json=1&s=post&q=index&tags=";

        public async Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit)
        {
            string _url = url;

            _url += string.Join("+", tags);
            _url += $"&limit={limit}";

            List<YukiImage> imgs = ImageSearch.GetImages(tags, ImageType.Rule34);

            if (imgs == default || imgs.Count < 1)
            {
                Rule34[] r34 = await ImageSearch.FetchImages<Rule34>(_url);

                for (int i = 0; i < r34.Length; i++)
                {
                    string[] imgTags = r34[i].tags.Split(' ');

                    if (!imgTags.ToList().Any(tag => blacklisted.Contains(tag)))
                    {
                        YukiImage img = new YukiImage();

                        img.type = ImageType.Rule34;
                        img.isExplicit = true;
                        img.url = $"https://us.rule34.xxx/images/{r34[i].directory}/{r34[i].image}";
                        img.page = "https://rule34.xxx/index.php?page=post&s=view&id=" + r34[i].id;
                        img.tags = imgTags;
                        img.source = "https://rule34.xxx/index.php?page=post&s=view&id=" + r34[i].id;

                        ImageSearch.CacheImage(img);
                    }
                }

                imgs = ImageSearch.GetImages(tags, ImageType.Rule34);

                if (imgs == default || imgs.Count < 1)
                {
                    return default;
                }
            }

            return imgs[new YukiRandom().Next(imgs.Count)];
        }
    }
}
