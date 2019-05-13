using System;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class CustomPrefixRepository : ICustomPrefixRepository, IDisposable
    {
        private YukiContext context;

        public CustomPrefixRepository(YukiContext context)
        {
            this.context = context;
        }


        public CustomPrefix GetPrefix(ulong guildId)
            => context.CustomPrefixes.Where(x => x.ServerId == guildId).FirstOrDefault();

        public void Add(CustomPrefix prefix)
            => context.CustomPrefixes.Add(prefix);

        public void Remove(CustomPrefix prefix)
            => context.CustomPrefixes.Remove(prefix);

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