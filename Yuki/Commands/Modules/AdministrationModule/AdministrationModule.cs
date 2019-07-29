using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.AdministrationModule
{
    [Name("Administration")]
    [RequireGuild]
    [RequireAdministrator]
    public partial class AdministrationModule : YukiModule { }
}
