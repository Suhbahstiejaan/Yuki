using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("warningaction")]
        public class WarningActionGroup : YukiModule
        {
            [Command("add")]
            [RequireModerator]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task AddWarningActionAsync(int warning, string actionType)
            {
                if(GuildSettings.GetGuild(Context.Guild.Id).WarningActions.FirstOrDefault(x => x.Warning == warning).Equals(default))
                {
                    WarningAction action = default;

                    switch (actionType.ToLower())
                    {
                        case "giverole":
                        case "role":
                            action = WarningAction.GiveRole;
                            break;
                        case "kick":
                            action = WarningAction.Kick;
                            break;
                        case "ban":
                            action = WarningAction.Ban;
                            break;
                    }

                    if(action != default)
                    {
                        GuildSettings.AddWarningAction(new GuildWarningAction()
                        {
                            Warning = warning,
                            WarningAction = action,
                        }, Context.Guild.Id);

                        await ReplyAsync(Language.GetString("warningaction_added").Replace("%action%", action.ToString()));
                    }
                    else
                    {
                        await ReplyAsync(Language.GetString("warningaction_invalid_action"));
                    }
                }
                else
                {
                    await ReplyAsync(Language.GetString("warningaction_exists"));
                }
            }

            [Command("remove")]
            [RequireModerator]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task RemoveWarningActionAsync(int warning)
            {
                if (!GuildSettings.GetGuild(Context.Guild.Id).WarningActions.FirstOrDefault(x => x.Warning == warning).Equals(default))
                {
                    GuildSettings.RemoveWarningAction(warning, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("warningaction_removed").Replace("%action%", warning.ToString()));
                }
                else
                {
                    await ReplyAsync(Language.GetString("warningaction_not_found"));
                }
            }
        }
    }
    
}
