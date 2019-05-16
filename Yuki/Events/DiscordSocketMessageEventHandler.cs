using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            
            int argPos = 0;

            DiscordSocketClient shard = (message.Channel is IGuildChannel) ?
                                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetShardFor(((IGuildChannel)message.Channel).Guild) :
                                            YukiBot.Services.GetRequiredService<YukiBot>().DiscordClient.GetShard(0);


            YukiUser currentUser = YukiBot.Services.GetRequiredService<MessageDB>().GetUser(message.Author.Id);

            if (!currentUser.Equals(default(YukiUser)) && currentUser.CanGetMsgs) /* Check to make sure the user exists in the db */
            {
                if(!HasPrefix(message, ref argPos))
                {
                    YukiBot.Services.GetRequiredService<MessageDB>().Add(
                        new YukiUser()
                        {
                            Id = message.Author.Id,
                            Messages = new List<YukiMessage>()
                            {
                                new YukiMessage()
                                {
                                    Id = message.Id,
                                    ChannelId = message.Channel.Id,
                                    Content = message.Content
                                }
                            }
                        }
                    );
                }
            }

            if (!HasPrefix(message, ref argPos))
                return;

            SocketCommandContext context;
            context = new SocketCommandContext(shard, message);
            IResult result = await YukiBot.Services.GetRequiredService<YukiBot>().CommandService.ExecuteAsync(context, argPos, YukiBot.Services);

            if(!result.IsSuccess)
            {
                if(result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        private static bool HasPrefix(SocketUserMessage message, ref int argPos)
        {
            foreach(string prefix in Config.GetConfig().prefix.ToArray())
            {
                if (message.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
