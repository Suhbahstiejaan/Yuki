using Discord;
using Discord.WebSocket;
using Interactivity;
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
                        GuildWarningAction _action = new GuildWarningAction()
                        {
                            Warning = warning,
                            WarningAction = action,
                        };

                        if(action == WarningAction.GiveRole)
                        {
                            IUserMessage _msg = await ReplyAsync(Language.GetString("warningaction_role_name"));
                            InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                            if (result.IsSuccess)
                            {
                                string roleName = result.Value.Content;

                                IRole role = Context.Guild.Roles.FirstOrDefault(_role => _role.Name.ToLower() == roleName.ToLower());

                                if(!role.Equals(default))
                                {
                                    _action.RoleId = role.Id;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }

                        GuildSettings.AddWarningAction(_action, Context.Guild.Id);

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
