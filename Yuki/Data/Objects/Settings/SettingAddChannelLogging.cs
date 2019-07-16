using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingAddChannelLogging : ISettingPage
    {
        public string Name { get; set; } = "log_set";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_channel_add_log_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                if (MentionUtils.TryParseChannel(result.Value.Content, out ulong channelId))
                {
                    GuildSettings.AddChannelLog(channelId, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("log_added") + ": " + $"<#{channelId}>");
                }
            }
        }
    }
}
