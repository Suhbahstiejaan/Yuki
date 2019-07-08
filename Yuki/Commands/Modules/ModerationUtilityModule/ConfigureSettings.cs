using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("config")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ConfigureGuildSettingsAsync()
        {
            string[] settings = new[]
            {
                "config_setting_join_leave",
                "config_setting_commands",
                "config_setting_data",
                "config_setting_warnings"
            };

            EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("config_title"), true)
                .AddField(Language.GetString("config_what_do"), string.Join("\n", settings));

            await ReplyAsync(embed);
        }
    }
}
