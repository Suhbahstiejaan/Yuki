using LiteDB;

namespace Yuki.Data.Objects
{
    public enum WarningAction
    {
        GiveRole,
        Kick,
        Ban
    }

    public struct GuildWarningAction
    {
        [BsonId]
        public int Warning { get; set; }

        public ulong RoleId { get; set; }
        public WarningAction WarningAction { get; set; }
    }
}