using System;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class WelcomeChannelRepository : IWelcomeChannelRepository, IDisposable
    {
        private YukiContext context;

        public WelcomeChannelRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddChannel(WelcomeChannel channel)
        {
            context.WelcomeChannels.Add(channel);
        }

        public void RemoveChannel(WelcomeChannel channel)
        {
            if(context.WelcomeChannels != null && context.WelcomeChannels.Count() > 0 && context.WelcomeChannels.FirstOrDefault(x => x == channel) != null)
                context.WelcomeChannels.Remove(channel);
        }

        public WelcomeChannel GetChannel(ulong guildId)
        {
            WelcomeChannel channel = context.WelcomeChannels.Where(x => x.ServerId == guildId).FirstOrDefault();
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