using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Entity;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Common
{
    public class NsfwChannelAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                if(context.Channel is IDMChannel)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(uow.NsfwChannelRepository.GetChannels(((IGuildChannel)context.Channel).GuildId).FirstOrDefault(ch => ch.ChannelId == context.Channel.Id) != null ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("Channel must be set as NSFW!"));
            }
        }
    }
}
