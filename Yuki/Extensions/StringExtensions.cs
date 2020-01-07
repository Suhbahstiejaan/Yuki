using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace Yuki.Extensions
{
    public static class StringExtensions
    {
        public static bool IsUrl(this string url)
            => Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static bool IsImage(this string url)
        {
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
            hex = hex.TrimStart('#');

            Color col;
            if (hex.Length == 6)
                col = Color.FromArgb(255, // hardcoded opaque
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber));
            else // assuming length of 8
                col = Color.FromArgb(
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
            
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
