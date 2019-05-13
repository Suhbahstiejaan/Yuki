using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface IJoinLeaveMessageRepository : IDisposable
    {
        JoinLeaveMessage GetJoinLeaveMessage(JoinLeaveMessage.MessageType msgType, ulong guildId);
        IEnumerable<JoinLeaveMessage> GetJoinLeaveMessages(ulong guildId);
        void AddJoinLeaveMessage(JoinLeaveMessage joinLeaveMessage);
        void RemoveJoinLeaveMessage(JoinLeaveMessage joinLeaveMessage);
    }
}
