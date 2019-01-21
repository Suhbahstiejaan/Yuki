using System;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class AutoBanRepository : IAutoBanRepository, IDisposable
    {
        private YukiContext context;

        public AutoBanRepository(YukiContext context)
        {
            this.context = context;
        }

        public void Add(AutoBanUser user)
        {
            context.AutoBanUsers.Add(user);
        }

        public void Remove(AutoBanUser user)
        {
            if (context.AutoBanUsers != null && context.AutoBanUsers.Count() > 0 && context.AutoBanUsers.FirstOrDefault(x => x == user) != null)
                context.AutoBanUsers.Remove(user);
        }

        public AutoBanUser GetUser(ulong userId, ulong guildId)
            => context.AutoBanUsers.Where(x => x.ServerId == guildId && x.UserId == userId).FirstOrDefault();

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