using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("nsfwchannel")]
        public class Nsfw : YukiModule
        {
            [Command("add")]
            public async Task AddNsfwChannelAsync([Remainder] string channelName)
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
                    GuildSettings.AddChannelNsfw(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("nsfw_channel_added").Replace("%channelname%", MentionUtils.MentionChannel(channelId)));
                }
            }

            [Command("remove")]
            public async Task RemoveNsfwChannelAsync([Remainder] string channelName)
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
                    GuildSettings.RemoveChannelNsfw(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("nsfw_channel_removed").Replace("%channelname%", MentionUtils.MentionChannel(channelId)));
                }
            }
        }
    }

    [Group("blacklist")]
    public class NsfwBlacklist : YukiModule
    {
        [Command("add")]
        public async Task AddTagToBlacklistAsync([Remainder] string[] tag)
        {
            for(int i = 0; i < tag.Length; i++)
            {
                GuildSettings.BlacklistTag(tag[i], Context.Guild.Id);
            }

            await ReplyAsync(Language.GetString("nsfw_tag_blacklisted").Replace("%tag%", string.Join(", ", tag)));
        }

        [Command("remove")]
        public async Task RemoveTagFromBlacklistAsync([Remainder] string[] tag)
        {
            for (int i = 0; i < tag.Length; i++)
            {
                GuildSettings.RemoveBlacklistTag(tag[i], Context.Guild.Id);
            }

            await ReplyAsync(Language.GetString("nsfw_tag_unblacklisted").Replace("%tag%", string.Join(", ", tag)));
        }
    }
}
