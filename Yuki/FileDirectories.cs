﻿using System;
using System.IO;

namespace Yuki
{
    public static class FileDirectories
    {
        internal static string DataRoot { get; } = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "data/";
        internal static string AssetRoot { get; } = DataRoot + "assets/";

        internal static string LangRoot { get; } = AssetRoot + "lang/";
        internal static string ImageRoot { get; } = AssetRoot + "images/";
        internal static string LogRoot  { get; } = DataRoot + "log/";
        internal static string PollRoot { get; } = DataRoot + "polls/";
        internal static string TempArchiveRoot { get; } = DataRoot + "tempChannelArchives/";
        internal static string DBRoot { get; } = DataRoot + "databases/";

        internal static string ConfigFile { get; } = DataRoot + "config.toml";
        internal static string StatusMessages { get; } = AssetRoot + "status.txt";

        internal static string SettingsDB { get; } = DBRoot + "settings.db";
        internal static string CacheDB { get; } = DBRoot + "cache.db";

        public static void CheckCreateDirectories()
        {
            if(!Directory.Exists(LangRoot))
            {
                Directory.CreateDirectory(LangRoot);
            }

            if(!Directory.Exists(ImageRoot))
            {
                Directory.CreateDirectory(ImageRoot);
            }

            if(!Directory.Exists(LogRoot))
            {
                Directory.CreateDirectory(LogRoot);
            }

            if(!Directory.Exists(PollRoot))
            {
                Directory.CreateDirectory(PollRoot);
            }

            if(!Directory.Exists(DBRoot))
            {
                Directory.CreateDirectory(DBRoot);
            }
            
            if(!Directory.Exists(TempArchiveRoot))
            {
                Directory.CreateDirectory(TempArchiveRoot);
            }
        }
    }
}
