using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class IgnoredChannelRepository : IIgnoredChannelRepository, IDisposable
    {
        private YukiContext context;

        public IgnoredChannelRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddIgnoredChannel(IgnoredChannel ignoredChannel)
        {
            context.IgnoredChannels.Add(ignoredChannel);
        }

        public void RemoveIgnoredChannel(IgnoredChannel ignoredChannel)
        {
            context.IgnoredChannels.Remove(ignoredChannel);
        }

        public IgnoredChannel GetIgnoredChannel(ulong channelId, ulong guildId)
        {
            IgnoredChannel ignoredChannel = null;
            ignoredChannel = context.IgnoredChannels.Where(x => x.ChannelId == channelId && x.ServerId == guildId).FirstOrDefault();
            return ignoredChannel;
        }

        public IEnumerable<IgnoredChannel> GetIgnoredChannels(ulong guildId)
        {
            IEnumerable<IgnoredChannel> ignoredChannels = context.IgnoredChannels.Where(x => x.ServerId == guildId);
            return ignoredChannels;
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
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
