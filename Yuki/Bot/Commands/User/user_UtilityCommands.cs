using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Yuki.Bot.Misc;
using Yuki.Bot.Helper;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Modules
{
    public class user_UtilityCommands : ModuleBase
    {
        private YukiRandom random = new YukiRandom();

        [Command("avatar")]
        public async Task GetAvatarAsync([Remainder] string user = null)
        {
            IGuildUser _user;

            if (user != null)
                _user = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(user));
            else
                _user = (IGuildUser)Context.User;

            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { Name = _user.Username + "#" + _user.Discriminator })
                .WithImageUrl(_user.GetAvatarUrl());

            await ReplyAsync("", false, embed.Build());
        }

        [Command("help")]
        public async Task HelpAsync([Remainder] string param = null)
            => await Help.GetHelp(Context.Channel, param, Localizer.YukiStrings.default_lang, YukiClient.Instance.DiscordClient.GetShardFor(Context.Guild));
        
        [Command("userinfo")]
        public async Task UserInfoAsync([Remainder] string usr = null)
        {
            IUser user = Context.User;
            ulong userId = Context.Guild.GetUserId(usr);

            if (userId != 0)
                user = (await Context.Guild.GetUserAsync(userId));
            else
                user = Context.User;

            await UserInfo.GetUserInfo(user, Context.Guild, Context.Channel);
        }

        [Command("serverinfo")]
        public async Task ServerInfoAsync()
        {
            if (Context.Channel is IDMChannel)
            {
                await ReplyAsync(random.NotInServer(Localizer.YukiStrings.default_lang));
                return;
            }

            int roleCount = Context.Guild.Roles.Where(x => x != Context.Guild.EveryoneRole).Select(x => x.Name).ToArray().Length;

            EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor(x => x.Name = "Info about " + Context.Guild.Name)
                    .WithThumbnailUrl(Context.Guild.IconUrl)
                    .AddField("Created", Context.Guild.CreatedAt.DateTime.YukiDateTimeString(), true)
                    .AddField("Members", Context.Guild.GetUsersAsync().Result.Count.ToString(), true)
                    .AddField("Owner", Context.Guild.GetOwnerAsync().Result.Username, true)
                    .AddField("Text Channels", Context.Guild.GetChannelsAsync().Result.Where(x => x is ITextChannel).ToArray().Length, true)
                    .AddField("Voice Channels", Context.Guild.GetChannelsAsync().Result.Where(x => x is IVoiceChannel).ToArray().Length, true);

            if (roleCount > 0)
                embed.AddField("Roles", roleCount);
            else
                embed.AddField("Roles", "None");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("roleinfo")]
        public async Task RoleInfoAsync([Remainder] string roleStr)
        {
            List<Data> data = Localizer.GetStrings(Localizer.YukiStrings.default_lang).role_info;

            if (Context.Channel is IDMChannel)
            {
                await ReplyAsync(random.NotInServer(Localizer.YukiStrings.default_lang));
                return;
            }

            foreach (IRole role in Context.Guild.Roles)
            {
                if (role.Name.ToLower() == roleStr.ToLower())
                {
                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(new EmbedAuthorBuilder() { Name = Localizer.GetLocalizedStringFromData(data, "info") + " " + role.Name });

                    List<string> u = new List<string>();

                    foreach (IGuildUser user in (await Context.Guild.GetUsersAsync()))
                        if (user.RoleIds.Contains(role.Id))
                            u.Add(user.Username);

                    string users = (u.Count > 0) ? String.Join(", ", u.ToArray()) : Localizer.GetLocalizedStringFromData(data, "none");

                    if (users.Length >= 2000)
                        users = Localizer.GetLocalizedStringFromData(data, "too_big");

                    embed.AddField(Localizer.GetLocalizedStringFromData(data, "role_id"), role.Id, true);
                    embed.AddField(Localizer.GetLocalizedStringFromData(data, "creation"), role.CreatedAt.DateTime.YukiDateTimeString(), true);
                    embed.AddField(Localizer.GetLocalizedStringFromData(data, "permission"), String.Join(", ", role.Permissions.ToList().Select(x => x.ToString())), true);
                    embed.AddField(Localizer.GetLocalizedStringFromData(data, "is_hoisted"), role.IsHoisted, true);
                    embed.AddField(Localizer.GetLocalizedStringFromData(data, "is_mentionable"), role.IsMentionable, true);
                    embed.Color = (role.Color.RawValue == Color.Default.RawValue) ? random.RandomColor : role.Color;
                    embed.AddField(Localizer.GetLocalizedStringFromData(data, "members") + " (" + u.Count + ")", users);

                    await ReplyAsync("", false, embed.Build());
                    return;
                }
            }
            await ReplyAsync(Localizer.GetLocalizedStringFromData(data, "not_found"));
        }

        [Command("ping")]
        public async Task PingAsync()
        {
            IUserMessage sent = await ReplyAsync("Pong!");
            await sent.ModifyAsync(msg => { msg.Content = "Waiting for a response..."; }); //heartbeat
            double pingMs = sent.EditedTimestamp.Value.DateTime.Subtract(sent.CreatedAt.DateTime).TotalMilliseconds;
            await sent.ModifyAsync(msg => { msg.Content = "Pong! Took " + pingMs + "ms to respond."; });
        }

        [Command("calc")]
        public async Task MathsAsync([Remainder] string mathStr)
            => await ReplyAsync("The answer is **" + mathStr.Evaluate() + "**");

        [Command("role")]
        public async Task RoleAsync([Remainder] string roleStr)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                IRole queriedRole = Context.Guild.GetRole(roleStr);
                if (queriedRole == null)
                {
                    await ReplyAsync("Role not set or invalid.");
                    return;
                }

                Role role = uow.RolesRepository.GetRole(queriedRole.Id, Context.Guild.Id);
                if (!(await Context.Guild.GetUserAsync(Context.User.Id)).RoleIds.Contains(role.RoleId))
                {
                    await (await Context.Guild.GetUserAsync(Context.User.Id)).AddRoleAsync(Context.Guild.GetRole(role.RoleId));
                    await ReplyAsync("You have joined " + queriedRole.Name + "!");
                }
                else
                {
                    await (await Context.Guild.GetUserAsync(Context.User.Id)).RemoveRoleAsync(Context.Guild.GetRole(role.RoleId));
                    await ReplyAsync("You have left " + queriedRole.Name + "!");
                }
            }
        }

        [Command("remindme")]
        public async Task RemindMeAsync([Remainder] string msg)
        {
            string[] split = msg.Split(new[] { "in" }, StringSplitOptions.None);

            if (split.Length == 2)
            {
                TimeSpan? time = StringHelper.GetTimeSpanFromString(split[1]);
                if (time.HasValue)
                {
                    Timer t = new Timer();

                    t.Interval = time.Value.TotalMilliseconds;

                    t.Elapsed += new ElapsedEventHandler((EventHandler)async delegate (object sender, EventArgs e)
                    {
                        await Context.User.SendMessageAsync(split[1]);
                        t.Stop();
                    });

                    t.Start();
                    await ReplyAsync("Got it, I'll remind you!");
                }
                else
                    await ReplyAsync("Incorrect time format.\n\nFormat: `<reminder> in <time>`");
            }
            else
                await ReplyAsync("Must specify a time and message\n\nFormat: `<reminder> in <time>`");
        }
        
        [Command("vote")]
        public async Task VoteAsync(string pollId, string option = null)
        {
            //Has the command been executed in a DM?
            if (Context.Channel is IDMChannel)
            {
                Poll poll = JSONManager.LoadPoll(pollId);
                //loop through all guilds with the command executor in it,
                IGuild guild = YukiClient.Instance.DiscordClient.GetGuild(poll.guildId);

                if (poll == null)
                {
                    await ReplyAsync("Could not find poll with ID \"" + pollId + "\".");
                    return;
                }

                //Is the votePollID parameter not identified? If it is, inform the user.
                if (poll.pollId == null)
                {
                    await ReplyAsync("Unknown poll ID.");
                    return;
                }

                if (guild.GetUserAsync(Context.User.Id) == null)
                    await ReplyAsync("Cannot vote for poll with ID \"" + pollId + "\": You're not in the server!");

                if (DateTime.Now > poll.deadline)
                {
                    await ReplyAsync("This poll has ended.");
                    return;
                }

                if (poll.participants.Contains(Context.User.Id))
                {
                    await ReplyAsync("You have already voted for this poll.");
                    return;
                }

                if(option == null)
                {
                    await ReplyAsync("Please rerun the command with your choice (option number or name) :\n" + poll.displayText);
                    return;
                }
                
                PollOption pollOptionToVoteFor = Polls.GetPollOption(poll, option);

                if (pollOptionToVoteFor == null)
                {
                    await ReplyAsync("Unknown poll option.");
                    return;
                }
                
                poll.totalVotes++;
                pollOptionToVoteFor.votes++;

                //Add the user's id to the participants list.
                poll.participants.Add(Context.User.Id);

                JSONManager.SavePoll(poll, pollId);
                await ReplyAsync("Your response has been recorded. Thank you for participating!");
                //If not, return. We don't want to do anything.
            }
            else return;
        }

        [Command("commands")]
        public async Task GetCommandsAsync(int pageNum = 1)
        {
            if (Context.Channel is IGuildChannel)
            {
                string[] commands;
                
                using (UnitOfWork uow = new UnitOfWork())
                    commands = uow.CommandsRepository.GetCommands(Context.Guild.Id).Select(cmd => cmd.CmdName).ToArray();

                PageManager manager = new PageManager(Context.Guild.SanitizeMentions(commands), "command");

                await ReplyAsync(manager.GetPage(pageNum));
            }
        }

        [Command("roles")]
        public async Task GetRolesAsync(int pageNum = 1)
        {
            if (Context.Channel is IGuildChannel)
            {
                string[] roles;
                using (UnitOfWork uow = new UnitOfWork())
                    roles = uow.RolesRepository.GetRoles(Context.Guild.Id).Select(role => Context.Guild.GetRole(role.RoleId).Name).ToArray();

                PageManager manager = new PageManager(roles, "role");

                await ReplyAsync(manager.GetPage(pageNum));
            }
        }

        [Command("reverse")]
        public async Task ReverseAsync([Remainder] string text)
        {
            char[] c = text.ToCharArray();

            Array.Reverse(c);

            await ReplyAsync(new string(c));
        }

        /*I might enable this one day
        [Command("regional")]
        public async Task RegionalTextAsync([Remainder] string txt)
        {
            txt = txt.ToLower();

            string text = "";

            string[] numbers = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            for (int i = 0; i < txt.Length; i++)
            {
                if (txt[i] >= 'a' && txt[i] <= 'z')
                    text += ":regional_indicator_" + txt[i] + ":";
                else
                {
                    if (txt[i] == ' ')
                        text += "\t";
                    else if (txt[i] == '!')
                        text += ":exclamation:";
                    else if (txt[i] >= '0' && txt[i] <= '9')
                        text += ":" + numbers[txt[i] - '0'] + ":";
                    else
                        text += txt[i];
                }
            }
            //string.Join("\t", txt.Where(y => y != ' ').Select(x => ":regional_indicator_" + x + ":"))
            await ReplyAsync(text);
        }*/
    }
}