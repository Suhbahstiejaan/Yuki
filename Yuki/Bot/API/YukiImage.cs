using Discord;
using System.Collections.Generic;

namespace Yuki.Bot.API
{
    public class YukiImage
    {
        public string Url { get; set; }
        public string Source { get; set; }
        public int Rating { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static List<YukiImage> GetBatchImages(IChannel channel, string term = "", bool isNsfw = false)
        {
            List<YukiImage> images = new List<YukiImage>();

            images.AddRange(GetBatchAnimeImages(channel, term, isNsfw));

            images.AddRange(Rule34.Rule34.GetImages(term).Result);
            images.AddRange(E621.E621.GetImages(term).Result);

            return images;
        }

        public static List<YukiImage> GetBatchAnimeImages(IChannel channel, string term = "", bool isNsfw = false)
        {
            List<YukiImage> images = new List<YukiImage>();
            List<YukiImage> _images = new List<YukiImage>();

            _images = Gelbooru.Gelbooru.GetImages(channel, term, isNsfw).Result;

            if (_images != null)
            {
                images.AddRange(_images);
                _images.Clear();
            }

            if (term.Split(' ').Length <= 2 || term.Split('+').Length <= 2)
            {
                _images = Danbooru.Danbooru.GetImages(channel, term, isNsfw).Result;

                if (_images != null)
                {
                    images.AddRange(_images);
                    _images.Clear();
                }
            }

            return images;
        }
    }
}
