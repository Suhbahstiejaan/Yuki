using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Services;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Common;

namespace Yuki.Bot.Modules.Moderator
{
    public class mod_SetCommands : ModuleBase
    {
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Group("set")]
        public class Set : ModuleBase
        {
            YukiRandom random = new YukiRandom();

            [Command("welcome")]
            public async Task SetJoinMessageAsync([Remainder] string joinMsg)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    JoinLeaveMessage jMsg = new JoinLeaveMessage();

                    jMsg.MsgType = JoinLeaveMessage.MessageType.Join;
                    jMsg.ServerId = Context.Guild.Id;
                    jMsg.Text = joinMsg;

                    JoinLeaveMessage existing = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessage(JoinLeaveMessage.MessageType.Join, jMsg.ServerId);
                    if(existing != null)
                    {
                        existing.Text = jMsg.Text;
                    }
                    else
                    {
                        uow.JoinLeaveMessagesRepository.AddJoinLeaveMessage(jMsg);
                    }

                    uow.Save();
                    await ReplyAsync("Welcome message set to \"" + jMsg.Text + "\".");
                }
            }

            [Command("welcomechannel")]
            public async Task SetWelcomeChannelAsync(string channelStr)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    ulong channelId = Context.Guild.GetChannelId(channelStr);
                    WelcomeChannel channel = new WelcomeChannel()
                    {
                        ServerId = Context.Guild.Id,
                        ChannelId = channelId
                    };

                    WelcomeChannel existing = uow.WelcomeChannelRepository.GetChannel(channel.ServerId);

                    if(existing != null)
                        existing = channel;
                    else
                        uow.WelcomeChannelRepository.AddChannel(channel);

                    uow.Save();
                    await ReplyAsync("Welcome channel set to <#" + channel.ChannelId + ">.");
                }
            }

            [Command("goodbye")]
            public async Task SetLeaveMessageAsync([Remainder] string leaveMsg)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    JoinLeaveMessage lMsg = new JoinLeaveMessage();

                    lMsg.MsgType = JoinLeaveMessage.MessageType.Leave;
                    lMsg.ServerId = Context.Guild.Id;
                    lMsg.Text = leaveMsg;

                    JoinLeaveMessage existing = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessage(JoinLeaveMessage.MessageType.Leave, lMsg.ServerId);
                    if(existing != null)
                    {
                        existing.Text = lMsg.Text;
                    }
                    else
                    {
                        uow.JoinLeaveMessagesRepository.AddJoinLeaveMessage(lMsg);
                    }

                    uow.Save();
                    await ReplyAsync("Goodbye message set to \"" + lMsg.Text + "\".");
                }
            }

            [Command("command")]
            public async Task SetCommandAsync([Remainder] string txt)
            {
                string[] msg = txt.Split(new char[] { ' ' }, 2);
                string action = "created";

                using(UnitOfWork uow = new UnitOfWork())
                {
                    Command cmd = new Command();
                    cmd.ServerId = Context.Guild.Id;
                    cmd.CmdName = msg[0];
                    cmd.CmdResponse = msg[1];
                    if(StringHelper.IsImage(cmd.CmdResponse))
                        cmd.CmdType = Command.CommandType.Image;
                    else
                        cmd.CmdType = Command.CommandType.Text;
                    Command existing = uow.CommandsRepository.GetCommand(cmd.CmdName, cmd.ServerId);

                    if(msg.Length < 2)
                        await ReplyAsync(random.MessageEmpty(Localizer.YukiStrings.default_lang));
                    else
                    {
                        if(cmd.CmdName.Length > 100 || cmd.CmdResponse.Length > 500)
                        {
                            await ReplyAsync("Command length exceeds character limit!");
                            return;
                        }
                        if(existing != null)
                        {
                            existing.CmdResponse = cmd.CmdResponse;
                            existing.CmdType = cmd.CmdType;
                            action = "edited";
                        }
                        else
                            uow.CommandsRepository.AddCommand(cmd);
                    }

                    uow.Save();
                    await ReplyAsync("Command **" + cmd.CmdName + "** " + action + "!");
                }
            }

            [Command("role")]
            public async Task SetRoleAsync([Remainder] string roleStr)
            {
                try
                {
                    IRole irole = Context.Guild.GetRole(roleStr);

                    if (irole == null)
                        await ReplyAsync("Role not set or invalid.");
                    else
                    {
                        Role role = new Role();
                        role.ServerId = irole.Guild.Id;
                        role.RoleId = irole.Id;

                        using (UnitOfWork uow = new UnitOfWork())
                        {
                            //Add the role to the database if it doesn't exist already
                            Role existingRole = uow.RolesRepository.GetRole(role.RoleId, role.ServerId);
                            if (existingRole == null)
                            {
                                uow.RolesRepository.AddRole(role);
                                uow.Save();
                                await ReplyAsync("Role `" + irole.Name + "` added!");
                            }
                            else
                            {
                                await ReplyAsync("Role `" + irole.Name + "` already exists in the database.");
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.Instance.Write(LogLevel.Error, e);
                }
            }

            [Command("muterole")]
            public async Task SetMuteRoleAsync([Remainder] string str)
            {
                string[] strs = str.Split(new char[] { ' ' }, 2);

                IRole role = Context.Guild.GetRole(strs[1]);

                using(UnitOfWork uow = new UnitOfWork())
                {
                    MuteRole muteRole = new MuteRole();
                    MuteRole existing = uow.MuteRolesRepository.GetMuteRole(Context.Guild.Id);

                    if(role == null)
                        await ReplyAsync("No role set!");
                    else
                    {
                        if(int.TryParse(strs[0], out int seconds))
                        {
                            TimeSpan autoTime = new TimeSpan(0, 0, seconds);
                            await ReplyAsync(strs[1] + " will be applied to muted users.");

                            if(existing == null)
                            {
                                muteRole.RoleId = role.Id;
                                muteRole.ServerId = Context.Guild.Id;

                                uow.MuteRolesRepository.AddMuteRole(muteRole);
                            }

                            uow.Save();
                        }
                        else
                            await ReplyAsync("Incorrect time format!");
                    }
                }
            }

            [Command("nocache")]
            public async Task SetNoCacheAsync(string channelStr)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    IgnoredChannel channel = new IgnoredChannel()
                    {
                        ChannelId = Context.Guild.GetChannelId(channelStr),
                        ServerId = Context.Guild.Id
                    };

                    IgnoredChannel existingChannel = uow.IgnoredChannelsRepository.GetIgnoredChannel(channel.ChannelId, Context.Guild.Id);

                    if(existingChannel != null)
                        await ReplyAsync("Messages in <#" + existingChannel.ChannelId + "> are already not being cached");

                    else
                    {
                        try
                        {
                            uow.IgnoredChannelsRepository.AddIgnoredChannel(channel);
                            uow.Save();

                            MessageCache.DeleteMessagesFromChannel(channel.ChannelId);
                        }
                        catch(Exception e) { Logger.Instance.Write(LogLevel.Error, e); }

                        await ReplyAsync("Messages in <#" + channel.ChannelId + "> will no longer be cached. Any pre-cached messages were removed.");
                    }
                }
            }

            [Command("rolecolor")]
            public async Task SetRoleColorAsync([Remainder] string roleInfo)
            {
                try
                {
                    /* role name, role color */
                    string[] split = roleInfo.Split(' ');
                    string roleColorStr = split[split.Length - 1];
                    string roleName = roleInfo.Remove(roleInfo.IndexOf(roleColorStr) - 1);

                    if(System.Drawing.ColorTranslator.FromHtml(roleColorStr) == null)
                    {
                        await ReplyAsync("Invalid color");
                        return;
                    }

                    System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(roleColorStr);

                    IRole role = Context.Guild.Roles.Where(x => x.Name == roleName).FirstOrDefault();

                    if(role != null)
                    {
                        ulong[] roleIds = Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id).Result.RoleIds.ToArray();
                        IRole[] botRoles = new IRole[roleIds.Length];

                        foreach(IRole _role in Context.Guild.Roles)
                        {
                            for(int i = 0; i < roleIds.Length; i++)
                            {
                                if(roleIds[i] == _role.Id)
                                {
                                    botRoles[i] = _role;
                                    break;
                                }
                            }
                        }

                        if(role.Position >= botRoles.OrderByDescending(x => x.Position).FirstOrDefault().Position)
                        {
                            await ReplyAsync("Cannot modify role: must be lower than my highest role");
                            return;
                        }
                        await role.ModifyAsync(x => x.Color = new Color(color.R, color.G, color.B));
                        await ReplyAsync(roleName + "'s color set to " + roleColorStr);
                    }
                    else
                        await ReplyAsync("Could not find a role with the name \"" + roleName + "\"");
                }
                catch(Exception e) { Logger.Instance.Write(LogLevel.Error, e); }
            }

            [Command("logchannel")]
            public async Task SetLogChannelAsync(string channelStr)
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    ulong channelId = Context.Guild.GetChannelId(channelStr);

                    LogChannel channel = new LogChannel()
                    {
                        ServerId = Context.Guild.Id,
                        ChannelId = channelId
                    };

                    LogChannel existing = uow.LogChannelRepository.GetChannel(channel.ServerId);

                    if(existing != null)
                        existing = channel;
                    else
                        uow.LogChannelRepository.AddChannel(channel);

                    uow.Save();
                    await ReplyAsync("Log channel set to <#" + channel.ChannelId + ">.");
                }
            }
            
            [Command("warningaction")]
            public async Task SetWarningActionAsync([Remainder] string actionStr)
            {
                try
                {
                    string[] actionStrArrr = actionStr.Split(' ');

                    if(actionStrArrr.Length < 2)
                    {
                        await ReplyAsync("Insufficient parameters. Accepted parameters:\n**\t**warningNum action\nOR\n**\t**warningNum action role\n\nAvailable actions: kick, ban, applyrole");
                        return;
                    }

                    if(int.TryParse(actionStrArrr[0], out int warnings) && warnings > 0)
                    {
                        GuildWarningAction action = new GuildWarningAction()
                        {
                            Action = actionStrArrr[1].GetEnum<WarningAction>(),
                            ServerId = Context.Guild.Id,
                            Warning = warnings
                        };

                        if(actionStrArrr.Length > 3)
                            action.RoleId = Context.Guild.GetRole(actionStrArrr[2]).Id;
                        else
                            action.RoleId = 0;

                        using(UnitOfWork uow = new UnitOfWork())
                        {
                            GuildWarningAction existing = uow.WarningActionRepository.GetAction(Context.Guild.Id, warnings);
                            if(existing != null)
                                existing = action;
                            else
                                uow.WarningActionRepository.AddAction(action);

                            uow.Save();

                            await ReplyAsync("Action " + action.Action.ToString().ToLower() + "will be performed when a user reaches " + action.Warning + " warnings.");
                        }
                    }
                    else
                        await ReplyAsync("Insufficient parameters. Accepted parameters:\n**\t**warningNum action\nOR\n**\t**warningNum action role\n\nAvailable actions: kick, ban, applyrole");
                }
                catch(Exception e) { Logger.Instance.Write(LogLevel.Error, e); }
            }

            [Command("prefix")]
            public async Task SetPrefixAsync([Remainder] string prefixStr)
            {
                try
                {
                    prefixStr = prefixStr.Replace("%20", " ");
                    bool shortened = prefixStr.Length > 3;

                    if (shortened)
                        prefixStr = prefixStr.Substring(0, 3);

                    using (UnitOfWork uow = new UnitOfWork())
                    {
                        CustomPrefix prefix = new CustomPrefix()
                        {
                            ServerId = Context.Guild.Id,
                            prefix = prefixStr
                        };

                        CustomPrefix existing = uow.CustomPrefixRepository.GetPrefix(prefix.ServerId);

                        if (existing != null)
                            uow.CustomPrefixRepository.Remove(existing);

                        uow.CustomPrefixRepository.Add(prefix);

                        uow.Save();

                        string response = "Prefix set to " + prefix.prefix + ".";

                        if (shortened)
                            response += " Your initial response exceeded the limit of 3 characters, so it has been shortened for you.";

                        await ReplyAsync(response);
                    }
                }
                catch(Exception e) { Logger.Instance.Write(LogLevel.Error, e); }
            }
        }
    }
}
