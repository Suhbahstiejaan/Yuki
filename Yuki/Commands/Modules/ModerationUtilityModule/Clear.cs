using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Group("clear")]
        [RequireModerator]
        public class ClearCommand : YukiModule
        {
            [Command]
            public async Task Base(int amount)
            {
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();

                foreach(IMessage message in messages)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                }

                await ReplyAsync(Language.GetString("clear_result").Replace("%amount%", amount.ToString()).Replace("%exec%", Context.User.Username));
            }

            [Command("from")]
            public async Task ClearMessagesFromUserAsync(IUser user, int amount = 100)
            {
                IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(1000, CacheMode.AllowDownload, null).FlattenAsync()).Where(msg => msg.Author.Id == user.Id).Take(amount);

                foreach (IMessage message in messages)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                }

                await ReplyAsync(Language.GetString("clear_result").Replace("%amount%", amount.ToString()).Replace("%exec%", Context.User.Username));
            }

            [Command("with")]
            public async Task ClearMessagesFromUserAsync(int amount, [Remainder] string str)
            {
                IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(1000, CacheMode.AllowDownload, null).FlattenAsync()).Where(msg => msg.Content.ToLower().Contains(str.ToLower())).Take(amount);

                foreach (IMessage message in messages)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                }

                await ReplyAsync(Language.GetString("clear_result").Replace("%amount%", amount.ToString()).Replace("%exec%", Context.User.Username));
            }
        }
    }
}
