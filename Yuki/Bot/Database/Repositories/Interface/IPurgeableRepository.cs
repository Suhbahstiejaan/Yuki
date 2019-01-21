using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IPurgeableRepository : IDisposable
    {
        Purgeable GetPurgeable(ulong guildId);
        IEnumerable<Purgeable> GetPurgeables();
        IEnumerable<Purgeable> GetPurgeablesOlderThan(int days);
        void AddPurgeable(Purgeable purgeable);
        void RemovePurgeable(Purgeable purgeable);
    }
}
