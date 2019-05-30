using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("viewpoll")]
        public async Task ViewPollAsync(string pollId)
        {
            try
            {
                Poll poll = PollingService.GetPoll(pollId);

                if (poll == null)
                {
                    await ReplyAsync(Language.GetString("poll_not_found").Replace("%id%", pollId));
                }

                if (!poll.UserCanVote(Context.User.Id))
                {
                    await ReplyAsync(Language.GetString("poll_not_in_server"));
                    return;
                }

                await ReplyAsync(poll.CreateEmbed(!(Context.Channel is IDMChannel) && Context.UserHasPermission(GuildPermission.ManageMessages)));
            }
            catch(Exception e)
            {
                LoggingService.Write(LogLevel.Debug, e);
            }
        }
    }
}
