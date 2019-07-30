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

        public string command_roulette_desc { get; set; }
        public string command_roulette_usage { get; set; }

        public string command_roulette_join_desc { get; set; }
        public string command_roulette_join_usage { get; set; }

        public string command_roulette_start_desc { get; set; }
        public string command_roulette_start_usage { get; set; }
        
        public string command_roulette_kick_desc { get; set; }
        public string command_roulette_kick_usage { get; set; }
        
        public string command_roulette_quit_desc { get; set; }
        public string command_roulette_quit_usage { get; set; }
        
        public string command_roulette_players_desc { get; set; }
        public string command_roulette_players_usage { get; set; }
        
        public string command_toss_desc { get; set; }
        public string command_toss_usage { get; set; }
        
        public string command_rolecol_desc { get; set; }
        public string command_rolecol_usage { get; set; }
        
        public string command_userinfo_desc { get; set; }
        public string command_userinfo_usage { get; set; }

        public string command_ban_desc { get; set; }
        public string command_ban_usage { get; set; }
        
        public string command_kick_desc { get; set; }
        public string command_kick_usage { get; set; }

        public string command_calculate_desc { get; set; }
        public string command_calculate_usage { get; set; }

        public string command_choose_desc { get; set; }
        public string command_choose_usage { get; set; }

        public string command_roleinfo_desc { get; set; }
        public string command_roleinfo_usage { get; set; }

        public string command_modules_desc { get; set; }
        public string command_modules_usage { get; set; }

        public string command_help_desc { get; set; }
        public string command_help_usage { get; set; }

        public string command_roll_desc { get; set; }
        public string command_roll_usage { get; set; }

        public string command_boobs_desc { get; set; }
        public string command_boobs_usage { get; set; }

        public string command_butts_desc { get; set; }
        public string command_butts_usage { get; set; }

        public string command_hentai_desc { get; set; }
        public string command_hentai_usage { get; set; }

        public string command_rule34_desc { get; set; }
        public string command_rule34_usage { get; set; }

        public string command_e621_desc { get; set; }
        public string command_e621_usage { get; set; }

        public string command_booru_desc { get; set; }
        public string command_booru_usage { get; set; }

        public string command_gelbooru_desc { get; set; }
        public string command_gelbooru_usage { get; set; }

        public string command_danbooru_desc { get; set; }
        public string command_danbooru_usage { get; set; }

        public string command_remindme_desc { get; set; }
        public string command_remindme_usage { get; set; }

        public string command_commands_desc { get; set; }
        public string command_commands_usage { get; set; }

        public string command_commandlist_desc { get; set; }
        public string command_commandlist_usage { get; set; }

        public string command_donate_desc { get; set; }
        public string command_donate_usage { get; set; }

        public string command_goodnight_desc { get; set; }
        public string command_goodnight_usage { get; set; }

        public string command_langs_desc { get; set; }
        public string command_langs_usage { get; set; }

        public string command_role_desc { get; set; }
        public string command_role_usage { get; set; }

        public string command_remwarn_desc { get; set; }
        public string command_remwarn_usage { get; set; }

        public string command_slowmode_desc { get; set; }
        public string command_slowmode_usage { get; set; }

        public string command_unmute_desc { get; set; }
        public string command_unmute_usage { get; set; }

        public string command_warn_desc { get; set; }
        public string command_warn_usage { get; set; }

        public string command_mute_desc { get; set; }
        public string command_mute_usage { get; set; }

        public string command_warnings_desc { get; set; }
        public string command_warnings_usage { get; set; }

        public string command_roles_desc { get; set; }
        public string command_roles_usage { get; set; }

        public string command_clear_desc { get; set; }
        public string command_clear_usage { get; set; }

        public string command_clear_from_desc { get; set; }
        public string command_clear_from_usage { get; set; }

        public string command_clear_with_desc { get; set; }
        public string command_clear_with_usage { get; set; }

        public string command_nyan_desc { get; set; }
        public string command_nyan_usage { get; set; }

        public string command_emojify_desc { get; set; }
        public string command_emojify_usage { get; set; }

        public string command_pokeinfo_desc { get; set; }
        public string command_pokeinfo_usage { get; set; }

        public string command_reverse_desc { get; set; }
        public string command_reverse_usage { get; set; }

        public string command_scramblr_desc { get; set; }
        public string command_scramblr_usage { get; set; }

        public string command_setlang_desc { get; set; }
        public string command_setlang_usage { get; set; }

        public string command_selfrole_add_desc { get; set; }
        public string command_selfrole_add_usage { get; set; }

        public string command_selfrole_remove_desc { get; set; }
        public string command_selfrole_remove_usage { get; set; }


        /* Strings */
        public string only_dm_channel { get; set; }
        public string success { get; set; }
        public string none { get; set; }
        public string activity { get; set; }

        public string _true { get; set; }
        public string _false { get; set; }

        public string enable { get; set; }
        public string disable { get; set; }

        public string enabled { get; set; }
        public string disabled { get; set; }

        public string invisible { get; set; }
        public string offline { get; set; }
        public string away { get; set; }
        public string donotdisturb { get; set; }

        public string listening { get; set; }
        public string playing { get; set; }
        public string streaming { get; set; }
        public string watching { get; set; }

        public string error_occurred { get; set; }
        public string incorrect_response_string { get; set; }

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

        public string rolecol_bot_require_higher { get; set; }
        public string rolecol_user_require_higher { get; set; }
        public string rolecol_set { get; set; }

        public string roulette_killed { get; set; }
        public string roulette_safe { get; set; }
        public string roulette_not_current_player { get; set; }
        public string roulette_not_started { get; set; }
        public string roulette_no_game { get; set; }
        public string roulette_game_over { get; set; }
        public string roulette_error { get; set; }
        public string roulette_starting { get; set; }
        public string roulette_not_enough_players { get; set; }
        public string roulette_not_game_master { get; set; }
        public string roulette_next_player { get; set; }
        public string roulette_winner { get; set; }
        public string roulette_trigger_pulled { get; set; }
        public string roulette_join_success { get; set; }
        public string roulette_join_fail { get; set; }
        public string roulette_game_quit { get; set; }
        public string roulette_player_kicked { get; set; }

        public string coin_heads { get; set; }
        public string coin_tails { get; set; }
        public string coin_flipped { get; set; }

        public string roll_rolled { get; set; }

        public string uinf_id { get; set; }
        public string uinf_status { get; set; }
        public string uinf_acc_create { get; set; }
        public string uinf_acc_join { get; set; }
        public string uinf_permissions { get; set; }
        public string uinf_roles { get; set; }

        public string roleinfo_id { get; set; }
        public string roleinfo_position { get; set; }
        public string roleinfo_created { get; set; }
        public string roleinfo_hoisted { get; set; }
        public string roleinfo_mentionable { get; set; }
        public string roleinfo_managed { get; set; }
        public string roleinfo_permissions { get; set; }

        public string modules_title { get; set; }
        public string modules_help { get; set; }
        public string modules_count { get; set; }

        public string commands_title { get; set; }
        public string commands_help { get; set; }
        public string commands_no_alias { get; set; }

        public string help_title { get; set; }
        public string help_aliases { get; set; }
        public string help_description { get; set; }
        public string help_info_description { get; set; }
        public string help_usage { get; set; }

        public string remindme_success { get; set; }
        public string remindme_datetime_short { get; set; }

        public string pokemon_weight { get; set; }
        public string pokemon_height { get; set; }
        public string pokemon_base_exp { get; set; }
        public string pokemon_abilities { get; set; }
        public string pokemon_types { get; set; }
        public string pokemon_evolution_chain { get; set; }
        public string pokemon_not_found { get; set; }

        public string donate_title { get; set; }
        public string donate_desc { get; set; }

        public string source { get; set; }
        public string page { get; set; }
        public string _explicit { get; set; }
        public string safe { get; set; }

        public string user_banned { get; set; }
        public string user_kicked { get; set; }
        public string user_unbanned { get; set; }
        public string user_muted { get; set; }
        public string user_unmuted { get; set; }
        public string user_warned { get; set; }
        public string user_remwarn { get; set; }

        public string clear_result { get; set; }

        public string mute_disabled { get; set; }
        public string warnings_disabled { get; set; }
        public string roles_disabled { get; set; }

        public string scramblr_not_enabled { get; set; }

        public string slowmode_time_long { get; set; }
        public string slowmode_disabled { get; set; }
        public string slowmode_enabled { get; set; }

        public string warnings_list_title { get; set; }
        public string no_custom_commands { get; set; }
        public string goodnight_title { get; set; }
        public string langs_title { get; set; }
        public string remindme_incorrect_response_string { get; set; }
        public string role_given { get; set; }
        public string role_not_found { get; set; }
        public string roles_list_title { get; set; }
        public string lang_set_to { get; set; }
        public string role_added { get; set; }
        public string role_removed { get; set; }
        public string moderator_role_added { get; set; }
        public string moderator_role_removed { get; set; }
        public string administrator_role_removed { get; set; }
        public string administrator_role_added { get; set; }

        /* Events */
        public string event_channel_create { get; set; }
        public string event_channel_name { get; set; }
        public string event_channel_permissions { get; set; }
    }
}
