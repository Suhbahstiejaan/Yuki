using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    public interface IWelcomeChannelRepository : IDisposable
    {
        WelcomeChannel GetChannel(ulong guildId);
        void AddChannel(WelcomeChannel channel);
        void RemoveChannel(WelcomeChannel channel);
    }
}
