using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Yuki.Bot.Common
{
    public class FileDirectories
    {
        public static string AppDataDirectory
            => AppDomain.CurrentDomain.BaseDirectory.Replace(@"\", "/") + @"data/";

        public static string DatabaseCopyPath
            => AppDataDirectory + "yuki_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".db";

        public static string Database
            => AppDataDirectory + "yuki.db";
    }
}
