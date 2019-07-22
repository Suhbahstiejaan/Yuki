using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ImageModule
{
    [Name("Image")]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class ImageModule : YukiModule { }
}
