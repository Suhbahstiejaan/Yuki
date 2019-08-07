using Qmmands;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            public async Task ListFiltersAsync()
            {
                string[] filters = GuildSettings.GetGuild(Context.Guild.Id).WordFilter.ToArray();

                string filterMsg = "";

                for(int i = 0; i < filters.Length; i++)
                {
                    string filter = filters[i];

                    if(filter.Length > 180)
                    {
                        filter = filter.Substring(0, 180);
                    }

                    filterMsg += $"{i+1} {filter}\n\n";
                }

                await ReplyAsync($"```{filterMsg}```");
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
