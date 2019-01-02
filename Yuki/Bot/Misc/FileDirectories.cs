using System;
using System.Runtime.InteropServices;

namespace Yuki.Bot.Misc
{
    public class FileDirectories
    {
        public static string AppDataDirectory {
            get
            {
                return (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : "/home") + @"/yuki/";
            }
        }
        public static string DatabaseCopyPath {
            get
            {
                return AppDataDirectory + "yuki_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".db";
            }
        }

        public static string Database {
            get
            {
                return AppDataDirectory + "yuki.db";
            }
        }
    }
}
