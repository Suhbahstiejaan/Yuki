using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Events
{
    public static class DiscordSocketMessageEventHandler
    {
        public static async Task MessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message))
                return;
            if (message.Source != MessageSource.User)
                return;
            
            DiscordSocketClient shard = (message.Channel is IGuildChannel) ?
                                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetShardFor(((IGuildChannel)message.Channel).Guild) :
                                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetShard(0);


            bool hasPrefix = HasPrefix(message, out string output);

            if (!hasPrefix)
                return;

            IResult result = await YukiBot.Services.GetRequiredService<YukiBot>().CommandService.ExecuteAsync(output, new YukiCommandContext(YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient, socketMessage as IUserMessage, YukiBot.Services));

            if(result is FailedResult failedResult)
            {
                if(!failedResult.Reason.ToLower().Contains("unknown command"))
                {
                    await message.Channel.SendMessageAsync(failedResult.Reason);
                }
            }
        }

        private static bool HasPrefix(SocketUserMessage message, out string output)
        {
            output = string.Empty;

            return CommandUtilities.HasAnyPrefix(message.Content, Config.GetConfig().prefix.AsReadOnly(), out string prefix, out output);
        }
    }
}
