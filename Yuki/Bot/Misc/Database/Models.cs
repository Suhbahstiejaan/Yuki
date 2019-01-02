using System;
using System.ComponentModel.DataAnnotations;
using Yuki.Bot.Services;

namespace Yuki.Bot.Misc.Database
{

    public class DbEntity
    {
        [Key]
        public int Id { get; set; }
    }

    public class Setting : DbEntity
    {
        public ulong ServerId { get; set; }

        public string Name { get; set; }
        public bool State { get; set; }
    }

    public class Role : DbEntity
    {
        public ulong ServerId { get; set; }

        public ulong RoleId { get; set; }
    }

    public class JoinLeaveMessage : DbEntity
    {
        public enum MessageType { Join, Leave }

        public ulong ServerId { get; set; }

        public MessageType MsgType { get; set; }
        
        public string Text { get; set; }
    }

    public class Command : DbEntity
    {
        public enum CommandType { Text, Image };
        public CommandType CmdType { get; set; }

        public ulong ServerId { get; set; }

        public string CmdName { get; set; }
        public string CmdResponse { get; set; }
    }

    public class IgnoredChannel : DbEntity
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class AutoAssignRole : DbEntity
    {
        public ulong ServerId { get; set; }
        public ulong RoleId { get; set; }
        public TimeSpan ApplyTime { get; set; }
    }

    public class IgnoredServer : DbEntity
    {
        public ulong ServerId { get; set; }
        public bool IsIgnored { get; set; }
    }

    public class MuteRole : DbEntity
    {
        public ulong ServerId { get; set; }
        public ulong RoleId { get; set; }
    }

    public class Purgeable : DbEntity
    {
        public ulong ServerId { get; set; }
        public DateTime LeaveDate { get; set; }
    }

    public class WelcomeChannel : DbEntity
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class LogChannel : DbEntity
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class WarnedUser : DbEntity
    {
        public int Warning { get; set; }
        public ulong ServerId { get; set; }
        public ulong UserId { get; set; }
        public string WarnReasons { get; set; }
    }

    public class GuildWarningAction : DbEntity
    {
        public ulong ServerId { get; set; }
        public int Warning { get; set; }
        public ulong RoleId { get; set; }
        public WarningAction Action { get; set; }
    }

    public class CustomPrefix : DbEntity
    {
        public ulong ServerId { get; set; }
        public string prefix { get; set; }
    }

    public class DataOptIn : DbEntity
    {
        public ulong UserId { get; set; }
        public bool optedIn { get; set; }
    }
}