using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Discord.Events
{
    public class GuildEvents
    {
        private Dictionary<ulong, GuildEvent> lastEvent = new Dictionary<ulong, GuildEvent>();

        /* data deletion events */
        public async Task JoinedGuild(SocketGuild guild)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Purgeable existing = uow.PurgeableGuildsRepository.GetPurgeable(guild.Id);

                if (existing != null)
                {
                    uow.PurgeableGuildsRepository.RemovePurgeable(existing);
                    uow.Save();
                }
            }

            IGuildUser[] users = guild.Users.ToArray();

            for (int i = 0; i < users.Length; i++)
                YukiClient.Instance.AddTo(YukiClient.Instance.GetShard(guild).ShardId, users[i].Id);
        }

        public async Task LeftGuild(SocketGuild guild)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Purgeable purge = new Purgeable();
                Purgeable existing = uow.PurgeableGuildsRepository.GetPurgeable(guild.Id);

                purge.LeaveDate = DateTime.Now;
                purge.ServerId = guild.Id;

                if (existing == null)
                    uow.PurgeableGuildsRepository.AddPurgeable(purge);
                else
                    existing.LeaveDate = purge.LeaveDate;
                uow.Save();
            }

            IGuildUser[] users = guild.Users.ToArray();

            for (int i = 0; i < users.Length; i++)
                YukiClient.Instance.RemoveFrom(YukiClient.Instance.GetShard(guild).ShardId, users[i].Id);
        }



        public async Task UserJoined(IGuildUser user)
        {
            Setting welcome = GetGuildSetting("welcome", user.Guild.Id);

            if (Enabled(welcome))
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    WelcomeChannel welcomeChannel = uow.WelcomeChannelRepository.GetChannel(user.Guild.Id);

                    ITextChannel channel = await user.Guild.GetTextChannelAsync(welcomeChannel.ChannelId);

                    JoinLeaveMessage msg = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessage(JoinLeaveMessage.MessageType.Join, user.GuildId);

                    if (msg != null)
                        await channel.SendMessageAsync(msg.Text.Replace("%user%", user.Username).Replace("%muser%", user.Mention));
                    else
                        await channel.SendMessageAsync(user.Mention + " has joined the server.");
                }
            }

            YukiClient.Instance.AddTo(YukiClient.Instance.GetShard(user.Guild).ShardId, user.Id);
        }

        public async Task UserLeft(IGuildUser user)
        {
            Setting goodbye = GetGuildSetting("goodbye", user.Guild.Id);

            if (Enabled(goodbye))
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    WelcomeChannel welcomeChannel = uow.WelcomeChannelRepository.GetChannel(user.Guild.Id);

                    ITextChannel channel = await user.Guild.GetTextChannelAsync(welcomeChannel.ChannelId);

                    JoinLeaveMessage msg = uow.JoinLeaveMessagesRepository.GetJoinLeaveMessage(JoinLeaveMessage.MessageType.Leave, user.GuildId);

                    if (msg != null)
                        await channel.SendMessageAsync(msg.Text.Replace("%user%", user.Username).Replace("%muser%", user.Mention));
                    else
                        await channel.SendMessageAsync(user.Mention + " has left the server.");
                }
            }

            YukiClient.Instance.RemoveFrom(YukiClient.Instance.GetShard(user.Guild).ShardId, user.Id);
        }

        public async Task UserKicked(SocketUser user, SocketUser moderator, SocketGuild guild, string reason)
        {

        }

        public async Task UserBanned(SocketUser user, SocketUser moderator, SocketGuild guild, string reason)
        {

        }

        public async Task UserBanned(SocketUser user, SocketGuild guild)
            => await UserBanned(user, null, guild, "");

        public async Task UserUnbanned(SocketUser user, SocketGuild guild)
        {

        }

        public async Task UserMute(SocketUser user, SocketUser moderator, SocketGuild guild, TimeSpan time, string reason)
        {

        }

        public async Task UserUnmute(SocketUser user, SocketUser moderator, SocketGuild guild, TimeSpan time, string reason)
        {

        }

        public async Task WarningAdded(SocketUser user, SocketUser moderator, SocketGuild guild, WarnedUser warned)
        {

        }

        public async Task WarningRemoved(SocketUser user, SocketUser moderator, SocketGuild guild, string reason, int warningNum)
        {

        }

        public async Task VoiceState(SocketUser user, SocketVoiceState state, SocketVoiceState state2)
        {

        }

        public async Task MessageEdited(Cacheable<IMessage, ulong> previous, SocketMessage current, ISocketMessageChannel channel)
        {
            await previous.DownloadAsync();

            if (!current.Author.IsBot && current.Content != previous.Value.Content)
            {
                string str = previous.Value.Content;
                if (str.Length > 1000)
                    str = str.Substring(0, 997) + "...";

                string str2 = current.Content;
                if (str2.Length > 1000)
                    str2 = str2.Substring(0, 997) + "...";

                EmbedBuilder embed = CreateLogEmbed(current.Author,
                    current.Id,
                    channel.Id,
                    "Message edited",
                    new[]
                    {
                        "Previous", str,
                        "New", str2
                    },
                    Color.Gold);

                await LogEvent(GuildEvent.MESSAGE_EDIT, ((IGuildChannel)channel).Guild, embed);
            }
        }

        public async Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            await message.DownloadAsync();

            if (!message.Value.Author.IsBot)
            {
                string str = message.Value.Content;
                if (str.Length > 1000)
                    str = str.Substring(0, 997) + "...";

                EmbedBuilder embed = CreateLogEmbed((SocketUser)message.Value.Author,
                    message.Value.Id,
                    channel.Id,
                    "Message deleted",
                    new[]
                    {
                    "Text", str ?? "",
                    },
                    Color.Red,
                    message.Value.Attachments.FirstOrDefault()?.ProxyUrl
                    );

                await LogEvent(GuildEvent.MESSAGE_DELETE, ((IGuildChannel)channel).Guild, embed);
            }
        }


        private EmbedBuilder CreateLogEmbed(SocketUser user, ulong messageId, ulong channelId, string title, string[] fields, Color color, string imgUrl = null)
        {
            EmbedBuilder embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = title,
                    IconUrl = user.GetAvatarUrl()
                },
                Description = "Message ID: " + messageId + "\nChannel: <#" + channelId + "> (" + channelId + ")",
                Footer = new EmbedFooterBuilder()
                {
                    Text = user.Username + "#" + user.Discriminator + " (" + user.Id + ")"
                },
                Color = color
            };
            
            for (int i = 0; i < fields.Length - 1; i += 2)
                if (!string.IsNullOrEmpty(fields[i]) && !string.IsNullOrEmpty(fields[i + 1]))
                {
                    string str = (fields[i + 1].Length > 2000) ? fields[i + 1].Substring(0, 1997) + "..." : fields[i + 1];
                    embed.AddField(fields[i], str);
                }

            if(imgUrl != null)
                embed.WithImageUrl(imgUrl);

            return embed;
        }

        private Setting GetGuildSetting(string settingName, ulong guildId)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                Setting setting = uow.SettingsRepository.GetSetting(settingName, guildId);
                return setting;
            }
        }

        private bool Enabled(Setting setting)
            => setting != null && setting.State;

        public async Task LogEvent(GuildEvent guildEvent, IGuild guild, EmbedBuilder embed)
        {
            Setting logging = GetGuildSetting("logging", guild.Id);

            if (Enabled(logging))
            {
                if (!lastEvent.ContainsKey(guild.Id))
                    lastEvent.Add(guild.Id, GuildEvent.NONE);

                if (lastEvent[guild.Id] != GuildEvent.USER_BAN &&
                   guildEvent == GuildEvent.USER_LEAVE)
                    return;
                else
                {
                    LogChannel channel = null;
                    using (UnitOfWork uow = new UnitOfWork())
                        channel = uow.LogChannelRepository.GetChannel(guild.Id);

                    if (channel != null)
                        await ((ITextChannel)await guild.GetChannelAsync(channel.ChannelId)).SendMessageAsync("", false, embed.Build());

                    lastEvent[guild.Id] = guildEvent;
                }
            }
        }
    }
}
