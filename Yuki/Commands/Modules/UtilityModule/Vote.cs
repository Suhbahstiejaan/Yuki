﻿using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("vote")]
        public async Task VoteAsync(string pollId, string choice = null)
        {
            if((Context.Channel is IDMChannel))
            {
                await ReplyAsync(Language.GetString("poll_not_in_server"));
                return;
            }


            Poll poll = PollingService.GetPoll(pollId);

            if(poll == null)
            {
                await ReplyAsync(Language.GetString("poll_not_found").Replace("%id%", pollId));
                return;
            }

            if(!poll.UserCanVote(Context.User.Id))
            {
                await ReplyAsync(Language.GetString("poll_not_in_server"));
                return;
            }

            if(choice == null)
            {
                await ReplyAsync(Language.GetString("poll_vote"));

                InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if(result.IsSuccess)
                {
                    choice = result.Value.Content;
                }
                else
                {
                    return;
                }
            }

            if(poll.ItemExists(choice, out PollItem item))
            {
                if(poll.HasUserVoted(Context.User.Id, out PollItem itemVoted))
                {
                    await ReplyAsync(Language.GetString("poll_already_voted"));

                    InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if(result.IsSuccess && result.Value.Content.ToLower() == "yes")
                    {
                        itemVoted.RemoveVote(Context.User.Id);
                        return;
                    }
                }
                else
                {
                    item.Vote(Context.User.Id);
                    await ReplyAsync(Language.GetString("poll_response_recorded"));
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("poll_unknown_item"));
            }
        }
    }
}
