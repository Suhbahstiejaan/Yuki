using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("teamrole")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task SetTeamRoleAsync([Remainder] string args)
        {
            string[] split = args.Split(' ');

            bool state = false;

            if (!bool.TryParse(split[split.Length-1], out state)) { await ReplyAsync($"{split[split.Length - 1]} is not a valid bool!"); }

            string roleName = string.Join(' ', split.Take(split.Length - 1));
            IRole role = Context.Guild.Roles.FirstOrDefault(_role => _role.Name.ToLower() == roleName.ToLower());

            if(role != default)
            {
                if (GuildSettings.GetGuild(Context.Guild.Id).GuildRoles.Any(_role => _role.Id == role.Id))
                {
                    GuildSettings.SetTeamRole(role.Id, Context.Guild.Id, state);
                    await ReplyAsync(Language.GetString("team_role_state").Replace("%rolename%", role.Name).Replace("%status%", state.ToString()));
                }
                else
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", role.Name));
                }
            }
            else
            {
                await ReplyAsync(await ReplyAsync(Language.GetString("role_not_found").Replace("%rolename%", roleName)));
            }
        }
    }
}
