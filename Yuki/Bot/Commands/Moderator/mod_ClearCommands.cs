using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.Modules.Moderator
{
    public class mod_ClearCommands : ModuleBase
    {
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [Group("clear")]
        public class Clear : ModuleBase
        {
            private YukiRandom random = new YukiRandom();

            [Command]
            public async Task BaseCommand(int toClear = 0)
            {
                int amountCleared = 0;
                int remove = toClear + 1; //increase it by one so we remove the users message + the specified amount before it

                if(toClear > 0)
                {
                    while(remove > 0)
                    {
                        int toRemove = (remove < 100) ? remove : 100;
                        IMessage[] messages = await Context.Channel.GetMessagesAsync(toRemove).Flatten().ToArray();
                        await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                        amountCleared += messages.Length;
                        remove -= toRemove;
                    }
                    await ReplyAsync(Context.User.Username + ", I removed " + (amountCleared - 1) + " message(s) for you");
                }
                else
                    await ReplyAsync(random.MessageEmpty(Localizer.YukiStrings.default_lang));
            }

            [Command("with")]
            public async Task ClearContainsAsync([Remainder] string str)
            {
                if(str != null)
                {
                    IMessage[] messages = (await Context.Channel.GetMessagesAsync(200).Flatten().ToArray()).Where(m => m.Content.Contains(str)).ToArray();
                    await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                    await ReplyAsync(Context.User.Username + ", I removed " + (messages.Count() - 1) + " message(s) for you");
                }
                else
                    await ReplyAsync(random.MessageEmpty(Localizer.YukiStrings.default_lang));
            }

            [Command("user")]
            public async Task ClearUserAsync(IGuildUser user)
            {
                if(user != null)
                {
                    IMessage[] messages = (await Context.Channel.GetMessagesAsync(200).Flatten().ToArray()).Where(x => x.Author == user).ToArray();
                    await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                    await ReplyAsync(Context.User.Username + ", I removed " + messages.Count() + " message(s) for you");
                }
                else
                    await ReplyAsync(random.MessageEmpty(Localizer.YukiStrings.default_lang));
            }

        }
    }
}
