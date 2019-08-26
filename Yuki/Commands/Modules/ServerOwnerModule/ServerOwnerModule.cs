using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ServerOwnerModule
{
    [Name("Administration")]
    [RequireGuild]
    [RequireServerOwner]
    public partial class ServerOwnerModule : YukiModule { }
}
