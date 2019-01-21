using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IAutoAssignRoleRepository : IDisposable
    {
        AutoAssignRole GetRole(ulong roleId, ulong guildId);
        IEnumerable<AutoAssignRole> GetRoles(ulong guildId);
        void AddRole(AutoAssignRole role);
        void RemoveRole(AutoAssignRole role);
    }
}
