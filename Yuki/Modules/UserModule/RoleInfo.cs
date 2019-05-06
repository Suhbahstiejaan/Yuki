using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data;
using Yuki.Services.Localization;

namespace Yuki.Modules.UserModule
{
    public partial class User
    {
        [Command("roleinfo")]
        public async Task RoleInfoAsync([Remainder] string roleStr)
        {
            Language lang = YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage(Context);

            if (!(Context.Channel is IDMChannel))
            {
                IRole role = Context.Guild.Roles.FirstOrDefault(_role => _role.Name == roleStr || _role.Id.ToString() == roleStr);

                Embed embed = new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder() { Name = role.Name })
                    .WithColor(role.Color)
                    .AddField(lang.GetString("created"), (role.CreatedAt - DateTimeOffset.Now).TotalDays + lang.GetString("days_ago"))
                    .AddField(lang.GetString("role_is_hoisted"), lang.GetString(role.IsHoisted.ToString().ToLower() + "_"))
                    .AddField(lang.GetString("role_is_mentionable"), lang.GetString(role.IsMentionable.ToString().ToLower() + "_"))
                    .AddField(lang.GetString("role_position"), role.Position)
                    .AddField(lang.GetString("role_permissions"), role.Permissions)
                    .Build();

                await ReplyAsync("", false, embed);
            }
            else
            {
                await ReplyAsync(lang.GetString("only_guild_channel"));
            }
        }
    }
}
