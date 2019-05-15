using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Nett;
using System.Collections.Generic;
using System.IO;
using Yuki.Data.Objects;

namespace Yuki.Services
{
    public class LocalizationService
    {
        private Dictionary<string, Language> languages = new Dictionary<string, Language>();

        public void LoadLanguages()
        {
            if(!Directory.Exists(YukiBot.DataDirectoryRootPath + "languages/"))
            {
                Directory.CreateDirectory(YukiBot.DataDirectoryRootPath + "languages/");
            }

            if(languages.Count < 1)
            {
                string[] langFiles = Directory.GetFiles(YukiBot.DataDirectoryRootPath + "languages/");

                for(int i = 0; i < langFiles.Length; i++)
                {
                    Language lang = Toml.ReadFile<Language>(langFiles[i]);

                    languages.Add(lang.Code, lang);
                }
            }
        }



        public void Reload()
        {
            if(languages.Count > 0)
            {
                languages.Clear();
                LoadLanguages();
            }
        }

        public Language GetLanguage(string code)
        {
            if(languages.ContainsKey(code))
            {
                return languages[code];
            }
            else
            {
                return null;
            }
        }

        public Language GetLanguage(ICommandContext context)
        {
            string langCode = YukiBot.Services.GetRequiredService<ConfigDB>().GetConfiguration(context.Guild.Id).langCode;

            if (string.IsNullOrEmpty(langCode))
            {
                langCode = "en_US";
            }

            return YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage(langCode);
        }
    }
}
