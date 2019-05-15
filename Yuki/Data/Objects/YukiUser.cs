using LiteDB;
using System.Collections.Generic;

namespace Yuki.Data.Objects
{
    public struct YukiUser
    {
        [BsonId]
        public ulong Id { get; set; }

        public bool IsPatron { get; set; }
        public bool CanGetMsgs { get; set; }

        public List<YukiMessage> Messages { get; set; }
    }
}
