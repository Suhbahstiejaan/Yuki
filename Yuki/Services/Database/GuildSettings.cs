using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class GuildSettings
    {
        private const string collection = "guild_settings";

        private static GuildConfiguration DefaultConfig(ulong guildId)
            => new GuildConfiguration()
            {
                Id = guildId,
                LangCode = "en_US",
                Prefix = null,

                EnableWelcome = false,
                EnableGoodbye = false,
                EnableNsfw = false,
                EnableCache = false,
                EnableLogging = false,
                EnableMute = false,
                EnablePrefix = false,
                EnableRoles = false,
                EnableWarnings = false,
                EnableFilter = false,
                EnableReactionRoles = false,
                EnableStarboard = false,
                EnableNegaStars = false,

                GuildRoles = new List<GuildRole>(),
                StarboardIgnoredChannels = new List<ulong>(),
                AutoBanUsers = new List<ulong>(),
                CacheIgnoredChannels = new List<ulong>(),
                LevelIgnoredChannels = new List<ulong>(),
                NsfwChannels = new List<ulong>(),
                ModeratorRoles = new List<ulong>(),
                AdministratorRoles = new List<ulong>(),
                NegaStarIgnoredChannels = new List<ulong>(),

                Commands = new List<GuildCommand>(),
                Settings = new List<GuildSetting>(),
                WarnedUsers = new List<GuildWarnedUser>(),
                WarningActions = new List<GuildWarningAction>(),
                MutedUsers = new List<GuildMutedUser>(),
                ReactableMessages = new List<ReactionMessage>(),

                NsfwBlacklist = new List<string>(),
                WordFilter = new List<string>(),

                StarRequirement = 10,
                NegaStarRequirement = 20,

                WelcomeChannel = 0,
                StarboardChannel = 0,
                LogChannel = 0,
                MuteRole = 0,

                WelcomeMessage = null,
                GoodbyeMessage = null
            };

        public static GuildConfiguration GetGuild(ulong id)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == id);
                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(id));
                    return GetGuild(id);
                }
                else
                {
                    if(config.NegaStarIgnoredChannels == null)
                    {
                        config.NegaStarIgnoredChannels = new List<ulong>();
                        configs.Update(config);
                    }

                    return config;
                }
            }
        }

        public static List<GuildConfiguration> GetGuilds()
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                return configs.FindAll().ToList();
            }
        }

        public static void AddOrUpdate(GuildConfiguration config)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (configs.FindById(config.Id).Equals(default(GuildConfiguration)))
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
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                if (!configs.FindAll().FirstOrDefault(conf => conf.Id == guildId).Equals(default(GuildConfiguration)))
                {
                    configs.Delete(guildId);
                }
            }
        }

        #region Sets
        public static void SetWelcome(string message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);
                
                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetWelcome(message, guildId);
                }

                config.WelcomeMessage = message;

                configs.Update(config);
            }
        }

        public static void SetGoodbye(string message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);
                
                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetGoodbye(message, guildId);
                }


                config.GoodbyeMessage = message;

                configs.Update(config);
            }
        }

        public static void SetMuteRole(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetMuteRole(roleId, guildId);
                }


                config.MuteRole = roleId;

                configs.Update(config);
            }
        }

        public static void SetLanguage(string langCode, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetLanguage(langCode, guildId);
                }


                config.LangCode = langCode;

                configs.Update(config);
            }
        }

        public static void SetWelcomeChannel(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetWelcomeChannel(channelId, guildId);
                }


                config.WelcomeChannel = channelId;

                configs.Update(config);
            }
        }
        #endregion

        #region Toggles
        public static void ToggleWelcome(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableWelcome = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleGoodbye(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableGoodbye = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleNsfw(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableNsfw = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleLogging(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableLogging = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleCache(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableCache = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleMute(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableMute = !config.EnableMute;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleWarnings(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableWarnings = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleRoles(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableRoles = state;

                    configs.Update(config);
                }
            }
        }

        public static void TogglePrefix(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnablePrefix = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleFilter(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableFilter = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleReactionRoles(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableReactionRoles = state;

                    configs.Update(config);
                }
            }
        }

        public static void ToggleStarboard(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableStarboard = state;

                    configs.Update(config);
                }
            }
        }
        
        public static void ToggleNegaStar(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableNegaStars = state;

                    configs.Update(config);
                }
            }
        }
        #endregion

        #region Adds
        public static void AddChannelNsfw(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
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
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.LogChannel = channelId;

                    configs.Update(config);
                }
            }
        }

        public static void AddChannelCache(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
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
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.WarningActions.Any(a => a.Warning == action.Warning))
                    {
                        config.WarningActions.Add(action);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddRole(ulong roleId, ulong guildId, bool isTeamRole)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if(config.GuildRoles == null)
                    {
                        config.GuildRoles = new List<GuildRole>();
                    }

                    if (!config.GuildRoles.Any(role => role.Id == roleId))
                    {
                        config.GuildRoles.Add(new GuildRole() { Id = roleId, IsTeamRole = isTeamRole });
                    }

                    configs.Update(config);
                }
            }
        }
        
        public static void SetTeamRole(ulong roleId, ulong guildId, bool isTeamRole)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.GuildRoles.Any(role => role.Id == roleId))
                    {
                        int index = config.GuildRoles.IndexOf(config.GuildRoles.FirstOrDefault(_role => _role.Id == roleId));

                        GuildRole role = config.GuildRoles[index];
                        role.IsTeamRole = true;

                        config.GuildRoles[index] = role;
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddPrefix(string prefix, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.Prefix = prefix;

                    configs.Update(config);
                }
            }
        }

        public static void AddWarning(ulong userId, string reason, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    GuildWarnedUser user = config.WarnedUsers.FirstOrDefault(u => u.Id == userId);

                    int index = config.WarnedUsers.IndexOf(user);

                    if (user.Equals(default))
                    {
                        user = new GuildWarnedUser()
                        {
                            Id = userId,
                            Warning = 0,
                            WarningReasons = new List<string>()
                        };
                        config.WarnedUsers.Add(user);
                        index = config.WarnedUsers.IndexOf(user);
                    }

                    user.Warning++;
                    user.WarningReasons.Add(reason);

                    config.WarnedUsers[index] = user;

                    configs.Update(config);
                }
            }
        }

        public static void AddRoleModerator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(config.Equals(default(GuildConfiguration)))
                {
                    if (!config.ModeratorRoles.Contains(roleId))
                    {
                        config.ModeratorRoles.Add(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddRoleAdministrator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.AdministratorRoles.Contains(roleId))
                    {
                        config.AdministratorRoles.Add(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddMute(GuildMutedUser user, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.MutedUsers.Any(u => u.Id == user.Id))
                    {
                        config.MutedUsers.Add(user);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void BlacklistTag(string tag, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.NsfwBlacklist.Contains(tag))
                    {
                        config.NsfwBlacklist.Add(tag);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddCommand(GuildCommand command, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.Commands.Any(c => c.Name.ToLower() == command.Name.ToLower()))
                    {
                        config.Commands.Add(command);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddFilter(string filter, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(config.Equals(default(GuildConfiguration)))
                {
                    if (config.WordFilter == null)
                    {
                        config.WordFilter = new List<string>();
                    }

                    if (!config.WordFilter.Any(str => str.ToLower() == filter.ToLower()))
                    {
                        config.WordFilter.Add(filter);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void AddReactionMessage(ReactionMessage message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.ReactableMessages == null)
                    {
                        config.ReactableMessages = new List<ReactionMessage>();
                    }

                    if (!config.ReactableMessages.Any(msg => msg.Id == message.Id))
                    {
                        config.ReactableMessages.Add(message);
                    }
                    else
                    {
                        config.ReactableMessages[config.ReactableMessages.IndexOf(config.ReactableMessages.FirstOrDefault(msg => msg.Id == message.Id))].Reactions.AddRange(message.Reactions);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void SetStarRequirement(int numStars, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.StarRequirement = numStars;

                    configs.Update(config);
                }
            }
        }
        
        public static void SetNegaStarRequirement(int numStars, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.NegaStarRequirement = numStars;

                    configs.Update(config);
                }
            }
        }

        public static void SetStarboardChannel(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.StarboardChannel = channelId;

                    configs.Update(config);
                }
            }
        }

        public static bool ToggleStarboardInChannel(ulong channelId, ulong guildId)
        {
            bool enabled = false;

            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    enabled = !config.StarboardIgnoredChannels.Contains(channelId);

                    if (!enabled)
                    {
                        config.StarboardIgnoredChannels.Remove(channelId);
                    }
                    else
                    {
                        config.StarboardIgnoredChannels.Add(channelId);
                    }

                    configs.Update(config);
                }
            }

            return enabled;
        }
        
        public static bool ToggleNegastarInChannel(ulong channelId, ulong guildId)
        {
            bool enabled = false;

            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    enabled = !config.NegaStarIgnoredChannels.Contains(channelId);

                    if (!enabled)
                    {
                        config.NegaStarIgnoredChannels.Remove(channelId);
                    }
                    else
                    {
                        config.NegaStarIgnoredChannels.Add(channelId);
                    }

                    configs.Update(config);
                }
            }

            return enabled;
        }
        #endregion

        #region Removes
        public static void RemoveChannelNsfw(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.NsfwChannels.Contains(channelId))
                    {
                        config.NsfwChannels.Remove(channelId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveChannelCache(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.CacheIgnoredChannels.Contains(channelId))
                    {
                        config.CacheIgnoredChannels.Remove(channelId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveChannelLog(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (config.Equals(default(GuildConfiguration)))
                {
                    if (config.LogChannel != 0)
                    {
                        config.LogChannel = 0;
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveWarningAction(int num, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (config.WarningActions.Any(a => a.Warning == num))
                    {
                        config.WarningActions.Remove(config.WarningActions.FirstOrDefault(a => a.Warning == num));
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveRole(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (config.GuildRoles.Any(role => role.Id == roleId))
                    {
                        config.GuildRoles.Remove(config.GuildRoles.First(role => role.Id == roleId));
                    }

                    configs.Update(config);
                }
            }
        }
        
        public static void DropRoleFromOld(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.AssignableRoles.Contains(roleId))
                    {
                        config.AssignableRoles.Remove(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveWarning(ulong userId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if(!config.Equals(default(GuildConfiguration)))
                {
                    GuildWarnedUser user = config.WarnedUsers.FirstOrDefault(u => u.Id == userId);

                    int index = config.WarnedUsers.IndexOf(user);

                    if (!user.Equals(default))
                    {
                        if(user.Warning > 0)
                        {
                            user.Warning--;
                            user.WarningReasons.Remove(user.WarningReasons[user.Warning++]);

                            config.WarnedUsers[index] = user;

                            configs.Update(config);
                        }
                    }
                }
            }
        }

        public static void RemoveRoleModerator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.ModeratorRoles.Contains(roleId))
                    {
                        config.ModeratorRoles.Remove(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveRoleAdministrator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.AdministratorRoles.Contains(roleId))
                    {
                        config.AdministratorRoles.Remove(roleId);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveMute(GuildMutedUser user, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.MutedUsers.Any(u => u.Id == user.Id))
                    {
                        config.MutedUsers.Remove(user);
                    }

                    configs.Update(config);
                }
            }
        }
        #endregion

        #region Gets

        public static GuildWarnedUser GetWarnedUser(ulong userId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    GuildWarnedUser user = config.WarnedUsers.FirstOrDefault(u => u.Id == userId);

                    int index = config.WarnedUsers.IndexOf(user);

                    if (!user.Equals(default))
                    {
                        return user;
                    }
                }

                return default;
            }
        }

        public static bool IsChannelExplicit(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    return config.NsfwChannels.Contains(channelId) && config.EnableNsfw;
                }

                return false;
            }
        }

        public static void RemoveBlacklistTag(string tag, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.NsfwBlacklist.Contains(tag))
                    {
                        config.NsfwBlacklist.Remove(tag);
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveCommand(string command, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.Commands.Any(c => c.Name.ToLower() == command.ToLower()))
                    {
                        config.Commands.Remove(config.Commands.FirstOrDefault(c => c.Name.ToLower() == command.ToLower()));
                    }

                    configs.Update(config);
                }
            }
        }

        public static void RemoveFilter(int filterIndex, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                ILiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.FindAll().FirstOrDefault(conf => conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if(filterIndex >= 0 && filterIndex < config.WordFilter.Count)
                    {
                        config.WordFilter.RemoveAt(filterIndex);
                    }

                    configs.Update(config);
                }
            }
        }
        #endregion

        #region Info

        #endregion
    }
}
