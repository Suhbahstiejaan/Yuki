using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Common.Events;
using Yuki.Bot.Common;

namespace Yuki.Bot.Services
{
    /* Discord.Net's default cache doesn't do what I need it to */
    public class MessageCache
    {
        private static List<CachedUser> Library = new List<CachedUser>();

        public static CachedUser[] Users
            => Library.ToArray();

        /* Used for dumping and loading messages from/into memory.
         * Scramblr doesn't work too well as soon as the bot starts up,
         * this is a workaround */
        private static readonly string cacheFile = FileDirectories.AppDataDirectory + "message_cache.json";

        public static readonly int maxMessagesPerUser = 20;

        public static int Size {
            get
            {
                int size = 0;

                foreach(CachedUser user in Library)
                    size += user.Messages.Count;

                return size;
            }
        }
        
        public static List<CachedMessage> Messages(ulong userId)
        {
            List<CachedMessage> messages = new List<CachedMessage>();

            CachedUser user = Library.FirstOrDefault(x => x.UserId == userId);
            if(user != null)
                messages.AddRange(user.Messages);

            return messages;
        }

        public static void CacheMessage(SocketMessage socketMessage)
        {
            if (socketMessage.Channel is IDMChannel)
                return;

            if (socketMessage.Author.IsBot)
                return;

            IGuildChannel guildChannel = ((IGuildChannel)socketMessage.Channel);
            
            using (UnitOfWork uow = new UnitOfWork())
            {
                if ((uow.IgnoredChannelsRepository.GetIgnoredChannel(guildChannel.Id, guildChannel.GuildId) != null) ||
                    (uow.IgnoredServerRepository.GetServer(guildChannel.GuildId) != null && uow.IgnoredServerRepository.GetServer(guildChannel.GuildId).IsIgnored))
                     return;

                if(uow.DataOptInRepository.GetUser(socketMessage.Author.Id) == null ||
                   !uow.DataOptInRepository.GetUser(socketMessage.Author.Id).optedIn)
                    return;
            }
            
            int argPos = 0;
            if (MessageEvents.HasPrefix((SocketUserMessage)socketMessage, ref argPos))
                return;

            CachedMessage message = new CachedMessage()
            {
                MessageId = socketMessage.Id,
                Content = socketMessage.Content,
                ChannelId = socketMessage.Channel.Id
            };
            
            foreach (CommandInfo command in YukiClient.Instance.CommandService.Commands)
            {
                string cmd = null;

                if (command.Module.IsSubmodule)
                    cmd += command.Module.Name + " ";
                cmd += command.Name;

                if (message.Content.Contains(cmd))
                    return;
            }

            CachedUser cachedUser = Library.FirstOrDefault(x => x.UserId == socketMessage.Author.Id);
            
            if (cachedUser != null)
            {
                CachedMessage[] messages = cachedUser.Messages.ToArray();

                if (messages.Count() > maxMessagesPerUser)
                {
                    int messagesToClear = messages.Count() - maxMessagesPerUser;

                    DeleteMessagesFromUser(cachedUser.UserId, messagesToClear);
                }

                /* Check to see if a message from this user already contains the same content, if it does return */
                for (int i = 0; i < messages.Length; i++)
                    if (messages[i].Content.ToLower() == socketMessage.Content.ToLower())
                        return;

                cachedUser.LastSeenOn = DateTime.Now;
                cachedUser.Messages.Add(message);

                Library[Library.IndexOf(Library.FirstOrDefault(x => x.UserId == socketMessage.Author.Id))] = cachedUser;
            }
            else
            {
                Library.Add(new CachedUser()
                {
                    UserId = socketMessage.Author.Id,
                    LastSeenOn = DateTime.Now,
                    Messages = new List<CachedMessage>() { message }
                });
            }
        }

        public static void EditMessage(SocketMessage message)
        {
            CachedUser user = Library.FirstOrDefault(x => x.UserId == message.Author.Id);

            if(user != null)
            {
                int indexU = Library.IndexOf(user);
                CachedMessage cachedMessage = Library[indexU].Messages.FirstOrDefault(x => x.MessageId == message.Id);

                if (cachedMessage != null)
                {
                    int indexM = Library[indexU].Messages.IndexOf(cachedMessage);
                    Library[indexU].Messages[indexM].Content = message.Content;
                }
            }
        }

        public static void DeleteMessage(SocketMessage message)
        {
            CachedUser user = Library.FirstOrDefault(x => x.UserId == message.Author.Id);

            if (user != null)
            {
                int indexU = Library.IndexOf(user);
                CachedMessage cachedMessage = Library[indexU].Messages.FirstOrDefault(x => x.MessageId == message.Id);

                if (cachedMessage != null)
                {
                    int indexM = Library[indexU].Messages.IndexOf(cachedMessage);
                    Library[indexU].Messages.Remove(cachedMessage);
                }
            }
        }

        public static void DeleteMessagesFromUser(ulong userId, int amount)
        {
            CachedUser user = Library.FirstOrDefault(x => x.UserId == userId);

            if (user != null)
                Library[Library.IndexOf(user)].Messages.RemoveRange(0, amount);
        }

        public static void DeleteUser(ulong userId)
            => Library.Remove(Library[Library.IndexOf(Library.FirstOrDefault(x => x.UserId == userId))]);

        public static void GetUser(ulong userId)
            => Library.FirstOrDefault(x => x.UserId == userId);

        public static bool HasUser(ulong userId)
            => Library.FirstOrDefault(x => x.UserId == userId) != null;

        public static void DeleteMessagesFromChannel(ulong channelId)
        {
            foreach(CachedUser user in Library.ToList())
            {
                List<CachedMessage> msgs = user.Messages;
                foreach (CachedMessage message in msgs)
                    if (message.ChannelId == channelId)
                        Library[Library.IndexOf(user)].Messages.Remove(message);
            }
        }

        public static void DumpCacheToFile()
        {
            string encryptedData = Encryption.Encrypt(JsonConvert.SerializeObject(Library.ToArray(), Formatting.Indented), YukiClient.Instance.Config.EncryptionKey);
            File.WriteAllText(cacheFile, encryptedData);
        }

        public static void LoadCacheFromFile()
        {
            if(File.Exists(cacheFile))
                Library = JsonConvert.DeserializeObject<CachedUser[]>(Encryption.Decrypt(File.ReadAllText(cacheFile), YukiClient.Instance.Config.EncryptionKey)).ToList();
        }
    }

    public class CachedUser
    {
        public ulong UserId { get; set; }
        public DateTime LastSeenOn { get; set; }
        public List<CachedMessage> Messages { get; set; }
    }

    public class CachedMessage
    {
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string Content { get; set; }
    }
}