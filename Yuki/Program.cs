/* Entrypoint */

using System;
using System.IO;
using System.Threading.Tasks;
using Yuki.Bot;
using Yuki.Bot.Common;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Services;
using Yuki.Bot.Services.Localization;

namespace Yuki
{
    public class Program
    {
        public static void Main() => MainAsync().GetAwaiter().GetResult();

        public static async Task MainAsync()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "Yuki " + Localizer.YukiStrings.version;

            Logger.Instance.Write(LogLevel.Info, "Setting up database/applying any needed migrations...");
            YukiContextFactory.DatabaseSetupOrMigrate();

            if (!Directory.Exists(JSONManager.jsonPath))
                Directory.CreateDirectory(JSONManager.jsonPath);

            Logger.Instance.Write(LogLevel.Info, "Retrieving messages...");

            MessageCache.LoadCacheFromFile();
            
            await YukiClient.Instance.LoginAsync();

            Logger.Instance.Write(LogLevel.Info, "Verifying command translations...");
            Localizer.VerifyCommands();

            await Task.Delay(-1);
        }
    }
}