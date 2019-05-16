using Nett;
using System.Collections.Generic;

namespace Yuki.Data
{
    public class Config
    {
        public string default_lang { get; set; }
        public List<string> prefix { get; set; }
        public int playing_seconds { get; set; }
        public int command_timeout_seconds { get; set; }
        
        public List<ulong> owners { get; set; }
        
        public string token { get; set; }
        public string cat_api { get; set; }
        public string encryption_key { get; set; }
        
        public List<string> blacklist { get; set; }

        private static Config Instance;

        public static Config GetConfig(bool reload = false)
        {
            if(Instance == null || reload)
            {
                Instance = Toml.ReadFile<Config>(FileDirectories.ConfigFile);
            }

            return Instance;
        }
    }
}
