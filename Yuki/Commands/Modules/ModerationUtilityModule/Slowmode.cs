using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("slowmode")]
        [RequireModerator]
        public async Task SlowmodeAsync(string timeString = null)
        {
            if (!(Context.Channel is IDMChannel))
            {
                int seconds = 0;
                
                DateTime time = timeString.ToDateTime();

                if(time.TimeOfDay.TotalHours > 6)
                {
                    await ReplyAsync(Language.GetString("slowmode_time_long"));
                    return;
                }
                else if(time.TimeOfDay.TotalSeconds < 0)
                {
                    time.TimeOfDay.Add(TimeSpan.FromSeconds(0));
                }

                seconds = (int)time.TimeOfDay.TotalSeconds;

                if (string.IsNullOrEmpty(timeString) || seconds == 0)
                {
                    await ReplyAsync(Language.GetString("slowmode_disabled"));
                }
                else
                {
                    await ReplyAsync(Language.GetString("slowmode_enabled"));
                }

                await ((ITextChannel)Context.Channel).ModifyAsync(p =>
                {
                    p.SlowModeInterval = new Optional<int>(seconds);
                });
            }
        }
    }
}
