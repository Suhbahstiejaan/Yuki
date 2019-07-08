using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.NsfwModule
{
    [Name("NSFW")]
    [RequireNsfw]
    public partial class NsfwModule : YukiModule { }
}
