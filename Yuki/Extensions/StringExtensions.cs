using System;
using System.IO;
using System.Text;

namespace Yuki.Extensions
{
    public static class StringExtensions
    {
        public static bool IsUrl(this string url)
            => Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static Discord.Color AsColor(this string hex)
        {
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(hex);

            return new Discord.Color(col.R, col.G, col.B);
        }
    }
}
