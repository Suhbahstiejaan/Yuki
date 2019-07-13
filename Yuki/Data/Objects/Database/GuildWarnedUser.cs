using LiteDB;
using System.Collections.Generic;

namespace Yuki.Data.Objects.Database
{
    public struct GuildWarnedUser
    {
        [BsonId]
        public ulong Id { get; set; }

        public int Warning { get; set; }
        public List<string> WarningReasons { get; set; }
    }
}
