using LiteDB;

namespace Yuki.Data.Objects.Database
{
    public struct YukiMessage
    {
        [BsonId]
        public ulong Id { get; set; }

        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }

        public string Content { get; set; }
    }
}
