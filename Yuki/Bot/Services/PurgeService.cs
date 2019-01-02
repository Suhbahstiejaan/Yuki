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

            if (purgeables == null || purgeables.Length < 1)
                return;

            for(int i = 0; i < purgeables.Length; i++)
            {
                Command[] commands = uow.CommandsRepository.GetCommands(purgeables[i].ServerId).ToArray();
                Setting[] settings = uow.SettingsRepository.GetSettings(purgeables[i].ServerId).ToArray();
                JoinLeaveMessage[] messages = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessages(purgeables[i].ServerId).ToArray();
                Role[] roles = uow.RolesRepository.GetRoles(purgeables[i].ServerId).ToArray();
                WelcomeChannel welcomeChannel = uow.WelcomeChannelRepository.GetChannel(purgeables[i].ServerId);

                for(int c = 0; c < commands.Length; c++)
                    uow.CommandsRepository.RemoveCommand(commands[c]);

                for(int s = 0; s < settings.Length; s++)
                    uow.SettingsRepository.RemoveSetting(settings[s]);

                for(int jlm = 0; jlm < messages.Length; jlm++)
                    uow.JoinLeaveMessagesRepository.RemoveJoinLeaveMessage(messages[jlm]);

                for(int r = 0; r < roles.Length; r++)
                    uow.RolesRepository.RemoveRole(roles[r]);

                if(welcomeChannel != null)
                    uow.WelcomeChannelRepository.RemoveChannel(welcomeChannel);

                uow.Save();
            }
        }
    }
}
