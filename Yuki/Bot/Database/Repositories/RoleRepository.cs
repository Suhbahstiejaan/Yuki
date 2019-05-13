using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class RoleRepository : IRoleRepository, IDisposable
    {
        private YukiContext context;

        public RoleRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddRole(Role role)
        {
            context.Roles.Add(role);
        }

        public void RemoveRole(Role role)
        {
            context.Roles.Remove(role);
        }

        public Role GetRole(ulong roleId, ulong guildId)
        {
            Role role = context.Roles.FirstOrDefault(x => x.ServerId == guildId && x.RoleId == roleId);
            return role;
        }

        public IEnumerable<Role> GetRoles(ulong guildId)
        {
            IEnumerable<Role> roles = context.Roles.Where(x => x.ServerId == guildId);
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
