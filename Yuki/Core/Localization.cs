using Discord;
using Nett;
using Newtonsoft.Json.Linq;
using Qmmands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yuki.Commands;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Core
{
    public static class Localization
    {
        public static Dictionary<string, Language> Languages { get; private set; } = new Dictionary<string, Language>();

        public static void LoadLanguages()
        {
            if (!Directory.Exists(FileDirectories.LangRoot))
            {
                Directory.CreateDirectory(FileDirectories.LangRoot);
            }

            if (Languages.Count < 1)
            {
                string[] langFiles = Directory.GetFiles(FileDirectories.LangRoot);

                for (int i = 0; i < langFiles.Length; i++)
                {
                    Language lang = Toml.ReadFile<Language>(langFiles[i]);

                    Languages.Add(lang.Code, lang);
                }
            }

            if (Languages.Count < 1)
            {
                Languages.Add("none", new Language());
            }
        }



        public static void Reload()
        {
            if (Languages.Count > 0)
            {
                Languages.Clear();
                LoadLanguages();
            }
        }

        public static Language GetLanguage(string code)
        {
            if (Languages.ContainsKey(code))
            {
                return Languages[code];
            }
            else
            {
                return Languages["none"];
            }
        }

        public static Language GetLanguage(YukiCommandContext context)
        {
            string langCode = "en_US";

            if (context.Channel is IGuildChannel)
            {
                langCode = GuildSettings.GetGuild(context.Guild.Id).LangCode;
            }

            if (string.IsNullOrWhiteSpace(langCode))
            {
                langCode = "en_US";
            }

            return GetLanguage(langCode);
        }

        public static void CheckTranslations()
        {
            foreach (KeyValuePair<string, Language> lang in Languages)
            {
                Logger.Write(LogLevel.Status, $"Checking translations for language w/code {lang.Value.Code}...");

                List<JProperty> properties = JObject.FromObject(lang.Value.Strings).Properties().ToList();
                int invalidTranslations = 0;


                foreach (JProperty property in properties)
                {
                    if (lang.Value.GetString(property.Name) == property.Name)
                    {
                        Logger.Write(LogLevel.Warning, $"   No translation found for {property.Name}");
                        invalidTranslations++;
                    }
                }

                if(invalidTranslations > 0)
                {
                    Logger.Write(LogLevel.Status, $"{lang.Value.Code} has {invalidTranslations} strings without a translation!");
                }
            }
        }
    }
}