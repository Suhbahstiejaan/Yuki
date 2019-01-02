using System;
using Yuki.Bot.Misc.Database.Repositories;

namespace Yuki.Bot.Misc.Database
{
    public class UnitOfWork : IDisposable
    {
        private YukiContext _context = new YukiContext();

#pragma warning disable 649
        private SettingRepository settingsRepository;
        private RoleRepository rolesRepository;
        private CommandRepository commandsRepository;
        private JoinLeaveMessageRepository joinLeaveMessagesRepository;
        private IgnoredChannelRepository ignoredChannelsRepository;
        private AutoAssignRoleRepository autoAssignedRolesRepository;
        private MuteRoleRepository muteRolesRepository;
        private PurgeableRepository purgeableGuildsRepository;
        private WelcomeChannelRepository welcomeChannelRepository;
        private IgnoreServerRepository ignoreServerRepository;
        private LogChannelRepository logChannelRepository;
        private WarningRepository warningRepository;
        private WarningActionRepository warningActionRepository;
        private CustomPrefixRepository customPrefixRepository;
        private DataOptInRepository dataOptInRepository;
#pragma warning restore 649

        public SettingRepository SettingsRepository
            => settingsRepository ?? new SettingRepository(_context);

        public RoleRepository RolesRepository
            => rolesRepository ?? new RoleRepository(_context);

        public CommandRepository CommandsRepository
            => commandsRepository ?? new CommandRepository(_context);

        public JoinLeaveMessageRepository JoinLeaveMessagesRepository
            => joinLeaveMessagesRepository ?? new JoinLeaveMessageRepository(_context);
        
        public IgnoredChannelRepository IgnoredChannelsRepository
            => ignoredChannelsRepository ?? new IgnoredChannelRepository(_context);
        
        public AutoAssignRoleRepository AutoAssignedRolesRepository
            => autoAssignedRolesRepository ?? new AutoAssignRoleRepository(_context);

        public MuteRoleRepository MuteRolesRepository
            => muteRolesRepository ?? new MuteRoleRepository(_context);
        
        public PurgeableRepository PurgeableGuildsRepository
            => purgeableGuildsRepository ?? new PurgeableRepository(_context);
        
        public WelcomeChannelRepository WelcomeChannelRepository
            => welcomeChannelRepository ?? new WelcomeChannelRepository(_context);
        
        public IgnoreServerRepository IgnoredServerRepository
            => ignoreServerRepository ?? new IgnoreServerRepository(_context);

        public LogChannelRepository LogChannelRepository
            => logChannelRepository ?? new LogChannelRepository(_context);

        public WarningRepository WarningRepository
            => warningRepository ?? new WarningRepository(_context);
        
        public WarningActionRepository WarningActionRepository
            => warningActionRepository ?? new WarningActionRepository(_context);

        public CustomPrefixRepository CustomPrefixRepository
            => customPrefixRepository ?? new CustomPrefixRepository(_context);

        public DataOptInRepository DataOptInRepository
            => dataOptInRepository ?? new DataOptInRepository(_context);

        public void Save()
            => _context.SaveChanges();
        
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed && disposing)
                _context.Dispose();

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
