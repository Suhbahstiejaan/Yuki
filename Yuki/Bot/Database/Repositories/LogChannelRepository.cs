using System;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class LogChannelRepository : ILogChannelRepository, IDisposable
    {
        private YukiContext context;

        public LogChannelRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddChannel(LogChannel channel)
        {
            context.LogChannels.Add(channel);
        }

        public void RemoveChannel(LogChannel channel)
        {
            context.LogChannels.Remove(channel);
        }

        public LogChannel GetChannel(ulong guildId)
        {
            LogChannel channel = context.LogChannels.FirstOrDefault(x => x.ServerId == guildId);
            return channel;
        }
        
        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    context.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
