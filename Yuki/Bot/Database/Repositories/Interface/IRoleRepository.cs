using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IRoleRepository : IDisposable
    {
        Role GetRole(ulong roleId, ulong guildId);
        IEnumerable<Role> GetRoles(ulong guildId);
        void AddRole(Role role);
        void RemoveRole(Role role);
    }
}
