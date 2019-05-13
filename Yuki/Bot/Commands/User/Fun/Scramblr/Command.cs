using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Yuki.Bot.Common;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services;

namespace Yuki.Bot.Commands.User.Fun
{
    public partial class Fun_
    {
        [Command("scramblr")]
        public async Task ScrambleMessage([Remainder] string user2 = null)
        {
            Scramblr scramblr = new Scramblr();

            using (UnitOfWork uow = new UnitOfWork())
            {
                DataOptIn existing = uow.DataOptInRepository.GetUser(Context.User.Id);

                if (user2 != null)
                {
                    if (user2.ToLower() == "agree")
                    {
                        if (existing != null)
                            existing.optedIn = true;
                        else
                            uow.DataOptInRepository.Add(new DataOptIn() { optedIn = true, UserId = Context.User.Id });
                        uow.Save();
                        await ReplyAsync("Thank you for agreeing!\n\nI'll start gathering a little data starting now. Check back with me after a few messages to see if there's anything cool I can do!\n\nIf you ever wish to opt out, run \"y!scramblr optout\"");
                        return;
                    }
                    else if (user2.ToLower() == "optout")
                    {
                        if (existing != null)
                            existing.optedIn = false;
                        else
                            uow.DataOptInRepository.Add(new DataOptIn() { optedIn = false, UserId = Context.User.Id });
                        uow.Save();
                        await ReplyAsync("You've opted out of data collection. We'll clear up any saved data now!\n\nIf you ever wish to opt back in, run \"y!scramblr accept\"");

                        if (MessageCache.HasUser(Context.User.Id))
                            MessageCache.DeleteUser(Context.User.Id);
                        return;
                    }
                }

                if (existing == null || !existing.optedIn)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor("Yuki Data Collection Agreement")
                    .WithColor(Colors.Pink)
                    .WithDescription("Hello!\n\n"
                        + "Because of how this command works, and in compliance with Discord's Developer Terms of Service section 2.4, we need your permission for us to store some data from you. By typing \"y!scramblr agree\", you agree:\n\n"
                        + "1. To allow us to store the message content from your most recent " + MessageCache.maxMessagesPerUser + " messages\n"
                        + "2. To allow us to store message IDs and the ID of the channel a message was sent in\n"
                        + "3. To allow us to store your user ID\n\n"
                        + "Don't worry, though! Message content is used **only** by scramblr. Your data will be encrypted and ***cannot*** be accessed by us. As you delete or edit messages, your messages will be deleted or edited accordingly. In order to save space, if we detect no activity from you in seven days, your data will automatically be cleared.");

                    await ReplyAsync("", false, embed.Build());
                }
                else
                {
                    IGuildUser _user2 = await Context.Guild.GetUserAsync(Context.Guild.GetUserId(user2));
                    await ReplyAsync(scramblr.GetMessage(((IGuildUser)(SocketUser)Context.User), _user2));
                }
            }
        }
    }
}
