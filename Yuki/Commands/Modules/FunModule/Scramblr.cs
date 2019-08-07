using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("scramblr", "scrambler")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ScramblrAsync(IUser user = null)
        {
            Scramblr scramblr = new Scramblr();

            if(UserSettings.CanGetMsgs(Context.User.Id))
            {
                if (user != null)
                {
                    if (UserSettings.CanGetMsgs(user.Id))
                    {
                        await ReplyAsync(scramblr.GetMessage(Context.User, user));
                    }
                    else
                    {
                        await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("%user%", $"{user.Username}#{user.Discriminator}"));
                    }
                }
                else
                {
                    await ReplyAsync(scramblr.GetMessage(Context.User));
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("%user%", $"{Context.User.Username}#{Context.User.Discriminator}"));
            }
        }

        [Command("scramblr", "scrambler")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ScramblrAsync(string text)
        {
            switch(text)
            {
                case "info":
                    await ReplyAsync(Language.GetString("scramblr_info"));
                    break;
                case "enable":
                    UserSettings.SetCanGetMessages(Context.User.Id, true);
                    await ReplyAsync(Language.GetString("scramblr_enabled"));
                    break;
                case "disable":
                    UserSettings.SetCanGetMessages(Context.User.Id, true);
                    
                    foreach(YukiMessage message in Messages.GetFrom(Context.User.Id))
                    {
                        Messages.Remove(message.Id);
                    }

                    await ReplyAsync(Language.GetString("scramblr_disabled"));
                    break;
            }
        }
    }
}
