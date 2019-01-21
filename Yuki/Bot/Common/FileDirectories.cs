using System;
using System.Runtime.InteropServices;

namespace Yuki.Bot.Common
{
    public class FileDirectories
    {
        public static string AppDataDirectory
            => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") : "/home") + @"/yuki/";

        public static string DatabaseCopyPath
            => AppDataDirectory + "yuki_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".db";

        public static string Database
            => AppDataDirectory + "yuki.db";
    }
}
