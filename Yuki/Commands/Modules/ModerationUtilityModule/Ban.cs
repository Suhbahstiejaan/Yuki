﻿using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("ban")]
        [RequireAdministrator]
        public async Task BanAsync(IUser user)
        {
            await Context.Guild.AddBanAsync(user);
            await ReplyAsync(Language.GetString("user_banned"));
        }
    }
}
