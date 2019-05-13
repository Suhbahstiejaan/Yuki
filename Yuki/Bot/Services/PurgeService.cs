using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Services
{
    public class PurgeService
    {
        private static UnitOfWork uow = new UnitOfWork();

        public static void CheckForPurge()
        {
            Purgeable[] purgeables = uow.PurgeableGuildsRepository.GetPurgeablesOlderThan(5).ToArray();

            Console.WriteLine(purgeables.Length);

            if (purgeables == null || purgeables.Length < 1)
                return;

            try
            {
                for (int i = 0; i < purgeables.Length; i++)
                {
                    WelcomeChannel welcomeChannel = uow.WelcomeChannelRepository.GetChannel(purgeables[i].ServerId);
                    IgnoredServer ignoredServer = uow.IgnoredServerRepository.GetServer(purgeables[i].ServerId);//.ToArray();
                    MuteRole mute = uow.MuteRolesRepository.GetMuteRole(purgeables[i].ServerId);
                    LogChannel log = uow.LogChannelRepository.GetChannel(purgeables[i].ServerId);
                    CustomPrefix customPrefix = uow.CustomPrefixRepository.GetPrefix(purgeables[i].ServerId);


                    Command[] commands = uow.CommandsRepository.GetCommands(purgeables[i].ServerId).ToArray();
                    Setting[] settings = uow.SettingsRepository.GetSettings(purgeables[i].ServerId).ToArray();
                    JoinLeaveMessage[] messages = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessages(purgeables[i].ServerId).ToArray();
                    Role[] roles = uow.RolesRepository.GetRoles(purgeables[i].ServerId).ToArray();
                    IgnoredChannel[] ignoredChannels = uow.IgnoredChannelsRepository.GetIgnoredChannels(purgeables[i].ServerId).ToArray();
                    AutoAssignRole[] autoRoles = uow.AutoAssignedRolesRepository.GetRoles(purgeables[i].ServerId).ToArray();
                    WarnedUser[] users = uow.WarningRepository.GetUsers(purgeables[i].ServerId).ToArray();
                    GuildWarningAction[] actions = uow.WarningActionRepository.GetActions(purgeables[i].ServerId).ToArray();
                    
                    if (commands == null)
                        commands = new Command[1];

                    if (settings == null)
                        settings = new Setting[1];

                    if (messages == null)
                        messages = null;

                    if (roles == null)
                        roles = new Role[1];

                    if (ignoredChannels == null)
                        ignoredChannels = new IgnoredChannel[1];

                    if (autoRoles == null)
                        autoRoles = new AutoAssignRole[1];

                    if (users == null)
                        users = new WarnedUser[1];

                    if (actions == null)
                        actions = new GuildWarningAction[1];
                    
                    for (int c = 0; c < commands.Length; c++)
                        uow.CommandsRepository.RemoveCommand(commands[c]);

                    for (int s = 0; s < settings.Length; s++)
                        uow.SettingsRepository.RemoveSetting(settings[s]);

                    for (int jlm = 0; jlm < messages.Length; jlm++)
                        uow.JoinLeaveMessagesRepository.RemoveJoinLeaveMessage(messages[jlm]);

                    for (int r = 0; r < roles.Length; r++)
                        uow.RolesRepository.RemoveRole(roles[r]);

                    for (int k = 0; k < ignoredChannels.Length; k++)
                        uow.IgnoredChannelsRepository.RemoveIgnoredChannel(ignoredChannels[i]);

                    for (int k = 0; k < autoRoles.Length; k++)
                        uow.AutoAssignedRolesRepository.RemoveRole(autoRoles[i]);

                    for (int k = 0; k < users.Length; k++)
                        uow.WarningRepository.RemoveUser(users[i]);

                    for (int k = 0; k < actions.Length; k++)
                        uow.WarningActionRepository.RemoveAction(actions[i]);
                    
                    if (ignoredServer != null)
                        uow.IgnoredServerRepository.RemoveServer(ignoredServer);

                    if (customPrefix != null)
                        uow.CustomPrefixRepository.Remove(customPrefix);

                    if (mute != null)
                        uow.MuteRolesRepository.RemoveMuteRole(mute);

                    if (log != null)
                        uow.LogChannelRepository.RemoveChannel(log);


                    if (welcomeChannel != null)
                        uow.WelcomeChannelRepository.RemoveChannel(welcomeChannel);

                    uow.PurgeableGuildsRepository.RemovePurgeable(purgeables[i]);

                    uow.Save();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("done");
        }
    }
}
