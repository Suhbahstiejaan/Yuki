using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.AdministrationModule
{
    public partial class AdministrationModule
    {
        [Command("setlang", "lang")]
        public async Task SetLangAsync(string langCode)
        {
            if(langCode == "default")
            {
                langCode = "en_US";
            }

            GuildSettings.SetLanguage(langCode, Context.Guild.Id);
            await ReplyAsync(Language.GetString("lang_set_to").Replace("%lang%", GuildSettings.GetGuild(Context.Guild.Id).LangCode));
        }
    }
}
