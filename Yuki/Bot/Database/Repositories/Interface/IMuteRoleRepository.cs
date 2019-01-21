using System;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IMuteRoleRepository : IDisposable
    {
        MuteRole GetMuteRole(ulong guildId);
        void AddMuteRole(MuteRole muteRole);
        void RemoveMuteRole(MuteRole muteRole);
    }
}
