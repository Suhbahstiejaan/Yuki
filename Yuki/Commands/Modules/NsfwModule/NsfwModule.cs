using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.NsfwModule
{
    [Name("NSFW")]
    [RequireNsfw]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class NsfwModule : YukiModule { }
}
