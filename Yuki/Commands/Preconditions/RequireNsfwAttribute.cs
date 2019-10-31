using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Preconditions
{
    public class RequireNsfwAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext c)
        {
            YukiCommandContext context = c as YukiCommandContext;

            if(context.Channel is IDMChannel)
            {
                return CheckResult.Successful;
            }

            GuildConfiguration config = GuildSettings.GetGuild(context.Guild.Id);

            return (config.EnableNsfw && config.NsfwChannels.Contains(context.Channel.Id)) ?
                        CheckResult.Successful : CheckResult.Unsuccessful("Not an NSFW channel");
        }
    }
}
