using Discord;
using System;
using Yuki.Commands;

namespace Yuki.Data
{
    public static class StringReplacements
    {
        public static string GetReplacement(string _string, YukiContextMessage Context)
        {
            string rebuilt = string.Empty;

            foreach(string substring in _string.Split(' '))
            {
                string str;

                switch (substring.ToLower())
                {
                    #region User
                    case "{user}":
                    case "{user.mention}":
                        str = Context.User.Mention;
                        break;
                    case "{user.id}":
                        str = Context.User.Id.ToString();
                        break;
                    case "{user.name}":
                        str = Context.User.Username;
                        break;
                    case "{user.discrim}":
                        str = Context.User.Discriminator;
                        break;
                    case "{user.tag}":
                        str = $"{Context.User.Username}#{Context.User.Discriminator}";
                        break;
                    case "{user.avatar}":
                        str = Context.User.GetAvatarUrl();
                        break;
                    #endregion
                    #region Server
                    case "{server}":
                    case "{server.name}":
                        str = Context.Guild.Name;
                        break;
                    case "{server.id}":
                        str = Context.Guild.Id.ToString();
                        break;
                    case "{server.member_count}":
                        str = Context.Guild.GetUsersAsync().Result.Count.ToString();
                        break;
                    case "{server.icon}":
                        str = Context.Guild.IconUrl;
                        break;
                    case "{server.region}":
                        str = Context.Guild.VoiceRegionId;
                        break;
                    #endregion
                    #region Server Owner
                    case "{server.owner}":
                    case "{server.owner.mention}":
                        str = Context.Guild.GetOwnerAsync().Result.Mention;
                        break;
                    case "{server.owner.id}":
                        str = Context.Guild.GetOwnerAsync().Result.Id.ToString();
                        break;
                    case "{server.owner.name}":
                        str = Context.Guild.GetOwnerAsync().Result.Username;
                        break;
                    case "{server.owner.discrim}":
                        str = Context.Guild.GetOwnerAsync().Result.Discriminator;
                        break;
                    case "{server.owner.tag}":
                        str = $"{Context.Guild.GetOwnerAsync().Result.Username}#{Context.Guild.GetOwnerAsync().Result.Discriminator}";
                        break;
                    case "{server.owner.avatar}":
                        str = Context.Guild.GetOwnerAsync().Result.GetAvatarUrl();
                        break;
                    #endregion
                    default:
                        str = substring;
                        break;
                }

                rebuilt += str + " ";
            }

            /* Get rid of trailing space */
            return rebuilt.Substring(0, rebuilt.Length - 1);
        }
    }

    public class YukiContextMessage
    {
        public IUser User;
        public IGuild Guild;

        public YukiContextMessage(IUser user, IGuild guild)
        {
            User = user;
            Guild = guild;
        }
    }
}
