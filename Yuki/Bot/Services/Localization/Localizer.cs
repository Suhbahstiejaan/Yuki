using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yuki.Bot.Misc;

namespace Yuki.Bot.Services.Localization
{
    public class Localizer
    {
        private static string dir = FileDirectories.AppDataDirectory + "lang\\";
        private static Logger _log = Logger.GetLoggerInstance();

        public static Dictionary<string, List<ModuleInfo>> modules = new Dictionary<string, List<ModuleInfo>>();

        /// <summary>
         /// Verifies each folder in lang has a valid entry for each command
         /// </summary>
        public static void VerifyCommands()
        {
            string lang = YukiStrings.default_lang;

            //get modules if we haven't already
            if (modules.Keys.Count == 0)
                GetModules();
            
            int commandCount = YukiClient.Instance.CommandService.Modules.Select(mod => mod.Commands.Count).Sum();
            
            foreach (string language in Directory.EnumerateDirectories(FileDirectories.AppDataDirectory + "lang"))
            {
                lang = language.Replace(FileDirectories.AppDataDirectory + "lang\\", "");
                int verifiedCommands = 0;

                List<string> commands = GetCommands(lang).Select(x => x.Name).ToList();

                //Iterate over every command in each module
                foreach (KeyValuePair<string, List<ModuleInfo>> pair in modules)
                {
                    try
                    {
                        foreach (ModuleInfo module in pair.Value)
                        {
                            foreach (CommandInfo command in module.Commands)
                            {
                                string cmdName = null;

                                if (module.IsSubmodule)
                                    cmdName += module.Name.ToLower();

                                /* 'BaseCommand' is used for the first command of a command group */
                                if (command.Name != "BaseCommand")
                                {
                                    if (cmdName != null)
                                        cmdName += "_";
                                    cmdName += command.Name;
                                }

                                LocalizedCommand cmd = GetCommands(lang, null, cmdName).FirstOrDefault();

                                if (cmd == null)
                                    _log.Write(LogSeverity.Warning, "Command \"" + cmdName + "\" must have an entry in " + YukiStrings.default_lang);
                                else
                                {
                                    verifiedCommands++;
                                    commands.Remove(cmd.Name);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //File most likely does not exist
                        _log.Write(LogSeverity.Error, e.Message);
                        break;
                    }
                }

                _log.Write(LogSeverity.Info, verifiedCommands + "/" + commandCount + " command translations valid for " + lang);


                if (commands != null)
                {
                    _log.Write(LogSeverity.Info, "Found " + commands.Count + " excess commands in commands.json:");
                    foreach (var command in commands)
                        Console.WriteLine("\t- " + command);
                }
            }
        }

        private static void GetModules()
        {
            foreach (ModuleInfo module in YukiClient.Instance.CommandService.Modules)
            {
                if (!module.IsSubmodule)
                {
                    string moduleName = module.Name.Split('_')[0];

                    if (!modules.ContainsKey(moduleName))
                        modules.Add(moduleName, new List<ModuleInfo>());

                    modules[moduleName].Add(module);

                    if (module.Submodules.Count > 0)
                        foreach (ModuleInfo subModule in module.Submodules)
                            modules[moduleName].Add(subModule);
                }
            }
        }

        //Get a list of all commands for a specified language
        public static List<LocalizedCommand> GetCommands(string lang, string toIgnore = null, string commandToGet = null)
        {
            List<LocalizedCommand> commands = new List<LocalizedCommand>();

            commands = JsonConvert.DeserializeObject<LocalizedCommands>(File.ReadAllText(dir + lang + "\\commands.json")).Commands;

            if (toIgnore != null && commands.FirstOrDefault(cmd => cmd.Name == toIgnore) != null)
                commands.Remove(commands.FirstOrDefault(cmd => cmd.Name == toIgnore));

            if (commandToGet == null)
                return commands;
            else
                return new List<LocalizedCommand>() { commands.FirstOrDefault(cmd => cmd.Name == commandToGet) };
        }
        
        public static URLStrings GetURLs
            => JsonConvert.DeserializeObject<URLStrings>(File.ReadAllText(dir + "urls.json"));

        public static TranslatedStrings GetStrings(string lang)
            => JsonConvert.DeserializeObject<TranslatedStrings>(File.ReadAllText(dir + lang + "\\strings.json"));

        public static YukiStrings YukiStrings
            => JsonConvert.DeserializeObject<YukiStrings>(File.ReadAllText(dir + "yuki.json"));

        public static string GetLocalizedStringFromData(List<Data> data, string toLocalize)
        {
            foreach(Data d in data)
                if (d.name == toLocalize)
                    return d.translation;

            /* return the default localization */
            foreach (Data d in data)
                if (d.name == YukiStrings.default_lang)
                    return d.translation;

            return "err:no_localization_found";
        }

        /* Doesn't technically have much to do with localization
         * since we're just parsing a text file thats the same for
         * every language but ¯\_(ツ)_/¯
         */
         public static string[] RamMoeBlacklist {
            get
            {
                List<string> lines = new List<string>();
                using (StreamReader reader = new StreamReader(dir + "rammoe_blacklist.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        lines.Add(line);
                }
                return lines.ToArray();
            }
        }

        /* Same as above tbh */
        public static string[] Blacklist {
            get
            {
                List<string> lines = new List<string>();
                using (StreamReader reader = new StreamReader(dir + "blacklist.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        lines.Add(line);
                }
                return lines.ToArray();
            }
        }
    }
}
