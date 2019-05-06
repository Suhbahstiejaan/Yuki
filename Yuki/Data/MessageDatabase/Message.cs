using LiteDB;
using System.Collections.Generic;

namespace Yuki.Data.MessageDatabase
{
    public struct Message
    {
        [BsonId]
        public ulong Id { get; set; }

        public ulong ChannelId { get; set; }
        public string Content { get; set; }
    }

    public struct YukiUser
    {
        [BsonId]
        public ulong Id { get; set; }

        public bool IsPatron { get; set; }
        public bool CanGetMsgs { get; set; }

        public List<Message> Messages { get; set; }
    }
}
