/* Entrypoint */

using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Bot;
using Yuki.Bot.API;
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
            if (!Directory.Exists(FileDirectories.AppDataDirectory + "lang/"))
            {
                if (!Directory.Exists(FileDirectories.AppDataDirectory))
                    Directory.CreateDirectory(FileDirectories.AppDataDirectory);

                Logger.Instance.Write(LogLevel.Warning, "Data not found! Downloading...");

                string downloadUrl;

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                    var json = JsonConvert.DeserializeObject<Github>(httpClient.GetStringAsync("https://api.github.com/repos/VeeThree/Yuki/releases/latest").GetAwaiter().GetResult());
                    
                    downloadUrl = json.assets.FirstOrDefault(asset => asset.name == "lang.zip").browser_download_url;
                }

                using (WebClient client = new WebClient())
                {
                    Logger.Instance.Write(LogLevel.Info, "Downloading from " + downloadUrl);

                    client.DownloadFile(new Uri(downloadUrl), FileDirectories.AppDataDirectory + "lang.zip");

                    ZipFile.ExtractToDirectory(FileDirectories.AppDataDirectory + "lang.zip", FileDirectories.AppDataDirectory);

                    File.Delete(FileDirectories.AppDataDirectory + "lang.zip");
                }
            }

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