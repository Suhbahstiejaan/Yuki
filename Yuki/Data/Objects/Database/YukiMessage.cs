using LiteDB;
using System;

namespace Yuki.Data.Objects.Database
{
    public struct YukiMessage
    {
        [BsonId]
        public ulong Id { get; set; }

        public DateTime SendDate { get; set; }

        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }

        public string Content { get; set; }
    }
}
