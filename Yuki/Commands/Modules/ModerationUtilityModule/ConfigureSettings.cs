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
                "config_setting_assignable_roles",
                "config_setting_mute",
                "config_setting_scramblr",
                "config_setting_nsfw",
                "config_setting_prefix",
                "config_setting_warnings",
                "config_setting_log"
            };

            EmbedBuilder embed = Context.CreateEmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                {
                    IconUrl = Context.User.GetAvatarUrl(),
                    Name = Language.GetString("config_title")
                })
                .AddField(Language.GetString("config_what_do"), string.Join("\n", settings));

            await ReplyAsync(embed);
        }
    }
}
