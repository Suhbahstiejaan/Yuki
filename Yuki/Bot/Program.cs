/* Entrypoint */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Yuki.Bot;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Services;

namespace Yuki
{
    public class Program
    {
        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
        private delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        private static Logger _log = Logger.GetLoggerInstance();


        public static void Main() => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);

            SetConsoleCtrlHandler(handler, true);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Yuki " + Localizer.YukiStrings.version;

            _log.Write(LogSeverity.Info, "Setting up database/applying any needed migrations...");
            YukiContextFactory.DatabaseSetupOrMigrate();

            if (!Directory.Exists(JSONManager.jsonPath))
                Directory.CreateDirectory(JSONManager.jsonPath);

            _log.Write(LogSeverity.Info, "Retrieving messages...");
            MessageCache.LoadCacheFromFile();
            await YukiClient.Instance.Login();

            await Task.Delay(-1);
        }

        static bool ConsoleEventCallback(int eventType)
        {
            //check if the console window is closing
            if (eventType == 2)
            {
                YukiClient.Instance.DiscordClient.LogoutAsync();
                YukiClient.Instance.DiscordClient.StopAsync();
                YukiClient.Instance.DiscordClient.Dispose();

                Console.WriteLine("Backing up database...");

                if(File.Exists(FileDirectories.DatabaseCopyPath))
                    File.Delete(FileDirectories.DatabaseCopyPath);
                File.Copy(FileDirectories.Database, FileDirectories.DatabaseCopyPath);
                
                Console.WriteLine("Backing up message cache...");
                MessageCache.DumpCacheToFile();

                Thread.Sleep(1000); //Sleep for 1s to give thing enough time to back up.
            }
            return false;
        }
    }
}