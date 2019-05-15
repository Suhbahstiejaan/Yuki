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
            return (string)Strings.GetType().GetProperty(stringName).GetValue(Strings, null)
                ?? stringName;
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
        public string created { get; set; }
        public string days_ago { get; set; }
        public string true_ { get; set; }
        public string false_ { get; set; }
        public string only_guild_channel { get; set; }

        public string scramblr_title { get; set; }
        public string scramblr_tos { get; set; }
        public string scramblr_agreed { get; set; }
        public string scramblr_opted_out { get; set; }

        public string ping_pong { get; set; }
        public string ping_waiting { get; set; }
        public string ping_response { get; set; }

        public string avatar_title { get; set; }

        public string coinflip_heads { get; set; }
        public string coinflip_tails { get; set; }
        public string coinflip_flipped { get; set; }

        public string role_is_hoisted { get; set; }
        public string role_is_mentionable { get; set; }
        public string role_position { get; set; }
        public string role_permissions { get; set; }
    }
}
