using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    public interface IAutoBanRepository : IDisposable
    {
        AutoBanUser GetUser(ulong userId, ulong guildId);
        void Add(AutoBanUser user);
        void Remove(AutoBanUser user);
    }
}
