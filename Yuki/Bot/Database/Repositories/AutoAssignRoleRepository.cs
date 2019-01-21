using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class AutoAssignRoleRepository : IAutoAssignRoleRepository, IDisposable
    {
        private YukiContext context;

        public AutoAssignRoleRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddRole(AutoAssignRole role)
        {
            context.AutoAssignedRoles.Add(role);
        }

        public void RemoveRole(AutoAssignRole role)
        {
            context.AutoAssignedRoles.Remove(role);
        }

        public AutoAssignRole GetRole(ulong roleId, ulong guildId)
        {
            AutoAssignRole role = context.AutoAssignedRoles.FirstOrDefault(x => x.RoleId == roleId && x.ServerId == guildId);
            return role;
        }

        public IEnumerable<AutoAssignRole> GetRoles(ulong guildId)
        {
            IEnumerable<AutoAssignRole> roles = context.AutoAssignedRoles.Where(x => x.ServerId == guildId);
            return roles;
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
