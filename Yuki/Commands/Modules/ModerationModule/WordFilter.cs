using Qmmands;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("filter")]
        public class WordFilter : YukiModule
        {
            [Command("add")]
            public async Task AddFilterAsync([Remainder] string filter)
            {
                GuildSettings.AddFilter(filter, Context.Guild.Id);

                await ReplyAsync(Language.GetString("filter_added"));
            }
            
            [Command("list")]
            public async Task ListFiltersAsync(int page = 0)
            {
                string[] filters = GuildSettings.GetGuild(Context.Guild.Id).WordFilter.ToArray();

                PageManager manager = new PageManager(filters, "filters");

                await ReplyAsync(manager.GetPage(page));
            }

            [Command("remove", "rem")]
            public async Task RemoveFilterAsync(int filterIndex)
            {
                GuildSettings.RemoveFilter(filterIndex--, Context.Guild.Id);

                await ReplyAsync(Language.GetString("filter_removed"));
            }
        }
    }
}
