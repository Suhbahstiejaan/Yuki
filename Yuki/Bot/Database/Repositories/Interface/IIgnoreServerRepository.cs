using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IIgnoreServerRepository : IDisposable
    {
        IgnoredServer GetServer(ulong guildId);
        void AddServer(IgnoredServer server);
    }
}
