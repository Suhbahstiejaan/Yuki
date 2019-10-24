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
        [Group("scramblr", "scrambler")]
        public class ScramblrGroup : YukiModule
        {
            [Command]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task Base(IUser user = null)
            {
                Scramblr scramblr = new Scramblr();

                if (UserSettings.CanGetMsgs(Context.User.Id))
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

            [Command("info")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ScramblrInfo()
            {
                await ReplyAsync(Language.GetString("scramblr_info"));
            }

            [Command("enable")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ScramblrEnable()
            {
                UserSettings.SetCanGetMessages(Context.User.Id, true);
                await ReplyAsync(Language.GetString("scramblr_enabled"));
            }

            [Command("disable")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ScramblrDisable()
            {
                UserSettings.SetCanGetMessages(Context.User.Id, true);

                foreach (YukiMessage message in Messages.GetFrom(Context.User.Id))
                {
                    Messages.Remove(message.Id);
                }

                await ReplyAsync(Language.GetString("scramblr_disabled"));
            }
        }
    }
}
