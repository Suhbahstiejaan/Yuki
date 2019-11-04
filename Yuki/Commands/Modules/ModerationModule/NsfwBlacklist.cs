using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("blacklist")]
        public class NsfwBlacklist : YukiModule
        {
            [Command("add")]
            public async Task AddTagToBlacklistAsync([Remainder] string[] tag)
            {
                for (int i = 0; i < tag.Length; i++)
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
}
