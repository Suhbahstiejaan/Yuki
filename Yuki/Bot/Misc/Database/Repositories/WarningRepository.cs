using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class WarningRepository : IWarningRepository
    {
        private YukiContext context;

        public WarningRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddUser(WarnedUser user)
        {
            context.Warnings.Add(user);
        }

        public void RemoveUser(WarnedUser user)
        {
            context.Warnings.Remove(user);
        }

        public WarnedUser GetUser(ulong guildId, ulong userId)
        {
            WarnedUser warnedUser = context.Warnings.FirstOrDefault(x => x.ServerId == guildId && x.UserId == userId);
            return warnedUser;
        }

        public IEnumerable<WarnedUser> GetUsers(ulong guildId)
        {
            IEnumerable<WarnedUser> warnings = context.Warnings.Where(x => x.ServerId == guildId);
            return warnings;
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
