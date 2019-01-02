/* Entrypoint */

using System;
using System.IO;
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
        public static void Main() => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Yuki " + Localizer.YukiStrings.version;

            Logger.GetLoggerInstance().Write(LogSeverity.Info, "Setting up database/applying any needed migrations...");
            YukiContextFactory.DatabaseSetupOrMigrate();

            if (!Directory.Exists(JSONManager.jsonPath))
                Directory.CreateDirectory(JSONManager.jsonPath);

            Logger.GetLoggerInstance().Write(LogSeverity.Info, "Retrieving messages...");
            MessageCache.LoadCacheFromFile();
            await YukiClient.Instance.Login();

            await Task.Delay(-1);
        }
    }
}