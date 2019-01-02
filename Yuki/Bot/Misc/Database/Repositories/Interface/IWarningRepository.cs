using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IWarningRepository : IDisposable
    {
        WarnedUser GetUser(ulong guildId, ulong userId);
        IEnumerable<WarnedUser> GetUsers(ulong guildId);
        void AddUser(WarnedUser user);
        void RemoveUser(WarnedUser user);
    }
}
