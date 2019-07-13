using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingSetWelcome : ISettingPage
    {
        public string Name { get; set; } = "welcome_set_welcome";

        public async Task<IUserMessage> Display(YukiModule Module, YukiCommandContext Context, IUserMessage message)
        {
            await message.ModifyAsync(emb =>
            {
                emb.Embed = new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_welcome_set_desc")
                    .Build();
            });

            return message;
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if(result.IsSuccess)
            {
                GuildSettings.SetWelcome(result.Value.Content, Context.Guild.Id);
            }
        }
    }
}
