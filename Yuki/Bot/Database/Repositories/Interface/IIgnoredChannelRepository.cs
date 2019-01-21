using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IIgnoredChannelRepository : IDisposable
    {
        IgnoredChannel GetIgnoredChannel(ulong channelId, ulong guildId);
        IEnumerable<IgnoredChannel> GetIgnoredChannels(ulong guildId);
        void AddIgnoredChannel(IgnoredChannel ignoredChannel);
        void RemoveIgnoredChannel(IgnoredChannel ignoredChannel);
    }
}
