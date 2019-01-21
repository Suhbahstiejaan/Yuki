using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class MuteRoleRepository : IMuteRoleRepository, IDisposable
    {
        private YukiContext context;

        public MuteRoleRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddMuteRole(MuteRole muteRole)
        {
            context.MuteRole.Add(muteRole);
        }

        public void RemoveMuteRole(MuteRole muteRole)
        {
            context.MuteRole.Remove(muteRole);
        }

        public MuteRole GetMuteRole(ulong guildId)
        {
            MuteRole muteRole = context.MuteRole.FirstOrDefault(x => x.ServerId == guildId);
            return muteRole;
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

