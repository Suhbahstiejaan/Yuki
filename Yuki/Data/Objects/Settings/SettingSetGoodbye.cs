using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingSetGoodbye : ISettingPage
    {
        public string Name { get; set; } = "welcome_set_goodbye";

        public async Task<IUserMessage> Display(YukiModule Module, YukiCommandContext Context, IUserMessage message)
        {
            await message.ModifyAsync(emb =>
            {
                emb.Embed = new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_goodbye_set_desc")
                    .Build();
            });

            return message;
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                GuildSettings.SetGoodbye(result.Value.Content, Context.Guild.Id);
            }
        }
    }
}
