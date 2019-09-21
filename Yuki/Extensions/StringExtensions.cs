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

        public static int LevenshteinDistance(this string source1, string source2) //O(n*m)
        {
            int source1Length = source1.Length;
            int source2Length = source2.Length;
            

            int[,] matrix = new int[source1Length + 1, source2Length + 1];


            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
            {
                return source2Length;
            }

            if (source2Length == 0)
            {
                return source1Length;
            }

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (int i = 0; i <= source1Length; matrix[i, 0] = i++) { }

            for (int j = 0; j <= source2Length; matrix[0, j] = j++) { }


            // Calculate rows and collumns distances
            for (int i = 1; i <= source1Length; i++)
            {
                for (int j = 1; j <= source2Length; j++)
                {
                    int cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                                    matrix[i - 1, j - 1] + cost);
                }
            }

            // return result
            return matrix[source1Length, source2Length];
        }
    }
}
