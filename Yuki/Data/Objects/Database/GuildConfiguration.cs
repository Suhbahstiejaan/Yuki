using LiteDB;
using System;
using System.Collections.Generic;

namespace Yuki.Data.Objects.Database
{
    public struct GuildConfiguration
    {
        [BsonId]
        public ulong Id { get; set; }

        public DateTime LeaveDate { get; set; } /* When the bot was removed from the server. Used to clean up unused data. */

        public bool EnableWelcome { get; set; }
        public bool EnableGoodbye { get; set; }
        public bool EnableCache { get; set; }
        public bool EnableMute { get; set; }
        public bool EnableNsfw { get; set; }
        public bool EnableWarnings { get; set; }
        public bool EnablePrefix { get; set; }
        public bool EnableRoles { get; set; }
        public bool EnableLogging { get; set; }

        public string WelcomeMessage { get; set; }
        public string GoodbyeMessage { get; set; }

        public string LangCode { get; set; }

        public string Prefix { get; set; } /* Custom prefix for server */

        public ulong WelcomeChannel { get; set; }
        public ulong LogChannel { get; set; }
        public ulong MuteRole { get; set; }
        
        public List<GuildSetting> Settings { get; set; }
        public List<GuildCommand> Commands { get; set; }
        public List<GuildWarnedUser> WarnedUsers { get; set; }
        public List<GuildWarningAction> WarningActions { get; set; }

        public List<ulong> CacheIgnoredChannels { get; set; }
        public List<ulong> LevelIgnoredChannels { get; set; }
        public List<ulong> AutoBanUsers { get; set; }    /* Users to ban when they join */
        public List<ulong> NsfwChannels { get; set; }
        public List<ulong> AssignableRoles { get; set; }
    }
}
