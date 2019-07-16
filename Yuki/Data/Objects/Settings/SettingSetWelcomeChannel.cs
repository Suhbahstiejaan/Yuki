using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingSetWelcomeChannel : ISettingPage
    {
        public string Name { get; set; } = "welcome_set_channel";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(Module.Language.GetString("set_welcomechannel_text"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author.Id == Context.User.Id && msg.Channel.Id == Context.Channel.Id);

            if (result.IsSuccess)
            {
                if (MentionUtils.TryParseChannel(result.Value.Content, out ulong channelId))
                {
                    GuildSettings.SetWelcomeChannel(channelId, Context.Guild.Id);
                    await Module.ReplyAsync(Module.Language.GetString("welcome_added") + ": " + $"<#{channelId}>");
                }
            }
        }
    }
}
