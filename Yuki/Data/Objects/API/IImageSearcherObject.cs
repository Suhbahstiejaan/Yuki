using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuki.Data.Objects.API
{
    public interface IImageSearcherObject
    {
        string url { get; set; }

        Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit);
    }
}
