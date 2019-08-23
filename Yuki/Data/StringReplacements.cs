using Discord;
using System;
using System.Linq;
using Yuki.Commands;

namespace Yuki.Data
{
    public static class StringReplacements
    {
        public static string GetReplacement(string _string, YukiContextMessage Context)
        {
            string rebuilt = string.Empty;

            foreach(string substring in _string.Split(' ').ToList().Select(s => s.ToLower()))
            {
                string str = substring;

                if(substring.Contains("{user}"))
                {
                    str = substring.Replace("{user}", Context.User.Mention);
                }

                if(substring.Contains("{user.mention}"))
                {
                    str = substring.Replace("{user.mention}", Context.User.Mention);
                }

                if (substring.Contains("{user.id}"))
                {
                    str = substring.Replace("{user.id}", Context.User.Id.ToString());
                }

                if (substring.Contains("{user.name}"))
                {
                    str = substring.Replace("{user.name}", Context.User.Username);
                }

                if (substring.Contains("{user.discrim}"))
                {
                    str = substring.Replace("{user.discrim}", Context.User.Discriminator);
                }

                if (substring.Contains("{user.tag}"))
                {
                    str = substring.Replace("{user.tag}", $"{Context.User.Username}#{Context.User.Discriminator}");
                }

                if (substring.Contains("{user.avatar}"))
                {
                    str = substring.Replace("{user.avatar}", Context.User.GetAvatarUrl());
                }

                if (substring.Contains("{server}"))
                {
                    str = substring.Replace("{server}", Context.Guild.Name);
                }

                if (substring.Contains("{server.name}"))
                {
                    str = substring.Replace("{server.name}", Context.Guild.Name);
                }

                if (substring.Contains("{server.members}"))
                {
                    str = substring.Replace("{server.members}", Context.Guild.GetUsersAsync().Result.Count.ToString());
                }

                if (substring.Contains("{server.id}"))
                {
                    str = substring.Replace("{server.id}", Context.Guild.Id.ToString());
                }

                if (substring.Contains("{server.icon}"))
                {
                    str = substring.Replace("{server.icon}", Context.Guild.IconUrl);
                }

                if (substring.Contains("{server.region}"))
                {
                    str = substring.Replace("{server.region}", Context.Guild.VoiceRegionId);
                }


                if (substring.Contains("{server.owner}"))
                {
                    str = substring.Replace("{server.owner}", Context.Guild.GetOwnerAsync().Result.Mention);
                }

                if (substring.Contains("{server.owner.mention}"))
                {
                    str = substring.Replace("{server.owner.mention}", Context.Guild.GetOwnerAsync().Result.Mention);
                }

                if (substring.Contains("{server.owner.id}"))
                {
                    str = substring.Replace("{server.owner.id}", Context.Guild.GetOwnerAsync().Result.Id.ToString());
                }

                if (substring.Contains("{server.owner.name}"))
                {
                    str = substring.Replace("{server.owner.id}", Context.Guild.GetOwnerAsync().Result.Username);
                }

                if (substring.Contains("{server.owner.discrim}"))
                {
                    str = substring.Replace("{server.owner.discrim}", Context.Guild.GetOwnerAsync().Result.Discriminator);
                }

                if (substring.Contains("{server.owner.tag}"))
                {
                    str = substring.Replace("{server.owner.tag}", $"{Context.Guild.GetOwnerAsync().Result.Username}#{Context.Guild.GetOwnerAsync().Result.Discriminator}");
                }

                if (substring.Contains("{server.owner.avatar}"))
                {
                    str = substring.Replace("{server.owner.avatar}", Context.Guild.GetOwnerAsync().Result.GetAvatarUrl());
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
