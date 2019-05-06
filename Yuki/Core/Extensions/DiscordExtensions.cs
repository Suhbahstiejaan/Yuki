using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.RegularExpressions;

namespace Yuki.Core.Extensions
{
    public static class DiscordExtensions
    {
        public static string SanitizeMentions(this IGuild guild, string str, bool ignoreUserMentions = false)
        {
            string[] strs = Regex.Split(str, @"\s+", RegexOptions.Singleline);

            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] != null && strs[i].Length > 0)
                {
                    ulong Id = 0;
                    string rep = strs[i];

                    if (MentionUtils.TryParseUser(strs[i], out Id))
                    {
                        rep = YukiBot.Services.GetRequiredService<DiscordShardedClient>().GetShardFor(guild).GetUser(Id).Username;
                    }
                    else if(MentionUtils.TryParseRole(strs[i], out Id))
                    {
                        rep = guild.GetRole(Id).Name;
                    }
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
