﻿using LiteDB;
using System.Linq;
using Yuki.Data.Objects;

namespace Yuki.Services
{
    public class ConfigDB
    {
        public const int MAX_COMMANDS = 100;
        public const int PATRON_ADDITIONAL_COMMANDS = 150;

        public void Add(GuildConfiguration guildConfig)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.ConfigDB))
            {
                LiteCollection<GuildConfiguration> collection = db.GetCollection<GuildConfiguration>();

                GuildConfiguration config = collection.FindAll().FirstOrDefault(_conf => _conf.Id == guildConfig.Id);

                if (config.Equals(default(GuildConfiguration)))
                {
                    //guildConfig = MakeUnique(guildConfig);

                    collection.Insert(guildConfig);
                }
                else
                {
                    Update(guildConfig);
                }
            }
        }

        public void Update(GuildConfiguration newConfiguration)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.ConfigDB))
            {
                LiteCollection<GuildConfiguration> collection = db.GetCollection<GuildConfiguration>();

                GuildConfiguration config = collection.FindAll().FirstOrDefault(_conf => _conf.Id == newConfiguration.Id);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    collection.Update(newConfiguration);
                }
                else
                {
                    Add(newConfiguration);
                }
            }
        }
        
        public void Delete(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.ConfigDB))
            {
                LiteCollection<GuildConfiguration> collection = db.GetCollection<GuildConfiguration>();

                /* Verify the config is in the db */
                GuildConfiguration config = collection.FindAll().FirstOrDefault(_conf => _conf.Id == guildId);

                if (!config.Equals(default(GuildConfiguration)))
                {
                    collection.Delete(config.Id);
                }
            }
        }

        public GuildConfiguration GetConfiguration(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.ConfigDB))
            {
                LiteCollection<GuildConfiguration> collection = db.GetCollection<GuildConfiguration>();

                return collection.FindAll().FirstOrDefault(_conf => _conf.Id == guildId);
            }
        }
    }
}
