using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("roles")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetRolesAsync()
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableRoles)
            {
                string roles = "";

                for (int i = 0; i < config.AssignableRoles.Count; i++)
                {
                    roles += $"{i + 1}. {Context.Guild.GetRole(config.AssignableRoles[i]).Name}\n";
                }

                EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("roles_list_title"))
                    .WithDescription(roles);

                await ReplyAsync(embed);
            }
            else
            {
                await ReplyAsync(Language.GetString("roles_disabled"));
            }
        }
    }
}
