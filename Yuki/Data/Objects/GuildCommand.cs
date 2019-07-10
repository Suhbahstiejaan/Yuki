using LiteDB;

namespace Yuki.Data.Objects
{
    public struct GuildCommand
    {
        [BsonId]
        public string Name { get; set; }
        public bool IsParsable { get; set; }
        public string Response { get; set; }
    }
}
