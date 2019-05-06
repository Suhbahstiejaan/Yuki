using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Yuki.Core.Extensions;
using Yuki.Data;
using Yuki.Services.Localization;

namespace Yuki.Modules.UserModule
{
    public partial class User : ModuleBase
    {
        [Command("avatar")]
        public async Task GetAvatarAsync([Remainder] string user = "")
        {
            Language lang = YukiBot.Services.GetRequiredService<LocalizationService>().GetLanguage(Context);

            IUser _user = Context.User;

            if (!string.IsNullOrEmpty(user) && Context.Channel is IGuildChannel &&
                ((_user = Context.Guild.GetUserAsync(MentionUtils.ParseUser(user)).Result as IUser) != null)) { }

            Embed embed = new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        {
                            Name = lang.GetString("avatar_title").Replace("<username>", _user.Username)
                        })
                    .WithImageUrl(_user.GetAvatarUrl(ImageFormat.Auto, 2048))
                    .Build();

            await ReplyAsync("", false, embed);
        }
    }
}
