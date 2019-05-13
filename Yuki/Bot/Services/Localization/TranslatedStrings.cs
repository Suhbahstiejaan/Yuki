using System.Collections.Generic;

namespace Yuki.Bot.Services.Localization
{
    public class TranslatedStrings
    {
        public string no_results;
        public List<string> eight_ball { get; set; }
        public List<string> message_empty { get; set; }
        public List<string> already_in_vc { get; set; }
        public List<string> not_server { get; set; }
        public List<string> slowmode_disabled { get; set; }
        public List<string> greeting { get; set; }

        //commands
        public List<Data> help { get; set; }
        public List<Data> audio { get; set; }
        public List<Data> owner { get; set; }
        public List<Data> info { get; set; }
        public List<Data> role_info { get; set; }
    }

    public class Data
    {
        public string name { get; set; }
        public string translation { get; set; }
    }
}
