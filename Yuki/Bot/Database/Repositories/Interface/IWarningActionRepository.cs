using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IWarningActionRepository : IDisposable
    {
        GuildWarningAction GetAction(ulong guildId, int warning);
        IEnumerable<GuildWarningAction> GetActions(ulong guildId);
        void AddAction(GuildWarningAction action);
        void RemoveAction(GuildWarningAction action);
    }
}
