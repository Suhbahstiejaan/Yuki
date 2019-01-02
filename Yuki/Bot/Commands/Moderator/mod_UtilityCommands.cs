using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Services;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Services.Localization;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.IO;
using Yuki.Bot.Discord.Events;
using Yuki.Bot.Misc.Extensions;

namespace Yuki.Bot.Modules.Moderator
{
    public class mod_UtilityCommands : ModuleBase
    {
        public YukiRandom random = new YukiRandom();

        private GuildEvents events = new GuildEvents();
        private MuteService muteService = new MuteService();
        private Warnings warnings = new Warnings();

        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [Command("slowmode")]
        public async Task SlowmodeAsync(int time = 0)
        {
            if (time > 0)
            {
                if (Slowmode.data.Keys.Count > 0)
                {
                    foreach (SlowmodeChannel channel in Slowmode.data.Keys)
                    {
                        if (channel.channelId == Context.Channel.Id)
                            channel.slowTime = time;
                    }
                }
                else
                {
                    SlowmodeChannel chnl = new SlowmodeChannel
                    {
                        slowTime = time,
                        channelId = Context.Channel.Id
                    };

                    List<SlowmodeUser> userList = new List<SlowmodeUser>();
                    SlowmodeUser user = new SlowmodeUser
                    {
                        userId = Context.User.Id,
                        messageTimestamp = Context.Message.Timestamp
                    };
                    userList.Add(user);
                    Slowmode.data.Add(chnl, userList);
                }
                await ReplyAsync(Context.Channel.Name + " is in slowmode for " + time + " second(s).");
            }
            else
            {
                foreach (SlowmodeChannel channel in Slowmode.data.Keys)
                {
                    if (channel.channelId == Context.Channel.Id)
                    {
                        Slowmode.data.Remove(channel);
                        await ReplyAsync("Slowmode has been disabled");
                        return;
                    }
                }
                await ReplyAsync(random.SlowmodeDisabled(Localizer.YukiStrings.default_lang));
            }
        }

        [RequireUserPermission(GuildPermission.BanMembers)]
        [Command("ban")]
        public async Task BanAsync([Remainder] string banString = null)
        {
            if(banString == null)
                await ReplyAsync("You must specify a user.");
            else
            {
                IGuildUser user = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(banString.Split(' ')[0]));

                if(user == Context.User)
                {
                    await ReplyAsync("You can't ban yourself, silly!");
                    return;
                }

                if(((IGuildUser)Context.User).CanModify(user))
                {
                    string reason = banString.Replace(banString.Split(' ')[0], null) ?? "Unspecified";
                    await ReplyAsync("*" + user.Username + " has been banned.*");
                    await events.UserBanned((SocketUser)user, (SocketUser)Context.User, (SocketGuild)Context.Guild, reason);
                    await Context.Guild.AddBanAsync(user);
                }
                else
                    await ReplyAsync("Unfortunately, I cannot ban this person :/");
            }
        }

