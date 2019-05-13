using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Discord;
using Newtonsoft.Json;
using Yuki.Bot.Common;
using Yuki.Bot.Misc;

namespace Yuki.Bot.Entity
{
    public class Config
    {
        private static Config _creds;

        public string Token = "";
        public string CatApiKey = "API_KEY";
        public string FirebaseKey = "FIREBASE_KEY";
        public string EncryptionKey = "ENCRYPT_KEY";
        public int StatusMessageSeconds = 300;
        public ulong[] OwnerIds = { 1234567890 };

        public static Config Get()
        {
            try
            {
                if(_creds == null)
                    _creds = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FileDirectories.AppDataDirectory + "config.json"));
            }
            catch (Exception)
            {
                if(!File.Exists(FileDirectories.AppDataDirectory + "config_example.json"))
                    File.WriteAllText(FileDirectories.AppDataDirectory + "config_example.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));

                Logger.Instance.Write(LogLevel.Warning, "Config file doesn't exist! An example has been downloaded for you.");
            }

            return _creds;
        }

        public bool IsOwner(IUser u) => OwnerIds.Contains(u.Id);
    }
}
