using System;
using System.IO;
using System.Net;

namespace Yuki.Bot.Common
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        Success,
        DiscordNet
    }

    public class Logger
    {
        /* Private */
        private string logFileName;
        private string LogDirectory = FileDirectories.AppDataDirectory + "/logs/";

        private StreamWriter log;
        private bool isDisposed;

        /* Public */
        public static Logger Instance { get; private set; } = new Logger();
        
        public Logger()
        {
            Instance = this;

            logFileName = "latest.log";


            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            if (File.Exists(LogDirectory + "latest.log"))
                File.Delete(LogDirectory + "latest.log");
        }

        public void Write(LogLevel severity, Exception e)
            => Write(severity, e.ToString());

        public void Write(LogLevel severity, string message, bool newline = true)
        {
            if (isDisposed)
                return;

            string logTime = "[" + DateTime.Now.ToLongTimeString() + "]";
            string logText = " [" + Enum.GetName(typeof(LogLevel), severity) + "]: " + message;

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
                
                YukiClient.Instance.Restart(1);
            }
        }

        public void SetLoggingDirectory(string directory)
            => LogDirectory = (directory.EndsWith("\\") ? directory : directory + "\\");

        public void WriteCrashLog()
            => File.WriteAllText(LogDirectory + "crash_" + DateTime.Now.ToFileTime() + ".log", new StreamReader(LogDirectory + logFileName).ReadToEnd());

        public void Close()
        {
            log.Dispose();
            isDisposed = true;
        }
    }
}