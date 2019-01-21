using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Yuki.Bot.Misc.Extensions
{
    public static class StringHelper
    {
        public static bool IsImage(this string url)
        {
            char[] chars = url.ToCharArray();

            if (url.Length > 3 && url.Substring(url.Length - 3).Select(x => char.IsLetterOrDigit(x)).Any(x => x == false))
                return false;
            string targetExtension = Path.GetExtension(url).Replace(".", "");
            if (String.IsNullOrEmpty(targetExtension))
                return false;
            else
                targetExtension = targetExtension.ToLowerInvariant();

            List<string> recognisedImageExtensions = new List<string>() { "jpeg", "jpg", "png", "gif" };

            foreach (string extension in recognisedImageExtensions)
                if (extension.Equals(targetExtension))
                    return true;
            return false;
        }

        public static bool IsUrl(this string txt)
            => Uri.TryCreate(txt, UriKind.Absolute, out Uri uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static string GetArgs(this string args, string key, char query)
        {
            int iqs = args.IndexOf(query);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1
                    ? args.Substring(iqs + 1) : string.Empty)[key];
        }

        public static TimeSpan? GetTimeSpanFromString(this string str)
        {
            int d = 0;
            int h = 0;
            int m = 0;
            string[] _str = Regex.Split(str, @"\s");
            for (int i = 0; i < _str.Length; i++)
            {
                if (!IsValidTimeSpanString(_str[i]))
                    return null;
                char[] c = _str[i].ToCharArray();
                if (int.TryParse(_str[i].Remove(_str[i].Length - 1), out int j))
                {
                    switch (c[c.Length - 1])
                    {
                        case 'd':
                            d += j;
                            break;
                        case 'h':
                            h += j;
                            break;
                        case 'm':
                            m += j;
                            break;
                    }
                }
                else
                    return null;
            }
            return new TimeSpan(d, h, m, 0);
        }

        private static bool IsValidTimeSpanString(this string str)
            => str.Last() == 'd' || str.Last() == 'h' || str.Last() == 'm';

        public static string CapitalizeFirst(this string str)
        {
            string[] split = str.Split(' ');

            for (int i = 0; i < split.Length; i++)
            {
                char[] c = split[i].ToCharArray();
                c[0] = char.ToUpper(c[0]);
                split[i] = new string(c);
            }

            return string.Join(" ", split);
        }

        public static double Evaluate(this string expression)
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

            foreach (var t in v)
            {
                if (t.ToString().ToLower() == str.ToLower())
                    return (T)t;
            }
            return default(T);
        }


    }
}
