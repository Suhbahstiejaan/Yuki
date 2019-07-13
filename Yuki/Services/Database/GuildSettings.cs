using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class GuildSettings
    {
        public const string path = "data/settings.db";

        private const string collection = "guild_settings";

        public static void AddOrUpdate(GuildConfiguration config)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> guilds = db.GetCollection<GuildConfiguration>(collection);

                if (!guilds.FindAll().Any(g => g.Id == config.Id))
                {
                    guilds.Insert(config);
                }
                else
                {
                    guilds.Update(config);
                }
            }
        }

        public static void Remove(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> guilds = db.GetCollection<GuildConfiguration>(collection);

                if (guilds.FindAll().Any(g => g.Id == guildId))
                {
                    guilds.Delete(guildId);
                }
            }
        }

        public static void SetWelcome(string welcomeMessage, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> guilds = db.GetCollection<GuildConfiguration>(collection);

                if (guilds.FindAll().Any(g => g.Id == guildId))
                {
                    GuildConfiguration guild = guilds.FindAll().FirstOrDefault(g => g.Id == guildId);

                    guild.WelcomeMessage = welcomeMessage;

                    guilds.Update(guild);
                }
            }
        }

        public static void SetGoodbye(string goodbyeMessage, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> guilds = db.GetCollection<GuildConfiguration>(collection);

                if (guilds.FindAll().Any(g => g.Id == guildId))
                {
                    GuildConfiguration guild = guilds.FindAll().FirstOrDefault(g => g.Id == guildId);

                    guild.WelcomeMessage = goodbyeMessage;

                    guilds.Update(guild);
                }
            }
        }

        public static void ToggleWelcome(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<GuildConfiguration> guilds = db.GetCollection<GuildConfiguration>(collection);

                if (guilds.FindAll().Any(g => g.Id == guildId))
                {
                    GuildConfiguration guild = guilds.FindAll().FirstOrDefault(g => g.Id == guildId);

                    GuildSetting setting = guild.Settings.FirstOrDefault(s => s.Name == "welcome");
                    int index = guild.Settings.IndexOf(setting);

                    setting.IsEnabled = !setting.IsEnabled;

                    guild.Settings[index] = setting;

                    guilds.Update(guild);
                }
            }
        }
    }
}
