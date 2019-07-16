using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Preconditions
{
    public class RequireNsfwAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext c, IServiceProvider provider)
        {
            YukiCommandContext context = c as YukiCommandContext;

            CheckResult result;

            if(context.Channel is IDMChannel)
            {
                result = CheckResult.Successful;
            }
            else
            {
                GuildConfiguration config = GuildSettings.GetGuild(context.Guild.Id);

                result = (config.EnableNsfw && config.NsfwChannels.Contains(context.Channel.Id)) ?
                    CheckResult.Successful : CheckResult.Unsuccessful("Not an NSFW channel");
            }

            return Task.FromResult(result);
        }
    }
}
