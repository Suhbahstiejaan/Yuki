using Discord;
using Nett;
using Qmmands;
using System.Collections.Generic;
using System.IO;
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

        public static void CheckCommands(CommandService commands)
        {
            foreach (KeyValuePair<string, Language> lang in Languages)
            {
                Logger.Write(LogLevel.Info, $"Checking translations for language {lang.Key}...");

                int validTranslations = commands.GetAllCommands().Count;

                foreach (Command c in commands.GetAllCommands())
                {
                    string cmd = $"command_{c.Name.ToLower().Replace(' ', '_')}_desc";

                    if (Languages[lang.Key].GetString(cmd) == cmd)
                    {
                        Logger.Write(LogLevel.Warning, $"No translation found for {cmd}");
                        validTranslations--;
                    }
                }

                if (validTranslations != 0)
                {
                    int numMissing = commands.GetAllCommands().Count - validTranslations;

                    if (numMissing > 1)
                    {
                        Logger.Write(LogLevel.Warning, $"{numMissing} commands are missing a translation. Please consider adding them!");
                    }
                    else
                    {
                        Logger.Write(LogLevel.Warning, $"A command is missing a translation. Please consider adding adding it!");
                    }
                }
                else
                {
                    Logger.Write(LogLevel.Info,
                        $"All command translations validated for {lang.Key}. This does not guarantee ALL string translations exist!");
                }
            }
        }
    }
}
