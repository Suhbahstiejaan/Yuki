using LiteDB;
using System.Linq;
using Yuki.Data.Objects;

namespace Yuki.Services
{
    public static class GuildsDB
    {
        public const int MAX_COMMANDS = 100;
        public const int PATRON_ADDITIONAL_COMMANDS = 150;

        public static void Add(GuildConfiguration guildConfig)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.GuildsDB))
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

        public static void Update(GuildConfiguration newConfiguration)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.GuildsDB))
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
        
        public static void Delete(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.GuildsDB))
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

        public static GuildConfiguration GetConfiguration(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.GuildsDB))
            {
                LiteCollection<GuildConfiguration> collection = db.GetCollection<GuildConfiguration>();

                if(collection.FindAll().Any(config => config.Id == guildId))
                {
                    return collection.FindAll().FirstOrDefault(_conf => _conf.Id == guildId);
                }

                return default;
            }
        }
    }
}
