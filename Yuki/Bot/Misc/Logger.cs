using System;
using System.IO;
using System.Net;
using Yuki.Bot.Entities;

namespace Yuki.Bot.Misc
{
    public enum LogSeverity
    {
        Info,
        Debug,
        Warning,
        Error
    }

    public class Logger
    {
        /* Private */
        private static Logger _loggerInstance;
        private static int _incidentNums;

        private string logFileName;
        private string LogDirectory = FileDirectories.AppDataDirectory + "\\logs\\";

        private StreamWriter log;

        /* Public */
        public static Logger GetLoggerInstance()
        {
            if (_loggerInstance is null)
                _loggerInstance = new Logger();

            return _loggerInstance;
        }

        public string SendNotificationFromFirebaseCloud(string notifText, string shortDesc, string incidentNo, string desc)
        {
            string notifTitle = "Yuki";

            if (System.Diagnostics.Debugger.IsAttached)
                notifTitle += " [Debug]";

            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=" + YukiClient.Instance.Credentials.FirebaseKey);
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strNJson = @"{
                    ""to"": ""/topics/ServiceNow"",
                    ""data"": {
                        ""ShortDesc"": """ + shortDesc + @""",
                        ""IncidentNo"": """ + incidentNo + @""",
                        ""Description"": """ + desc + @"""
},
  ""notification"": {
                ""title"": """ + notifTitle + @""",
    ""text"": """ + notifText + @""",
""sound"":""default""
  }
        }";
                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        public Logger()
        {
            _loggerInstance = this;
            logFileName = "latest.log";

            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            if (File.Exists(LogDirectory + "latest.log"))
                File.Delete(LogDirectory + "latest.log");
        }

        public void Write(LogSeverity severity, Exception e)
            => Write(severity, e.ToString());

        public void Write(LogSeverity severity, string message)
        {
            string logText = "[" + DateTime.Now.ToLongTimeString() + "] [" + Enum.GetName(typeof(LogSeverity), severity) + "]: " + message;

            ConsoleColor c = Console.ForegroundColor;

            switch (severity)
            {
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(logText);

            if (log == null)
                log = new StreamWriter(LogDirectory + logFileName);

            log.WriteLine(logText);
            log.Flush();

            Console.ForegroundColor = c;

            if (severity == LogSeverity.Error)
            {
                Close();
                WriteCrashLog();
                
                if(YukiClient.Instance.Credentials.FirebaseKey != null)
                {
                    _incidentNums++;
                    try
                    {
                        SendNotificationFromFirebaseCloud("An error has occurred", "Error", "INC_" + _incidentNums, message);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Couldnt send notification! " + e);
                        _incidentNums--;
                    }
                }
            }
        }

        public void SetLoggingDirectory(string directory) => LogDirectory = (directory.EndsWith("\\") ? directory : directory + "\\");

        public void WriteCrashLog()
        {
            File.WriteAllText(LogDirectory + "crash_" + DateTime.Now.ToFileTime() + ".log", new StreamReader(LogDirectory + logFileName).ReadToEnd());
        }

        public void Close()
            => log.Dispose();
    }
}