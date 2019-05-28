using System;
using System.Collections.Generic;

namespace Yuki.Data.Objects
{
    public class Language
    {
        public string Code { get; set; }

        public List<CommandTranslation> Command { get; set; }
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
        public string only_dm_channel { get; set; }

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

        public string poll_not_found { get; set; }
        public string poll_not_in_server { get; set; }
        public string poll_ended { get; set; }
        public string poll_vote { get; set; }
        public string poll_unknown_item { get; set; }
        public string poll_already_voted { get; set; }
        public string poll_response_recorded { get; set; }

        public string poll_create_creating { get; set; }
        public string poll_create_created { get; set; }
        public string poll_creating_title_str { get; set; }
        public string poll_creating_items_str { get; set; }
        public string poll_create_items_desc { get; set; }
        public string poll_creating_deadline_str { get; set; }
        public string poll_create_show_vote_str { get; set; }
        public string poll_create_show_vote_desc { get; set; }
        public string poll_create_deadline_desc { get; set; }
        public string poll_create_title { get; set; }
        public string poll_create_title_long { get; set; }
        public string poll_create_items { get; set; }
        public string poll_create_items_short { get; set; }
        public string poll_create_deadline { get; set; }
        public string poll_create_deadline_invalid { get; set; }
        public string poll_create_deadline_long { get; set; }
        public string poll_create_allow_view { get; set; }
        public string poll_created_id { get; set; }
    }
}
