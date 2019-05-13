using Discord;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Extensions;

namespace Yuki.Bot.Helper
{
    public class UserInfo
    {
        private static IUser _user;
        private static IMessageChannel _channel;
        private static IGuild _guild;

        private static string GetRoleCount {
            get
            {
                int roleCount = _guild.GetUserAsync(_user.Id).Result.RoleIds.Count - 1; //We don't want to count @everyone
                return (roleCount > 0) ? "(" + roleCount + ")" : "None";
            }
        }

        private static string GetNickname {
            get
            {
                if(_channel is IDMChannel)
                    return "Not set";
                else
                    return _guild.GetUserAsync(_user.Id).Result.Nickname ?? "None";
            }
        }

        private static string GetActivityType {
            get
            {
                if(_user.Activity != null)
                    return _user.Activity.Type.ToString() + ((_user.Activity.Type == ActivityType.Listening) ? " to" : "");
                else
                    return "Currently doing";
            }
        }

        private static string GetActivityName
            => (_user.Activity != null) ? _user.Activity.Name ?? "Nothing" : "Nothing";

        private static string GetStatus
            => (_user.Status == UserStatus.DoNotDisturb) ? "Do Not Disturb" : _user.Status.ToString();

        private static string GetRoles
        {
            get
            {
                if(_channel is IDMChannel)
                    return "None";


                ulong[] uRoles = _guild.GetUserAsync(_user.Id).Result.RoleIds.Where(x => x != _guild.EveryoneRole.Id).ToArray();
                string roles = string.Join(", ", _guild.Roles.Where(x => uRoles.Contains(x.Id)).OrderBy(role => role.Name));

                if(roles.Length <= 3)
                    roles = "None";

                return roles;
            }
        }

        public static async Task GetUserInfo(IUser user, IGuild guild, IMessageChannel channel)
        {
            _user = user;
            _guild = guild;
            _channel = channel;

            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(x => x.Name = "Info about " + user.Username + "#" + user.Discriminator)
                .WithThumbnailUrl(user.GetAvatarUrl())
                .AddField("Joined Discord", user.CreatedAt.DateTime.YukiDateTimeString(), true);

            if(!(channel is IDMChannel))
                embed.AddField("Joined Server", guild.GetUserAsync(user.Id).Result.JoinedAt.Value.DateTime.YukiDateTimeString(), true);

            embed.AddField(GetActivityType, GetActivityName, true);
            embed.AddField("Nickname", GetNickname, true);
            embed.AddField("Status", GetStatus, true);
            embed.AddField("Roles " + GetRoleCount, GetRoles);

            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
