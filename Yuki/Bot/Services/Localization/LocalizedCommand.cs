using System.Collections.Generic;

namespace Yuki.Bot.Services.Localization
{
    public class LocalizedCommand
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Usage { get; set; }
    }

    public class LocalizedCommands
    {
        public List<LocalizedCommand> Commands { get; set; }
    }
}
