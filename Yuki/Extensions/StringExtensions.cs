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

        public static bool LevenshteinAny(this string s, string t)
        {
            foreach(string str in s.Split(' '))
            {
                if(LevenshteinDistance(str.ToLower(), t.ToLower()) > 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static int LevenshteinDistance(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
