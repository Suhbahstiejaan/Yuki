using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("avatar")]
        public async Task GetAvatarAsync(IUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            string title = Language.GetString("avatar_user_avatar").Replace("%username_full%", $"{user.Username}#{user.Discriminator}");

            await ReplyAsync(Context.CreateImageEmbedBuilder(title, user.GetBigAvatarUrl()));
        }
    }
}
