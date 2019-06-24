using System;

namespace Yuki.Data.Objects
{
    public class Language
    {
        public string Code { get; set; }

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
        /* Commands */
        public string command_avatar_desc { get; set; }
        public string command_avatar_usage { get; set; }

        public string command_ping_desc { get; set; }
        public string command_ping_usage { get; set; }

        public string command_repeat_desc { get; set; }
        public string command_repeat_usage { get; set; }

        public string command_cry_desc { get; set; }
        public string command_cry_usage { get; set; }

        public string command_cuddle_desc { get; set; }
        public string command_cuddle_usage { get; set; }

        public string command_kiss_desc { get; set; }
        public string command_kiss_usage { get; set; }

        public string command_hug_desc { get; set; }
        public string command_hug_usage { get; set; }

        public string command_lewd_desc { get; set; }
        public string command_lewd_usage { get; set; }

        public string command_lick_desc { get; set; }
        public string command_lick_usage { get; set; }

        public string command_nom_desc { get; set; }
        public string command_nom_usage { get; set; }

        public string command_owo_desc { get; set; }
        public string command_owo_usage { get; set; }

        public string command_pat_desc { get; set; }
        public string command_pat_usage { get; set; }

        public string command_pout_desc { get; set; }
        public string command_pout_usage { get; set; }

        public string command_rem_desc { get; set; }
        public string command_rem_usage { get; set; }

        public string command_slap_desc { get; set; }
        public string command_slap_usage { get; set; }

        public string command_smug_desc { get; set; }
        public string command_smug_usage { get; set; }

        public string command_stare_desc { get; set; }
        public string command_stare_usage { get; set; }

        public string command_tickle_desc { get; set; }
        public string command_tickle_usage { get; set; }

        public string command_8ball_desc { get; set; }
        public string command_8ball_usage { get; set; }

        public string command_serverinfo_desc { get; set; }
        public string command_serverinfo_usage { get; set; }

        public string command_vote_desc { get; set; }
        public string command_vote_usage { get; set; }

        public string command_viewpoll_desc { get; set; }
        public string command_viewpoll_usage { get; set; }

        public string command_createpoll_desc { get; set; }
        public string command_createpoll_usage { get; set; }

        public string command_rroulette_desc { get; set; }
        public string command_rroulette_usage { get; set; }

        public string command_config_desc { get; set; }
        public string command_config_usage { get; set; }


        /* Strings */
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

        public string serverinfo_owner { get; set; }
        public string serverinfo_verification_level { get; set; }
        public string serverinfo_region { get; set; }
        public string serverinfo_categories { get; set; }
        public string serverinfo_channels { get; set; }
        public string serverinfo_channels_text { get; set; }
        public string serverinfo_channels_voice { get; set; }
        public string serverinfo_members { get; set; }
        public string serverinfo_online { get; set; }
        public string serverinfo_roles { get; set; }
        public string serverinfo_roles_view { get; set; }
        public string serverinfo_created { get; set; }

        public string eightball_response_title { get; set; }
        public string eightball_response_1 { get; set; }
        public string eightball_response_2 { get; set; }
        public string eightball_response_3 { get; set; }
        public string eightball_response_4 { get; set; }
        public string eightball_response_5 { get; set; }
        public string eightball_response_6 { get; set; }
        public string eightball_response_7 { get; set; }
        public string eightball_response_8 { get; set; }
        public string eightball_response_9 { get; set; }
        public string eightball_response_10 { get; set; }
        public string eightball_response_11 { get; set; }
        public string eightball_response_12 { get; set; }
        public string eightball_response_13 { get; set; }
        public string eightball_response_14 { get; set; }
        public string eightball_response_15 { get; set; }
        public string eightball_response_16 { get; set; }
        public string eightball_response_17 { get; set; }
        public string eightball_response_18 { get; set; }
        public string eightball_response_19 { get; set; }
        public string eightball_response_20 { get; set; }

        public string config_title { get; set; }
        public string config_setting_join_leave { get; set; }
        public string config_setting_commands { get; set; }
        public string config_setting_assignable_roles { get; set; }
        public string config_setting_mute { get; set; }
        public string config_setting_scramblr { get; set; }
        public string config_setting_nsfw { get; set; }
        public string config_setting_prefix { get; set; }
        public string config_setting_warnings { get; set; }
        public string config_setting_log { get; set; }
    }
}
