using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("roles")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetRolesAsync(int page = 0)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableRoles)
            {
                PageManager manager = new PageManager(config.AssignableRoles.Select(role => Context.Guild.GetRole(role).Name).ToArray(), "roles");

                await ReplyAsync(manager.GetPage(page));
            }
            else
            {
                await ReplyAsync(Language.GetString("roles_disabled"));
            }
        }
    }
}
