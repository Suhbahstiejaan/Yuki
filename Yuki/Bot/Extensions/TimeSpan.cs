using System;

namespace Yuki.Bot.Misc.Extensions
{
    public static class TimeSpan_
    {
        public static string PrettyTime(this TimeSpan span)
        {
            if (span.Days > 0)
                return span.ToString(@"d\:hh\:mm\:ss");

            if (span.Hours > 0)
                return span.ToString(@"hh\:mm\:ss");

            return span.ToString(@"mm\:ss");
        }
    }
}
