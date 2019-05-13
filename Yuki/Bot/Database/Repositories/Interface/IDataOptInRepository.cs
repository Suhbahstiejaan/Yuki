using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IDataOptInRepository : IDisposable
    {
        DataOptIn GetUser(ulong userId);
        void Add(DataOptIn data);
        void Remove(DataOptIn data);
    }
}
