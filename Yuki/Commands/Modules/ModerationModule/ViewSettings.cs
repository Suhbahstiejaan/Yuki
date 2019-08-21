using Qmmands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("settings")]
        public async Task ViewSettingsAsync()
        {
            await ReplyAsync("```welcome\ngoodbye\nnsfw\nlogging\nmessage cache\nmessagecache\nselfrole\nroles\nwarnings\nmuting\nfilter```");
        }
    }
}
