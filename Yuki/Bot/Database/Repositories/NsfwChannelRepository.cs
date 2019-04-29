using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class NsfwChannelRepository : IDisposable
    {
        private YukiContext context;

        public NsfwChannelRepository(YukiContext context)
        {
            this.context = context;
        }

        public void Add(NsfwChannel channel)
        {
            context.NsfwChannels.Add(channel);
        }

        public void Remove(NsfwChannel channel)
        {
            context.NsfwChannels.Remove(channel);
        }

        public NsfwChannel Get(ulong channelId, ulong guildId)
        {
            NsfwChannel channel = context.NsfwChannels.FirstOrDefault(x => x.ChannelId == channelId && x.ServerId == guildId);
            return channel;
        }

        public IEnumerable<NsfwChannel> GetChannels(ulong guildId)
        {
            IEnumerable<NsfwChannel> channels = context.NsfwChannels.Where(x => x.ServerId == guildId);
            return channels;
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
