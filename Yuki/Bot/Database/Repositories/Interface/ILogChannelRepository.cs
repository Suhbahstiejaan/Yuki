using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface ILogChannelRepository : IDisposable
    {
        LogChannel GetChannel(ulong guildId);
        void AddChannel(LogChannel channel);
        void RemoveChannel(LogChannel channel);
    }
}
