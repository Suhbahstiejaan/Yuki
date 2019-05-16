﻿using System;
using System.IO;

namespace Yuki.Services
{
    public enum LogLevel
    {
        DiscordNet,
        Debug,
        Info,
        Error,
        Warning,
        Status
    }

    public static class LoggingService
    {
        private static string latestLogFile;
        
        static LoggingService()
        {
            if(!Directory.Exists(FileDirectories.LogRoot))
            {
                Directory.CreateDirectory(FileDirectories.LogRoot);
            }

            if(File.Exists(FileDirectories.LogRoot + "latest.log"))
            {
                File.Delete(FileDirectories.LogRoot + "latest.log");
            }

            latestLogFile = FileDirectories.LogRoot + "latest.log";

            File.Create(latestLogFile).Dispose();
        }

        public static void Write(LogLevel logLevel, object o)
        {
            /* only print debug info if its a prerelease or dev build */
            if((logLevel == LogLevel.Debug) && (Version.ReleaseType != ReleaseType.Development && Version.ReleaseType != ReleaseType.PreRelease))
            {
                return;
            }

            string line = $"[{DateTime.Now.ToShortTimeString()}] [{logLevel.ToString()}] {o.ToString()}";

            using (FileStream file = new FileStream(latestLogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine(line);
                    sw.Flush();
                }
            }

            Console.WriteLine(line);

            if(logLevel == LogLevel.Error)
            {
                File.Copy(latestLogFile, FileDirectories.LogRoot + $"crash_{DateTime.Now.ToLongDateString()}.log");

                if(Version.ReleaseType != ReleaseType.Development)
                {
                    //Do something
                }
            }
        }
    }
}
