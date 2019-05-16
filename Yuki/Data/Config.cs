using Nett;
using System.Collections.Generic;

namespace Yuki.Data
{
    public class Config
    {
        public string default_lang { get; set; }
        public List<string> prefix { get; set; }
        public int playing_seconds { get; set; }
        public bool notifications { get; set; }

        public List<ulong> owners { get; set; }
        
        public string token { get; set; }
        public string cat_api { get; set; }
        public string encryption_key { get; set; }
        public string firebase { get; set; }

        public List<string> blacklist { get; set; }
        public List<string> rammoe_exclude { get; set; }
        
        public static Config GetConfig()
        {
            return Toml.ReadFile<Config>(FileDirectories.ConfigFile);
        }
    }
}
