using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("warnings")]
        public async Task GetWarningsAsync(IGuildUser user)
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableWarnings)
            {
                GuildWarnedUser wUser = GuildSettings.GetWarnedUser(user.Id, Context.Guild.Id);

                string warn = "";

                for(int i = 0; i < wUser.Warning; i++)
                {
                    warn += $"{i + 1}. {wUser.WarningReasons[i]}\n";
                }

                EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("warnings_list_title"))
                    .WithDescription(warn);

                await ReplyAsync(embed);
            }
            else
            {
                await ReplyAsync(Language.GetString("warnings_disabled"));
            }
        }
    }
}
