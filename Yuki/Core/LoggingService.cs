using System;
using System.IO;

namespace Yuki.Core
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        SilentError,
        Success,
        DiscordNet
    }

    public class LoggingService
    {
        /* Private */
        private string logFileName = "latest.log";
        public static string LogDirectory = YukiBot.DataDirectoryRootPath + "logs/";

        private StreamWriter log;
        private bool isDisposed;

        public void Write(LogLevel logLevel, object o)
        {
            Write(logLevel, o.ToString());
        }

        public void Write(LogLevel severity, string message, bool newline = true)
        {
            try
            {
                if (isDisposed)
                    return;

                string logLevel = Enum.GetName(typeof(LogLevel), severity);

                if (logLevel == "SilentError")
                    logLevel = "Error";

                string logTime = "[" + DateTime.Now.ToLongTimeString() + "]";
                string logText = " [" + logLevel + "]: " + message;

                ConsoleColor c = Console.ForegroundColor;

                Console.Write(logTime);

                switch (severity)
                {
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                    case LogLevel.SilentError:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogLevel.DiscordNet:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                }

                if (newline)
                    Console.WriteLine(logText);
                else
                    Console.Write(logText);

                if (log == null)
                    log = new StreamWriter(LogDirectory + logFileName);

                if (newline)
                    log.WriteLine(logText);
                else
                    log.Write(logText);

                log.Flush();

                Console.ForegroundColor = c;

                if (severity == LogLevel.Error)
                {
                    Close();
                    WriteCrashLog();
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        public void SetLoggingDirectory(string directory)
        {
            LogDirectory = (directory.EndsWith("\\") ? directory : directory + "\\");
        }

        public void WriteCrashLog()
        {
            File.WriteAllText(LogDirectory + "crash_" + DateTime.Now.ToFileTime() + ".log", new StreamReader(LogDirectory + logFileName).ReadToEnd());
        }

        public void Close()
        {
            log.Dispose();
            isDisposed = true;
        }
    }
}
