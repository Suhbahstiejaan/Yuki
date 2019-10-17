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

            return result;
        }
    }
}
