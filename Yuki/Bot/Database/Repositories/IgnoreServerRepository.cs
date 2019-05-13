using System;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class IgnoreServerRepository : IIgnoreServerRepository, IDisposable
    {
        private YukiContext context;

        public IgnoreServerRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddServer(IgnoredServer server)
        {
            context.IgnoredServers.Add(server);
        }

        public void RemoveServer(IgnoredServer s)
        {
            context.IgnoredServers.Remove(s);
        }
        
        public IgnoredServer GetServer(ulong guildId)
        {
            IgnoredServer server = context.IgnoredServers.FirstOrDefault(x => x.ServerId == guildId);
            return server;
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
