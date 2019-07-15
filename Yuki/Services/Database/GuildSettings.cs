using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class GuildSettings
    {
        public const string path = "data/settings.db";

        private const string collection = "guild_settings";

        private static GuildConfiguration DefaultConfig(ulong guildId)
            => new GuildConfiguration()
            {
                Id = guildId,
                LangCode = "en_US",
                Prefix = null,

                LeaveDate = default,

                EnableWelcome = false,
                EnableGoodbye = false,
                EnableNsfw = false,
                EnableCache = false,
                EnableLogging = false,

                AssignableRoles = new List<ulong>(),
                AutoBanUsers = new List<ulong>(),
                CacheIgnoredChannels = new List<ulong>(),
                LevelIgnoredChannels = new List<ulong>(),
                NsfwChannels = new List<ulong>(),
                Commands = new List<GuildCommand>(),
                Settings = new List<GuildSetting>(),
                WarnedUsers = new List<GuildWarnedUser>(),

                WelcomeChannel = 0,
                LogChannel = 0,
                MuteRole = 0,

                WelcomeMessage = null,
                GoodbyeMessage = null
            };

        public static GuildConfiguration GetGuild(ulong id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (!configs.FindAll().Any(conf => conf.Id == id))
                {
                    AddOrUpdate(DefaultConfig(id));
                    return GetGuild(id);
                }
                else
                {
                    return configs.FindAll().FirstOrDefault(conf => conf.Id == id);
                }
            }
        }

        public static void AddOrUpdate(GuildConfiguration config)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (!configs.FindAll().Any(conf => conf.Id == config.Id))
                {
                    configs.Insert(config);
                }
                else
                {
                    configs.Update(config);
                }
            }
        }

        public static void Remove(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    configs.Delete(guildId);
                }
            }
        }

        #region Sets
        public static void SetWelcome(string message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (!configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetWelcome(message, guildId);
                }


                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                config.WelcomeMessage = message;

                configs.Update(config);
            }
        }

        public static void SetGoodbye(string message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (!configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetGoodbye(message, guildId);
                }


                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                config.GoodbyeMessage = message;

                configs.Update(config);
            }
        }

        public static void SetMuteRole(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (!configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetMuteRole(roleId, guildId);
                }


                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                config.MuteRole = roleId;

                configs.Update(config);
            }
        }
        #endregion

        #region Toggles
        public static void ToggleWelcome(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {

                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableWelcome = !config.EnableWelcome;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleGoodbye(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableGoodbye = !config.EnableGoodbye;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleNsfw(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableNsfw = !config.EnableNsfw;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleLogging(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableLogging = !config.EnableLogging;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleCache(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableCache = !config.EnableCache;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleMute(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableMute = !config.EnableMute;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleWarnings(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableWarnings = !config.EnableWarnings;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleRoles(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnableRoles = !config.EnableRoles;

                    configs.Update(config);
                }
            }
        }

        public static void TogglePrefix(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                    config.EnablePrefix = !config.EnablePrefix;

                    configs.Update(config);
                }
            }
        }
        #endregion

        #region Adds
        public static void AddChannelNsfw(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if(!config.NsfwChannels.Contains(channelId))
                    {
                        config.NsfwChannels.Add(channelId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddChannelLog(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    config.LogChannel = channelId;

                    configs.Update(config);
                }
            }
        }

        public static void AddChannelCache(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if(!config.CacheIgnoredChannels.Contains(channelId))
                    {
                        config.CacheIgnoredChannels.Add(channelId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddWarningAction(GuildWarningAction action, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if (!config.WarningActions.Any(a => a.Warning == action.Warning))
                    {
                        config.WarningActions.Add(action);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddRole(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if (!config.AssignableRoles.Contains(roleId))
                    {
                        config.AssignableRoles.Add(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddPrefix(string prefix, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    config.Prefix = prefix;

                    configs.Update(config);
                }
            }
        }
        #endregion

        #region Removes
        public static void RemChannelNsfw(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if (config.NsfwChannels.Contains(channelId))
                    {
                        config.NsfwChannels.Remove(channelId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemChannelCache(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if (config.CacheIgnoredChannels.Contains(channelId))
                    {
                        config.CacheIgnoredChannels.Remove(channelId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemWarningAction(int num, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if (config.WarningActions.Any(a => a.Warning == num))
                    {
                        config.WarningActions.Remove(config.WarningActions.FirstOrDefault(a => a.Warning == num));
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemRole(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindAll().Any(conf => conf.Id == guildId))
                {
                    GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                    if (config.AssignableRoles.Contains(roleId))
                    {
                        config.AssignableRoles.Remove(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        #endregion
    }
}
