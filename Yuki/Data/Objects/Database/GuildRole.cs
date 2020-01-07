using LiteDB;

namespace Yuki.Data.Objects.Database
{
    public struct GuildRole
    {
        [BsonId]
        public ulong Id { get; set; }
        
        public bool IsTeamRole { get; set; }
    }
}
