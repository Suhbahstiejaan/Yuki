using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    [Name("Moderation")]
    [RequireGuild]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class ModerationUtilityModule : YukiModule { }
}
