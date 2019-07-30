using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("cache")]
        public class Cache : YukiModule
        {
            [Command("ignore")]
            public async Task IgnoreChannelAsync([Remainder] string channelName)
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
                    GuildSettings.AddChannelCache(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("cache_channel_ignored").Replace("%channelname%", MentionUtils.MentionChannel(channelId)));
                }
            }

            [Command("notice")]
            public async Task NoticeChannelAsync([Remainder] string channelName)
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
                    GuildSettings.RemoveChannelCache(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("cache_channel_noticed").Replace("%channelname%", MentionUtils.MentionChannel(channelId)));
                }
            }
        }
    }
}
