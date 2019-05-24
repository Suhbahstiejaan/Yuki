using System;
using System.Collections.Generic;

namespace Yuki.Data.Objects
{
    public class Language
    {
        public string Code { get; set; }

        public List<CommandTranslation> Translations { get; set; }
        public TranslatedStrings Strings { get; set; }

        public string GetString(string stringName)
        {
            string name;

            try
            {
                name = (string)Strings.GetType().GetProperty(stringName).GetValue(Strings, null) ?? "";
            }
            catch (Exception)
            {
                name = stringName;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return stringName;
            }

            return name;
        }
    }

    public class CommandTranslation
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Usage { get; set; }
    }

    public class TranslatedStrings
    {
        public string ping_pinging { get; set; }
        public string ping_pong { get; set; }
        public string ping_latency { get; set; }
        public string ping_api_latency { get; set; }

        public string avatar_user_avatar { get; set; }

        public string interactive_repeat_send_message { get; set; }

        public string rammoe_cry { get; set; }
        public string rammoe_cuddle { get; set; }
        public string rammoe_cuddle_alt { get; set; }
        public string rammoe_hug { get; set; }
        public string rammoe_hug_alt { get; set; }
        public string rammoe_kiss { get; set; }
        public string rammoe_kiss_alt { get; set; }
        public string rammoe_lewd { get; set; }
        public string rammoe_lewd_alt { get; set; }
        public string rammoe_lick { get; set; }
        public string rammoe_lick_alt { get; set; }
        public string rammoe_nom { get; set; }
        public string rammoe_nom_alt { get; set; }
        public string rammoe_nyan { get; set; }
        public string rammoe_owo { get; set; }
        public string rammoe_pat_alt { get; set; }
        public string rammoe_pout { get; set; }
        public string rammoe_rem { get; set; }
        public string rammoe_slap { get; set; }
        public string rammoe_slap_alt { get; set; }
        public string rammoe_smug { get; set; }
        public string rammoe_smug_alt { get; set; }
        public string rammoe_stare { get; set; }
        public string rammoe_stare_alt { get; set; }
        public string rammoe_tickle { get; set; }
        public string rammoe_tickle_alt { get; set; }
        public string rammoe_triggered { get; set; }
        public string rammoe_triggered_alt { get; set; }
        public string rammoe_nsfw_gtn { get; set; }
        public string rammoe_potato { get; set; }
    }
}
