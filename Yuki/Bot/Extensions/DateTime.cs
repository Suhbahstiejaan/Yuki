using System;

namespace Yuki.Bot.Misc.Extensions
{
    public static class DateTime_
    {
        public static string YukiDateTimeString(this DateTime dateTime)
            => dateTime.DayOfWeek + ", " + dateTime.ToString("MMMM") + " " + dateTime.Day + ", " + " " + dateTime.Year + " @ " + dateTime.ToString("h:mm tt");
    }
}
