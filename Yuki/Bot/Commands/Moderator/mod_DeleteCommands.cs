using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Modules.Moderator
{
    public class mod_DeleteCommands : ModuleBase
    {
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Group("delete")]
        public class Delete : ModuleBase
        {
            [Command("command")]
            public async Task DeleteCommandAsync(string command)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    Command cmd = uow.CommandsRepository.GetCommand(command, Context.Guild.Id);

                    if(cmd == null)
                        await ReplyAsync("Command not found");
                    else
                    {
                        uow.CommandsRepository.RemoveCommand(cmd);
                        uow.Save();
                        await ReplyAsync(command + " deleted!");
                    }
                }
            }

            [Command("welcome")]
            public async Task RemoveWelcomeMessageAsync()
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    JoinLeaveMessage jMsg = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessage(JoinLeaveMessage.MessageType.Join, Context.Guild.Id);

                    if(jMsg == null)
                        await ReplyAsync("No welcome message has been set for this server.");
                    else
                    {
                        uow.JoinLeaveMessagesRepository.RemoveJoinLeaveMessage(jMsg);
                        uow.Save();
                        await ReplyAsync("Welcome message removed.");
                    }
                }
            }

            [Command("goodbye")]
            public async Task RemoveGoodbyeMessageAsync()
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    JoinLeaveMessage lMsg = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessage(JoinLeaveMessage.MessageType.Leave, Context.Guild.Id);

                    if(lMsg == null)
                        await ReplyAsync("No goodbye message has been set for this server.");
                    else
                    {
                        uow.JoinLeaveMessagesRepository.RemoveJoinLeaveMessage(lMsg);
                        uow.Save();
                        await ReplyAsync("Goodbye message removed.");
                    }
                }
            }

            [Command("welcomechannel")]
            public async Task RemoveWelcomeChannelAsync()
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    WelcomeChannel channel = uow.WelcomeChannelRepository.GetChannel(Context.Guild.Id);

                    if (channel == null)
                        await ReplyAsync("No welcome message has been set for this server.");
                    else
                    {
                        uow.WelcomeChannelRepository.RemoveChannel(channel);
                        uow.Save();
                        await ReplyAsync("Welcome message removed.");
                    }
                }
            }

            [Command("nocache")]
            public async Task RemoveIgnoredChannelAsync(string channelStr)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    ulong channelId = Context.Guild.GetChannelId(channelStr);

                    IgnoredChannel channel = uow.IgnoredChannelsRepository.GetIgnoredChannel(channelId, Context.Guild.Id);

                    if(channel == null)
                        await ReplyAsync("Could not find \"" + Context.Guild.GetChannelAsync(channelId).Result.Name + "\"");
                    else
                    {
                        uow.IgnoredChannelsRepository.RemoveIgnoredChannel(channel);
                        uow.Save();
                        await ReplyAsync("Messages in <#" + channel.ChannelId + "> will be cached again.");
                    }
                }
            }

            [Command("muterole")]
            public async Task RemoveMuteRoleAsync()
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    MuteRole role = uow.MuteRolesRepository.GetMuteRole(Context.Guild.Id);

                    if(role == null)
                        await ReplyAsync("No mute role has been set for this server.");
                    else
                    {
                        uow.MuteRolesRepository.RemoveMuteRole(role);
                        uow.Save();
                        await ReplyAsync("Mute role removed.");
                    }
                }
            }

            [Command("role")]
            public async Task RoleAsync(string roleStr)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    IRole iRole = Context.Guild.GetRole(roleStr);

                    if(iRole == null)
                        await ReplyAsync("Role not set or invalid.");
                    else
                    {
                        Role existingRole = uow.RolesRepository.GetRole(iRole.Id, Context.Guild.Id);
                        if(existingRole != null)
                        {
                            uow.RolesRepository.RemoveRole(existingRole);
                            uow.Save();
                            await ReplyAsync("Role " + iRole.Name + " removed!");
                        }
                        else
                        {
                            await ReplyAsync("Role " + iRole.Name + " doesn't exist in the database.");
                        }
                    }
                }
            }

            [Command("warningaction")]
            public async Task DeleteWarningActionAsync(int warningNum)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    GuildWarningAction action = uow.WarningActionRepository.GetAction(Context.Guild.Id, warningNum);

                    if(action != null)
                    {
                        uow.WarningActionRepository.RemoveAction(action);
                        await ReplyAsync("Removed action");
                        uow.Save();
                    }
                    else
                        await ReplyAsync("Could not find action!");
                }
            }


            [Command("prefix")]
            public async Task DeletePrefixAsync(int warningNum)
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    CustomPrefix prefix = uow.CustomPrefixRepository.GetPrefix(Context.Guild.Id);

                    if (prefix != null)
                    {
                        uow.CustomPrefixRepository.Remove(prefix);
                        await ReplyAsync("Removed custom prefix");
                        uow.Save();
                    }
                    else
                        await ReplyAsync("No prefix set for this server!");
                }
            }
        }
    }
}
