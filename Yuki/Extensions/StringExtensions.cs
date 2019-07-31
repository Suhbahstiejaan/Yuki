using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Yuki.Extensions
{
    public static class StringExtensions
    {
        public static bool IsUrl(this string url)
            => Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static bool IsImage(this string url)
        {
            /*char[] chars = url.ToCharArray();

            if (url.Length > 3 && url.Substring(url.Length - 3).Select(x => char.IsLetterOrDigit(x)).Any(x => x == false))
                return false;
            string targetExtension = Path.GetExtension(url).Replace(".", "");
            if (String.IsNullOrEmpty(targetExtension))
                return false;
            else
                targetExtension = targetExtension.ToLowerInvariant();*/

            List<string> recognisedImageExtensions = new List<string>() { ".jpeg", ".jpg", ".png", ".gif" };

            foreach (string extension in recognisedImageExtensions)
            {
                if (extension.Equals(Path.GetExtension(url)))
                {
                    Console.WriteLine(extension);

                    return true;
                }
            }
            return false;
        }

        public static Discord.Color AsColor(this string hex)
        {
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(hex);

            return new Discord.Color(col.R, col.G, col.B);
        }

        public static double Calculate(this string expression)
        {
            DataTable table = new DataTable();

            table.Columns.Add("expression", string.Empty.GetType(), expression);

            DataRow row = table.NewRow();

            table.Rows.Add(row);

            return double.Parse((string)row["expression"]);
        }

        public static T GetEnum<T>(this string str)
        {
            Array v = Enum.GetValues(typeof(T));

            foreach (object t in v)
            {
                if (t.ToString().ToLower() == str.ToLower())
                {
                    return (T)t;
                }
            }
            return default;
        }
    }
}
