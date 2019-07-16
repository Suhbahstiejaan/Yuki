using Discord;
using Nett;
using System.Collections.Generic;
using System.IO;
using Yuki.Commands;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public static class LocalizationService
    {
        public static Dictionary<string, Language> Languages { get; private set; } = new Dictionary<string, Language>();

        public static void LoadLanguages()
        {
            if(!Directory.Exists(FileDirectories.LangRoot))
            {
                Directory.CreateDirectory(FileDirectories.LangRoot);
            }

            if(Languages.Count < 1)
            {
                string[] langFiles = Directory.GetFiles(FileDirectories.LangRoot);

                for(int i = 0; i < langFiles.Length; i++)
                {
                    Language lang = Toml.ReadFile<Language>(langFiles[i]);

                    Languages.Add(lang.Code, lang);
                }
            }

            if(Languages.Count < 1)
            {
                Languages.Add("none", new Language());
            }
        }



        public static void Reload()
        {
            if(Languages.Count > 0)
            {
                Languages.Clear();
                LoadLanguages();
            }
        }

        public static Language GetLanguage(string code)
        {
            if(Languages.ContainsKey(code))
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

            if(context.Channel is IGuildChannel)
            {
                langCode = GuildSettings.GetGuild(context.Guild.Id).LangCode;
            }

            if(string.IsNullOrWhiteSpace(langCode))
            {
                langCode = "en_US";
            }

            return GetLanguage(langCode);
        }
    }
}
