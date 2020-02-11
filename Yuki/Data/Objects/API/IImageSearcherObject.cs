using System.Threading.Tasks;

namespace Yuki.Data.Objects.API
{
    public interface IImageSearcherObject
    {
        string url { get; set; }

        Task<YukiImage> GetImage(string[] tags, string[] blacklisted, bool forceExplicit);
        Task<YukiImage[]> GetImages(string[] tags, string[] blacklisted, bool forceExplicit);
    }
}
