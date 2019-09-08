using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("disable")]
        public async Task DisableAsync([Remainder] string option)
        {
            bool found = true;

            ulong guildId = Context.Guild.Id;

            switch (option.ToLower())
            {
                case "welcome":
                    GuildSettings.ToggleWelcome(guildId, false);
                    break;
                case "goodbye":
                    GuildSettings.ToggleGoodbye(guildId, false);
                    break;
                case "nsfw":
                    GuildSettings.ToggleNsfw(guildId, false);
                    break;
                case "logging":
                    GuildSettings.ToggleLogging(guildId, false);
                    break;
                case "message cache":
                case "messagecache":
                    GuildSettings.ToggleCache(guildId, false);
                    break;
                case "selfrole":
                case "roles":
                    GuildSettings.ToggleRoles(guildId, false);
                    break;
                case "reactionrole":
                    GuildSettings.ToggleReactionRoles(guildId, false);
                    break;
                case "warnings":
                    GuildSettings.ToggleWarnings(guildId, false);
                    break;
                case "muting":
                    GuildSettings.ToggleMute(guildId, false);
                    break;
                case "filter":
                    GuildSettings.ToggleFilter(guildId, false);
                    break;
                case "starboard":
                    GuildSettings.ToggleStarboard(guildId, false);
                    break;
                default:
                    found = false;
                    break;
            }

            if (found)
            {
                await ReplyAsync(Language.GetString("setting_disabled").Replace("%settingname%", option));
            }
        }
    }
}
