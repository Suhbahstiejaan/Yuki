using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Text.RegularExpressions;

namespace Yuki.Bot.Misc.Extensions
{
    public static class Discord
    {
        public static ulong GetUserId(this IGuild guild, string user)
        {
            if (!string.IsNullOrEmpty(user))
            {
                if (ulong.TryParse(user.Replace("<@", "").Replace(">", ""), out ulong id) || ulong.TryParse(user.Replace("<@!", "").Replace(">", ""), out id))
                    return id;
                else
                {
                    int index = user.LastIndexOf('#');

                    if (index != -1)
                    {
                        if(guild != null)
                            return guild.GetUsersAsync().Result.FirstOrDefault(usr => usr.Username == user.Substring(0, index) && usr.Discriminator == user.Substring(index + 1)).Id;
                        else
                            return YukiClient.Instance.GetShard(0).Guilds.Select(x => x.Users.FirstOrDefault(usr => usr.Username == user.Substring(0, index) && usr.Discriminator == user.Substring(index + 1))).FirstOrDefault().Id;
                    }
                }
            }

            return 0;
        }

        public static bool NextMessageValid(this IMessage msg, SocketUser user, IChannel channel)
            => msg != null && msg.Channel is IGuildChannel && msg.Author == user;

        public static ulong GetChannelId(this IGuild guild, string str)
        {
            if (ulong.TryParse(str, out ulong temp))
            { }
            else
            {
                IGuildChannel channel = guild.GetChannelsAsync().Result.Where(x => x.Name == str).FirstOrDefault();
                if (channel != null)
                    temp = channel.Id;
                else
                {
                    if (ulong.TryParse(str.Replace("<#", "").Replace(">", ""), out ulong id))
                        temp = guild.GetChannelAsync(id).Result.Id;
                    else
                    {
                        IGuildChannel[] channels = guild.GetChannelsAsync().Result.Where(x => x is ITextChannel).ToArray();

                        for (int i = 0; i < channels.Length; i++)
                        {
                            if (channels[i].Name == str)
                            {
                                temp = channels[i].Id;
                                break;
                            }
                        }
                    }
                }
            }
            return temp;
        }

        public static bool CanModify(this IGuildUser executor, IGuildUser toModify)
        {
            ulong[] requestedRoleIds = toModify.RoleIds.Where(x => x != executor.Guild.EveryoneRole.Id).ToArray();
            ulong[] executorRoleIds =  executor.RoleIds.Where(x => x != executor.Guild.EveryoneRole.Id).ToArray();
            ulong[] botRoleIds = executor.Guild.GetUserAsync(YukiClient.Instance.GetShard(executor.Guild).CurrentUser.Id).Result.RoleIds.Where(x => x != executor.Guild.EveryoneRole.Id).ToArray();
            IRole[] serverRoles = executor.Guild.Roles.Where(x => x.Id != executor.Guild.EveryoneRole.Id).ToArray();
            
            if (serverRoles != null)
            {
                IRole[] requestedRoles = new IRole[requestedRoleIds.Length];
                IRole[] executorRoles = new IRole[executorRoleIds.Length];
                IRole[] botRoles = new IRole[botRoleIds.Length];

                foreach (IRole role in serverRoles)
                {
                    for (int j = 0; j < requestedRoleIds.Length; j++)
                        if (role.Id == requestedRoleIds[j])
                            requestedRoles[j] = role;

                    for (int j = 0; j < botRoleIds.Length; j++)
                        if (role.Id == botRoleIds[j])
                            botRoles[j] = role;

                    for (int j = 0; j < executorRoleIds.Length; j++)
                        if (role.Id == executorRoleIds[j])
                            executorRoles[j] = role;
                }

                requestedRoles.OrderBy(x => x.Position);
                executorRoles.OrderBy(x => x.Position);
                botRoles.OrderBy(x => x.Position);
                
                return (botRoles[0].Position > requestedRoles[0].Position && executorRoles[executorRoles.Length - 1].Position > requestedRoles[0].Position);
            }

            return executor.Guild.OwnerId == executor.Id;
        }

        public static IRole GetRole(this IGuild guild, string str)
        {
            if (ulong.TryParse(str, out ulong temp))
            { }
            else
            {
                IRole role = guild.GetRole(temp);
                if (role != null)
                    return role;
                else
                {
                    IRole[] roles = guild.Roles.ToArray();

                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (roles[i].Name.ToLower() == str.ToLower())
                            return roles[i];
                    }
                    if (ulong.TryParse(str.Replace("<#", "").Replace(">", ""), out ulong id))
                    {
                        temp = guild.GetChannelsAsync().Result.Where(x => x.Id == id).FirstOrDefault().Id;
                        role = guild.GetRole(temp);
                        if (role != null)
                            return role;
                    }
                }
            }

            return null;
        }

        public static string SanitizeMentions(this IGuild guild, string str, bool ignoreUserMentions = false)
        {
            string[] strs = Regex.Split(str, @"\s+", RegexOptions.Singleline);

            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] != null && strs[i].Length > 0)
                {
                    ulong Id = guild.GetUserId(strs[i]);
                    string rep = strs[i];
                    if (Id != 0)
                    {
                        SocketUser user = YukiClient.Instance.GetShard(guild).GetUser(Id);
                        if (user != null && !ignoreUserMentions)
                            rep = user.Username;
                        else if (guild.GetRole(Id) != null)
                            rep = guild.GetRole(Id).Name.Replace("@", "");
                    }
                    else if (rep != "")
                        rep = rep.Replace("@", "");
                    
                    strs[i] = rep;
                }
            }

            return string.Join(" ", strs);
        }

        public static string[] SanitizeMentions(this IGuild guild, string[] strs, bool ignoreUserMentions = false)
        {
            string[] toRet = strs;

            for (int i = 0; i < toRet.Length; i++)
                toRet[i] = guild.SanitizeMentions(toRet[i], ignoreUserMentions);

            return toRet;
        }
    }
}
