﻿using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("starboard")]
        public class Starboard : YukiModule
        {
            [Command("requirement")]
            public async Task SetStarboardRequirementAsync(int stars)
            {
                if(stars > 0 && stars < 101)
                {
                    GuildSettings.SetStarRequirement(stars, Context.Guild.Id);
                    await ReplyAsync(Language.GetString("starboard_requirement_set"));
                }
                else
                {
                    await ReplyAsync(Language.GetString("starboard_invalid_requirement"));
                }
            }

            [Command("channel")]
            public async Task SetStarboardChannelAsync([Remainder] string channelName)
            {
                ulong channelId = 0;

                if (MentionUtils.TryParseChannel(channelName, out channelId)) { }
                else
                {
                    foreach (ITextChannel channel in (await Context.Guild.GetTextChannelsAsync()))
                    {
                        if (channel.Name.ToLower() == channelName.ToLower())
                        {
                            channelId = channel.Id;
                            break;
                        }
                    }
                }

                if (channelId == 0)
                {
                    await ReplyAsync(Language.GetString("channel_not_found").Replace("%channelname%", channelName).Replace("%user%", Context.User.Username));
                }
                else
                {
                    GuildSettings.SetStarboardChannel(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("starboard_channel_set").Replace("%channelname%", MentionUtils.MentionChannel(channelId)));
                }
            }
        }
    }
}
