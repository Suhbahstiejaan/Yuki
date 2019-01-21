using System;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class DataOptInRepository : IDataOptInRepository, IDisposable
    {
        private YukiContext context;

        public DataOptInRepository(YukiContext context)
        {
            this.context = context;
        }

        public void Add(DataOptIn data)
        {
            context.DataCollectionOptIn.Add(data);
        }

        public void Remove(DataOptIn data)
        {
            context.DataCollectionOptIn.Remove(data);
        }

        public DataOptIn GetUser(ulong userId)
        {
            DataOptIn data = context.DataCollectionOptIn.FirstOrDefault(x => x.UserId == userId);
            return data;
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
