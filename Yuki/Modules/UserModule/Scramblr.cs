using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Core.Extensions;
using Yuki.Data;
using Yuki.Data.MessageDatabase;
using Yuki.Services.Localization;

namespace Yuki.Modules.UserModule
{
    public partial class User : ModuleBase
    {
        [Command("scramblr")]
        public async Task ScramblrAsync([Remainder] string val = null)
        {
            if(!(Context.Channel is IGuildChannel))
            {
                await ReplyAsync(YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage("en_US").GetString("only_guild_channel"));
                return;
            }

            Language lang = YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage(Context);


            YukiUser user = YukiBot.Services.GetRequiredService<MDatabase>().GetUser(Context.User.Id);

            if (user.Equals(default(YukiUser)))
            {
                if(val != "optin" && val != "optout")
                {
                    Embed embed = new EmbedBuilder()
                        .WithAuthor(new EmbedAuthorBuilder() { Name = lang.GetString("scramblr_title").Replace("<username>", Context.User.Username) })
                        .WithDescription(lang.GetString("scramblr_tos"))
                        .Build();

                    await ReplyAsync("", false, embed);
                }
                else
                {
                    bool state = (val == "optin");

                    if(state)
                    {
                        user = new YukiUser()
                        {
                            Id = Context.User.Id,
                            Messages = new List<Message>()
                        };

                        YukiBot.Services.GetRequiredService<MDatabase>().Add(user);

                        await ReplyAsync(lang.Strings.scramblr_agreed);
                    }
                    else
                    {
                        YukiBot.Services.GetRequiredService<MDatabase>().Delete(Context.User.Id);

                        await ReplyAsync(lang.GetString("scramblr_opted_out"));
                    }
                }
            }
            else
            {
                ulong id2 = MentionUtils.ParseUser(val);

                if (id2 != 0)
                {
                    await ReplyAsync(new Scramblr(Context.User.Id, id2, Context.Guild).GetMessage());
                }
                else
                {
                    await ReplyAsync(new Scramblr(Context.User.Id, Context.Guild).GetMessage());
                }
            }
        }
    }
}
