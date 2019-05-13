
using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    public interface ICustomPrefixRepository : IDisposable
    {
        CustomPrefix GetPrefix(ulong guildId);
        void Add(CustomPrefix prefix);
        void Remove(CustomPrefix prefix);
    }
}
