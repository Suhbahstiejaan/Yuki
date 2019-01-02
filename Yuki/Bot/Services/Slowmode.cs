using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Services
{
    public class SlowmodeChannel
    {
        public int slowTime;
        public ulong channelId;
    }

    public class SlowmodeUser
    {
        public ulong userId;
        public DateTimeOffset messageTimestamp;
    }

    public class Slowmode
    {
        public static Dictionary<SlowmodeChannel, List<SlowmodeUser>> data = new Dictionary<SlowmodeChannel, List<SlowmodeUser>>();

        public static async Task Check(SocketMessage message)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                SlowmodeChannel currChannel = data.FirstOrDefault(chn => chn.Key.channelId == message.Channel.Id).Key;

                bool ignoreAdmins = false;
                ITextChannel channel = (ITextChannel)message.Channel;

                Setting setting = uow.SettingsRepository.GetSetting("slowmodeAdmins", channel.GuildId);

                if (setting != null)
                    ignoreAdmins = !setting.State;

                if (data.Keys.Count > 0)
                {
                    SlowmodeUser currUser = data[currChannel].FirstOrDefault(usr => usr.userId == message.Author.Id);

                    if (currUser != null)
                    {
                        if (!message.Author.IsBot)
                        {
                            if (ignoreAdmins && (((IGuildUser)message.Author).GuildPermissions.Administrator || message.Author.Id == channel.Guild.OwnerId))
                                return;

                            IGuildChannel guildChannel = (IGuildChannel)message.Channel;
                            IGuildUser guildUser = await guildChannel.GetUserAsync(message.Author.Id);

                            if (TimeSpan.FromTicks(message.Timestamp.Ticks).TotalSeconds < (TimeSpan.FromTicks(currUser.messageTimestamp.Ticks).TotalSeconds + currChannel.slowTime) && (!guildUser.GuildPermissions.ManageMessages || !ignoreAdmins))
                                await message.DeleteAsync();
                            else
                                currUser.messageTimestamp = message.Timestamp;
                            return;
                        }
                        return;
                    }
                    else
                    {
                        SlowmodeUser slowUser = new SlowmodeUser
                        {
                            userId = message.Author.Id,
                            messageTimestamp = message.Timestamp
                        };
                        data[currChannel].Add(slowUser);
                    }
                }
            }
        }
    }
}