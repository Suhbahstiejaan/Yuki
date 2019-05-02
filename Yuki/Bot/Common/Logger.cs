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
        
        public string SendNotificationFromFirebaseCloud(string notifText, string desc)
        {
            string result = "-1";
            string webAddr = "https://fcm.googleapis.com/fcm/send";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(webAddr);

            webRequest.ContentType = "application/json";
            webRequest.Headers.Add(HttpRequestHeader.Authorization, "key=" + YukiClient.Instance.Config.FirebaseKey);
            webRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                string strNJson = @"{
                    ""to"": ""/topics/ServiceNow"",
                    ""data"": {
                        ""Description"": """ + desc + @"""
},
  ""notification"": {
                ""title"": """ + YukiClient.Instance.Client.CurrentUser.Username + @""",
    ""text"": """ + notifText + @""",
""sound"":""default""
  }
        }";
                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                result = streamReader.ReadToEnd();

            return result;
        }

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
                
                if(YukiClient.Instance.Config.FirebaseKey != null)
                {
                    try
                    {
                        SendNotificationFromFirebaseCloud("An error has occurred", message);
                    }
                    catch(Exception e)
                    {
                        Write(LogLevel.Warning, "Couldnt send notification! " + e);
                    }
                }

                YukiClient.Instance.Shutdown(1);
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