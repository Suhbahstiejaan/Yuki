using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class PurgeableRepository : IPurgeableRepository, IDisposable
    {
        private YukiContext context;

        public PurgeableRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddPurgeable(Purgeable purgeable)
        {
            context.Purgeable.Add(purgeable);
        }

        public void RemovePurgeable(Purgeable purgeable)
        {
            context.Purgeable.Remove(purgeable);
        }

        public Purgeable GetPurgeable(ulong guildId)
        {
            Purgeable purgeable = context.Purgeable.FirstOrDefault(x => x.ServerId == guildId);
            return purgeable;
        }

        public IEnumerable<Purgeable> GetPurgeables()
        {
            return context.Purgeable;
        }

        public IEnumerable<Purgeable> GetPurgeablesOlderThan(int days)
        {      
            return context.Purgeable.Where(x => DateTime.Now.Subtract(x.LeaveDate).Days > days);
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
