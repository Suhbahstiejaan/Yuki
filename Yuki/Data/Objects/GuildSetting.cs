using LiteDB;

namespace Yuki.Data.Objects
{
    public struct GuildSetting
    {
        [BsonId]
        public string Name { get; set; }

        public bool IsEnabled { get; set; }
    }
}
