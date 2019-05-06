using LiteDB;
using System;
using System.Collections.Generic;

namespace Yuki.Data.ConfigurationDatabase
{
    public enum WarningAction
    {
        GiveRole,
        Kick,
        Ban
    }

    public struct GuildConfiguration
    {
        [BsonId]
        public ulong Id { get; set; }

        public bool IsIgnored { get; set; } /* Used for scramblr */

        public DateTime LeaveDate { get; set; } /* When the bot was removed from the server. Used to clean up unused data. */

        public string WelcomeMessage { get; set; }
        public string GoodbyeMessage { get; set; }

        public string langCode { get; set; }

        public string Prefix { get; set; } /* Custom prefix for server */

        public ulong WelcomeChannel { get; set; }
        public ulong LogChannel { get; set; }
        public ulong MuteRole { get; set; }
        //public ulong AutoAssignedRole { get; set; }

        public List<GuildSetting> Settings { get; set; }
        public List<GuildCommand> Commands { get; set; }
        public List<GuildWarnedUser> WarnedUsers { get; set; }

        public List<ulong> IgnoredChannels { get; set; } /* Used for leveling and scramblr */
        public List<ulong> AutoBanUsers { get; set; }    /* Users to ban when they join */
        public List<ulong> NsfwChannels { get; set; }
        public List<ulong> AssignableRoles { get; set; }
    }

    public struct GuildSetting
    {
        [BsonId]
        public string Name { get; set; }

        public bool IsEnabled { get; set; }
    }

    public struct GuildCommand
    {
        [BsonId]
        public string Name { get; set; }

        public string Response { get; set; }
    }

    public struct GuildWarnedUser
    {
        [BsonId]
        public ulong Id { get; set; }

        public int Warning { get; set; }
        public List<string> WarningReasons { get; set; }
    }

    public struct GuildWarningAction
    {
        [BsonId]
        public int Warning { get; set; }

        public ulong RoleId { get; set; }
        public WarningAction WarningAction { get; set; }
    }
}