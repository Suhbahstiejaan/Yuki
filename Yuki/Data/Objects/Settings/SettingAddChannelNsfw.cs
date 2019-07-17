using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingAddChannelNsfw : ISettingPage
    {
        public string Name { get; set; } = "nsfw_add_channel";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(Module.Language.GetString("setting_channel_add_nsfw_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                if(MentionUtils.TryParseChannel(result.Value.Content, out ulong channelId))
                {
                    GuildSettings.AddChannelNsfw(channelId, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("nsfw_added") + ": " + $"<#{channelId}>");
                }
            }
        }
    }
}
