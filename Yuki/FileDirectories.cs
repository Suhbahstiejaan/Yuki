using System;

namespace Yuki
{
    public static class FileDirectories
    {
        internal static string DataRoot { get; } = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "/data/";
        internal static string LangRoot { get; } = DataRoot + "lang/";
        internal static string LogRoot { get; } = DataRoot + "log/";

        internal static string ConfigFile { get; } = DataRoot + "config.toml";

        internal static string ConfigDB { get; } = DataRoot + "configuration.db";
        internal static string MessageDB { get; } = DataRoot + "messages.db";
    }
}
