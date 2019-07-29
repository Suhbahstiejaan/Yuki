using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationModule
{
    [Name("Moderation")]
    [RequireGuild]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    [RequireModerator]
    public partial class ModerationUtilityModule : YukiModule { }
}
