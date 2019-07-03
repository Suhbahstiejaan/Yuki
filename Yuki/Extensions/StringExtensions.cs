using System;
using System.Data;
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