        [RequireUserPermission(GuildPermission.KickMembers)]
        [Command("kick")]
        public async Task KickAsync([Remainder] string kickString = null)
        {
            if (kickString == null)
                await ReplyAsync("You must specify a user.");
            else
            {
                IGuildUser user = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(kickString.Split(' ')[0]));

                if(user == Context.User)
                {
                    await ReplyAsync("You can't kick yourself, silly!");
                    return;
                }
                
                if(((IGuildUser)Context.User).CanModify(user))
                {
                    string reason = kickString.Remove(0, kickString.Split(' ')[0].Length) ?? "Unspecified";
                    await ReplyAsync("*" + user.Username + " has been kicked.*");
                    await events.UserKicked((SocketUser)user, (SocketUser)Context.User, (SocketGuild)Context.Guild, reason);
                    await (await Context.Guild.GetUserAsync(user.Id)).KickAsync();
                }
                else
                    await ReplyAsync("Sorry! I can't kick this person =/");
            }
        }

        [RequireUserPermission(GuildPermission.MuteMembers)] /* MuteMembers permission is for VCs */
        [Command("mute")]
        public async Task MuteAsync([Remainder] string txt)
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                //User  time (minutes)  reason
                string[] strs = txt.Split(new char[] { ' ' }, 3);

                ulong userId = Context.Guild.GetUserId(strs[0].Replace("<@", "").Replace(">", ""));

                if(userId != 0)
                {
                    IGuildUser user = await Context.Guild.GetUserAsync(userId);
                    if(!((IGuildUser)Context.User).CanModify(user))
                    {
                        await ReplyAsync("I can't mute this person =/");
                        return;
                    }

                    MutedUser mutedUser = new MutedUser();

                    mutedUser.Id = userId;
                    mutedUser.Guild = Context.Guild;
                    mutedUser.MuteChannel = (ITextChannel)Context.Channel;
                    if(int.TryParse(strs[1], out int seconds))
                        mutedUser.Time = TimeSpan.FromSeconds(seconds);
                    else
                        mutedUser.Time = TimeSpan.FromSeconds(30);
                    mutedUser.Timer = new Timer();
                    mutedUser.Timer.Interval = mutedUser.Time.TotalMilliseconds;
                    mutedUser.Timer.Elapsed += new ElapsedEventHandler((EventHandler)delegate (object sender, EventArgs e)
                    {
                        muteService.Unmute(mutedUser, (SocketUser)Context.User);
                    });

                    mutedUser.MuteReason = strs[2] ?? "Reason unspecified";

                    await user.AddRoleAsync(Context.Guild.GetRole(uow.MuteRolesRepository.GetMuteRole(Context.Guild.Id).RoleId));
                    mutedUser.Timer.Start();

                    await ReplyAsync("*Muted " + user.Username + " for " + mutedUser.Time.PrettyTime() + ".*");
                    muteService.Mute(mutedUser, (SocketUser)Context.User);
                    await events.UserMute((SocketUser)user, (SocketUser)Context.User, (SocketGuild)Context.Guild, mutedUser.Time, mutedUser.MuteReason);
                }
                else
                    await ReplyAsync("No user specified!");
            }
        }

        [RequireUserPermission(GuildPermission.MuteMembers)] /* MuteMembers permission is for VCs */
        [Command("unmute")]
        public async Task UnmuteAsync([Remainder] string txt)
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                //User  reason
                string[] strs = txt.Split(new char[] { ' ' }, 2);

                ulong userId = Context.Guild.GetUserId(strs[0].Replace("<@", "").Replace(">", ""));
                string reason = strs[2];
                
                if(userId != 0)
                {
                    IGuildUser user = await Context.Guild.GetUserAsync(userId);
                    if(!((IGuildUser)Context.User).CanModify(user))
                    {
                        await ReplyAsync("I can't unmute this person =/");
                        return;
                    }

                    MutedUser _user = muteService.GetUser(userId);
                    muteService.Unmute(_user, (SocketUser)Context.User);
                    reason = strs[2] ?? "Reason unspecified";

                    await ReplyAsync("*" + user.Username + " has been unmuted.*");
                    await events.UserUnmute((SocketUser)user, (SocketUser)Context.User, (SocketGuild)Context.Guild, _user.Time, _user.MuteReason);
                }
                else
                    await ReplyAsync("No user specified!");
            }
        }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Command("toggle")]
        public async Task Toggle([Remainder] string setting = null)
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                switch(setting)
                {
                    case "slowmodeAdmins":
                    case "logging":
                    case "welcome":
                    case "goodbye":
                        Setting _setting = new Setting() { Name = setting, ServerId = Context.Guild.Id };
                        Setting currSetting = uow.SettingsRepository.GetSetting(setting, Context.Guild.Id);
                        if(currSetting == null)
                        {
                            _setting.State = true;
                            uow.SettingsRepository.AddSetting(_setting);

                            if (setting == "welcome")
                                uow.SettingsRepository.AddSetting(new Setting() { Name = "goodbye", ServerId = _setting.ServerId, State = true });
                        }
                        else
                        {
                            _setting = currSetting;
                            _setting.State = !_setting.State;
                        }
                        uow.Save();
                        string state = (_setting.State) ? "enabled" : "disabled";

                        await ReplyAsync(setting + " has been **" + state + "**.");
                        break;
                    default:
                        await ReplyAsync(random.MessageEmpty(Localizer.YukiStrings.default_lang));
                        return;
                }
            }
        }
        
        [Command("ignoreserver")]
        public async Task IgnoreServerAsync()
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                IgnoredServer server = new IgnoredServer()
                {
                    IsIgnored = true,
                    ServerId = Context.Guild.Id
                };

                IgnoredServer existing = uow.IgnoredServerRepository.GetServer(server.ServerId);

                if(existing != null)
                    existing.IsIgnored = !existing.IsIgnored;
                else
                    uow.IgnoredServerRepository.AddServer(server);

                uow.Save();

                string state = (existing.IsIgnored) ? "will be ignored" : "will no longer be ignored";

                await ReplyAsync("Channels in this server " + state);
            }
        }

        [RequireUserPermission(GuildPermission.KickMembers)]
        [Command("warn")]
        public async Task WarnAsync([Remainder] string warnString = null)
        {
            if(warnString == null)
                await ReplyAsync("You must specify a user.");
            else
            {
                IGuildUser user = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(warnString.Split(' ')[0]));
                string reason = warnString.Remove(0, warnString.Split(' ')[0].Length).Replace("|", "%$#");
                
                if(String.IsNullOrEmpty(reason))
                    reason = "Unspecified";

                using(UnitOfWork uow = new UnitOfWork())
                {
                    WarnedUser warned = uow.WarningRepository.GetUser(Context.Guild.Id, user.Id);

                    if(user == Context.User || !((IGuildUser)Context.User).CanModify(user))
                    {
                        await ReplyAsync("I can't warn " + user.Username);
                        return;
                    }

                    if(warned == null)
                    {
                        warned = new WarnedUser() { ServerId = Context.Guild.Id, UserId = user.Id, Warning = 1, WarnReasons = reason + "|" };
                        uow.WarningRepository.AddUser(warned);
                    }
                    else
                    {
                        string[] warns = warned.WarnReasons.Split('|');
                        if(warns.Length <= 5)
                            warned.WarnReasons += reason + "|";
                        else
                        {
                            List<string> warnList = warns.ToList();
                            warnList.Remove(warns[0]);
                            warnList.Add(reason);
                            warned.WarnReasons = string.Join("|", warnList.ToArray());
                        }
                        if(warned.Warning < 5)
                            warned.Warning++;
                        else
                        {
                            await ReplyAsync(user.Username + "#" + user.Discriminator + " has the max amount of warnings allowed");
                            return;
                        }
                    }

                    uow.Save();

                    await ReplyAsync("*" + user.Username + "#" + user.Discriminator + " has been warned. Warnings: " + warned.Warning + "*");
                    await events.WarningAdded((SocketUser)user, (SocketUser)Context.User, (SocketGuild)Context.Guild, warned);
                    
                    await warnings.Act(warned);
                }
            }
        }

        [RequireUserPermission(GuildPermission.KickMembers)]
        [Command("unwarn")]
        public async Task UnwarnAsync([Remainder] string warnString = null)
        {
            if(warnString == null)
                await ReplyAsync("You must specify a user.");
            else
            {
                IGuildUser user = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(warnString.Split(' ')[0]));

                using(UnitOfWork uow = new UnitOfWork())
                {
                    WarnedUser warned = uow.WarningRepository.GetUser(Context.Guild.Id, user.Id);

                    if(warned != null)
                    {
                        if(user == Context.User || !((IGuildUser)Context.User).CanModify(user))
                        {
                            await ReplyAsync("I can't remove a warning from this person!");
                            return;
                        }
                        if(warned.Warning <= 0)
                            uow.WarningRepository.RemoveUser(warned);
                        else
                            warned.Warning--;

                        uow.Save();

                        string reason = warnString.Remove(0, warnString.Split(' ')[0].Length);
                        
                        if(String.IsNullOrEmpty(reason))
                            reason = "Unspecified";

                        await ReplyAsync("*" + user.Username + " has had a warning removed. Warnings: " + warned.Warning + "*");

                        await events.WarningRemoved((SocketUser)user, (SocketUser)Context.User, (SocketGuild)Context.Guild, reason, warned.Warning);
                    }
                }
            }
        }

        [RequireUserPermission(GuildPermission.KickMembers)]
        [Command("warnings")]
        public async Task WarningsAsync([Remainder] string str = "1")
        {
            if (int.TryParse(str, out int pageNum))
            {
                string[] users = null;

                using (UnitOfWork uow = new UnitOfWork())
                {
                    if (uow.WarningRepository.GetUsers(Context.Guild.Id) != null)
                        users = uow.WarningRepository.GetUsers(Context.Guild.Id).Select(user => Context.Guild.GetUserAsync(user.UserId).Result.Username + "#" + Context.Guild.GetUserAsync(user.UserId).Result.Discriminator +
                                                                                                "\tWarnings: " + user.Warning).ToArray();
                }

                PageManager manager = new PageManager(users, "warned user");
                await ReplyAsync(manager.GetPage(pageNum));
            }
            else
            {
                ulong userId = Context.Guild.GetUserId(str);

                if (userId != 0)
                {
                    IGuildUser guildUser = await Context.Guild.GetUserAsync(userId);

                    WarnedUser user = null;
                    using (UnitOfWork uow = new UnitOfWork())
                        user = uow.WarningRepository.GetUser(Context.Guild.Id, userId);

                    if (user != null)
                    {
                        string warn = "";
                        string[] warnings = user.WarnReasons.Split('|');

                        for (int i = 0; i < warnings.Length; i++)
                            warn += "*" + (i + 1) + ". " + warnings[i].Replace("%$#", "|") + "*\n";

                        await ReplyAsync(guildUser.Username + "#" + guildUser.Discriminator + " has " + user.Warning + " warning(s).\nPrevious " + warnings.Length + " warnings:\n" + warn);
                    }
                }
            }
        }
        
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("createpoll")]
        public async Task CreatePollAsync([Remainder] string parameters = null)
        {
            try
            {
                if(!string.IsNullOrEmpty(parameters))
                {
                    /* order: name, options, end time */
                    string[] _params = Regex.Split(parameters, @"\s*[|]\s*");
                    string[] pollOptions = Regex.Split(_params[1], @"\s*[,]\s*");

                    if (pollOptions.Length < 2)
                    {
                        await ReplyAsync("Insufficient options! Must have at least two.");
                        return;
                    }

                    TimeSpan? t = StringHelper.GetTimeSpanFromString(_params[2]);

                    if (t.HasValue && t.Value.TotalSeconds > 0)
                    {
                        TimeSpan time = t.Value;

                        if (!(time.TotalDays <= 14 && time.TotalSeconds != 0))
                        {
                            await ReplyAsync("Time must be less than 14 days and greater than 0 seconds");
                            return;
                        }


                        DateTime dt = DateTime.Now.Add(time);
                        Poll poll = new Poll(_params[0], pollOptions, dt, Context.Guild.Id);
                        Polls.AddPoll(poll);
                        JSONManager.SavePoll(poll, poll.pollId);
                        await ReplyAsync("Poll created successfully!\n\nName: " + _params[0] + "\nOptions: " + String.Join(", ", pollOptions) + "\nEnd date: " + dt.ToShortDateString() + "\nPoll ID: " + poll.pollId);
                    }
                    else
                        await ReplyAsync("Invalid time!");
                }
            }
            catch (Exception e) { Logger.GetLoggerInstance().Write(Misc.LogSeverity.Error, e); }
        }
        
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("stats")]
        public async Task PollStatsAsync(string pollId)
        {
            Poll poll = Polls.GetPoll(pollId);

            //Is the pollArg parameter not identified? If it is, inform the user.
            if (poll.pollId == null)
            {
                await ReplyAsync("Unknown poll ID.");
                return;
            }


            //delete the poll file if its time is up
            if (TimeSpan.FromTicks(poll.deadline.Ticks).TotalMilliseconds <= TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds)
                File.Delete(FileDirectories.AppDataDirectory + "polls\\" + poll.pollId + ".json");

            await ReplyAsync(poll.statsText + $"\nTotal votes: " + poll.totalVotes);
        }
    }
}