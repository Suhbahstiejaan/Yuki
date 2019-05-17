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
        public string ping_pong { get; set; }
        public string ping_waiting { get; set; }
        public string ping_response { get; set; }

        public string avatar_user_avatar { get; set; }
    }
}
