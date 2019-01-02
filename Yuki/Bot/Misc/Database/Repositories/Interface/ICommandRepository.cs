using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface ICommandRepository : IDisposable
    {
        Command GetCommand(string cmdName, ulong guildId);
        IEnumerable<Command> GetCommands(ulong guildId);
        void AddCommand(Command cmd);
        void RemoveCommand(Command cmd);
    }
}
