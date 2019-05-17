using Nett;
using System.Collections.Generic;
using System.IO;
using Yuki.Commands;
using Yuki.Data.Objects;

namespace Yuki.Services
{
    public static class LocalizationService
    {
        private static Dictionary<string, Language> languages = new Dictionary<string, Language>();

        public static void LoadLanguages()
        {
            if(!Directory.Exists(FileDirectories.LangRoot))
            {
                Directory.CreateDirectory(FileDirectories.LangRoot);
            }

            if(languages.Count < 1)
            {
                string[] langFiles = Directory.GetFiles(FileDirectories.LangRoot);

                for(int i = 0; i < langFiles.Length; i++)
                {
                    Language lang = Toml.ReadFile<Language>(langFiles[i]);

                    languages.Add(lang.Code, lang);
                }
            }

            if(languages.Count < 1)
            {
                languages.Add("none", new Language());
            }
        }



        public static void Reload()
        {
            if(languages.Count > 0)
            {
                languages.Clear();
                LoadLanguages();
            }
        }

        public static Language GetLanguage(string code)
        {
            if(languages.ContainsKey(code))
            {
                return languages[code];
            }
            else
            {
                return languages["none"];
            }
        }

        public static Language GetLanguage(YukiCommandContext context)
        {
            string langCode = ConfigDB.GetConfiguration(context.Guild.Id).langCode;

            if (string.IsNullOrEmpty(langCode))
            {
                langCode = "en_US";
            }

            return GetLanguage(langCode);
        }
    }
}
