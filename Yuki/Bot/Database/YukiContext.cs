using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Data.Sqlite;
using System.IO;
using System;
using Yuki.Bot.Misc;
using Yuki.Bot.Common;

namespace Yuki.Bot.Misc.Database
{

    public class YukiContextFactory : IDesignTimeDbContextFactory<YukiContext>
    {
        private static YukiContext _context;
        
        public static void DatabaseSetupOrMigrate()
        {
            YukiContext c = new YukiContext();
            c.Database.EnsureCreated();
        }

        public YukiContext CreateDbContext(string[] args)
        {
            if(!Directory.Exists(FileDirectories.AppDataDirectory))
                Directory.CreateDirectory(FileDirectories.AppDataDirectory);

            DbContextOptionsBuilder<YukiContext> optionsBuilder = new DbContextOptionsBuilder<YukiContext>();
            SqliteConnectionStringBuilder stringBuilder = new SqliteConnectionStringBuilder("Data Source=" + FileDirectories.Database);
            optionsBuilder.UseSqlite(stringBuilder.ToString(), x => x.SuppressForeignKeyEnforcement());
            _context = new YukiContext(optionsBuilder.Options);
            _context.Database.SetCommandTimeout(60);
            //Logger.Instance.Write(LogSeverity.Info, "Performing any needed migrations....");
            _context.Database.Migrate();
            //Logger.Instance.Write(LogSeverity.Info, "Database setup!");
            return _context;
        }
        
        public static void Migrate()
        {
            _context.Database.Migrate();
        }
    }

    public class YukiContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<JoinLeaveMessage> JoinLeaveMessages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AutoAssignRole> AutoAssignedRoles { get; set; }
        public DbSet<IgnoredChannel> IgnoredChannels { get; set; }
        public DbSet<MuteRole> MuteRole { get; set; }
        public DbSet<Purgeable> Purgeable { get; set; }
        public DbSet<WelcomeChannel> WelcomeChannels { get; set; }
        public DbSet<IgnoredServer> IgnoredServers { get; set; }
        public DbSet<LogChannel> LogChannels { get; set; }
        public DbSet<WarnedUser> Warnings { get; set; }
        public DbSet<GuildWarningAction> WarningActions { get; set; }
        public DbSet<CustomPrefix> CustomPrefixes { get; set; }
        public DbSet<DataOptIn> DataCollectionOptIn { get; set; }
        public DbSet<AutoBanUser> AutoBanUsers { get; set; }
        public DbSet<NsfwChannel> NsfwChannels { get; set; }

        public YukiContext(DbContextOptions<YukiContext> options)
            : base(options)
        {
            SQLitePCL.Batteries.Init();
        }

        public YukiContext()
        {
            SQLitePCL.Batteries.Init();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=" + FileDirectories.Database);

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Command>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<Setting>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<JoinLeaveMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<AutoAssignRole>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<IgnoredChannel>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<MuteRole>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<MuteRole>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<WelcomeChannel>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<IgnoredServer>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<LogChannel>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<WarnedUser>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<GuildWarningAction>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            model.Entity<CustomPrefix>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<DataOptIn>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<AutoBanUser>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            model.Entity<NsfwChannel>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
