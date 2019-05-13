using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class JoinLeaveMessageRepository : IJoinLeaveMessageRepository, IDisposable
    {
        private YukiContext context;

        public JoinLeaveMessageRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddJoinLeaveMessage(JoinLeaveMessage joinLeaveMessage)
        {
            context.JoinLeaveMessages.Add(joinLeaveMessage);
        }

        public void RemoveJoinLeaveMessage(JoinLeaveMessage joinLeaveMessage)
        {
            context.JoinLeaveMessages.Remove(joinLeaveMessage);
        }

        public JoinLeaveMessage GetJoinLeaveMessage(JoinLeaveMessage.MessageType msgType, ulong guildId)
        {
            JoinLeaveMessage joinLeaveMessage = context.JoinLeaveMessages.FirstOrDefault(x => x.ServerId == guildId && x.MsgType == msgType);
            return joinLeaveMessage;
        }

        public JoinLeaveMessage[] GetJoinLeaveMessages()
        {
            List<JoinLeaveMessage> msgs = new List<JoinLeaveMessage>();

            foreach(var msg in context.JoinLeaveMessages)
            {
                bool exists = false;
                foreach(var m in msgs)
                {
                    if(m == msg)
                        exists = true;
                }

                if(!exists)
                    msgs.Add(msg);
            }

            return msgs.ToArray();
        }

        public IEnumerable<JoinLeaveMessage> GetJoinLeaveMessages(ulong guildId)
        {
            IEnumerable<JoinLeaveMessage> joinLeaveMessages = context.JoinLeaveMessages.Where(x => x.ServerId == guildId);
            return joinLeaveMessages;
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
