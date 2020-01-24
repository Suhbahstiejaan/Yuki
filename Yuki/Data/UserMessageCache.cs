using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yuki.Data.Objects;
using Yuki.Events;
using Yuki.Services.Database;

namespace Yuki.Data
{
    public static class UserMessageCache
    {
        public static readonly int MaxMessages = 100;

        private static List<YukiMessage> Messages = new List<YukiMessage>();

        public static void AddOrUpdate(SocketMessage message)
        {
            if(message.Channel is IDMChannel)
            {
                return;
            }

            if(message.Author.IsBot)
            {
                return;
            }

            IGuild guild = (message.Channel as IGuildChannel).Guild;

            if(GuildSettings.GetGuild(guild.Id).CacheIgnoredChannels.Contains(message.Channel.Id))
            {
                return;
            }

            if(DiscordSocketEventHandler.HasPrefix(message as SocketUserMessage, out string _prefix))
            {
                return;
            }

            if(UserSettings.CanGetMsgs(message.Author.Id))
            {
                YukiMessage yukiMessage = new YukiMessage()
                {
                    Id = message.Id,

                    SendDate = new System.DateTime(message.Timestamp.UtcTicks),

                    AuthorId = message.Author.Id,
                    ChannelId = message.Channel.Id,
                    
                    Content = (message as SocketUserMessage)
                            .Resolve(TagHandling.FullName, TagHandling.NameNoPrefix, TagHandling.Name, TagHandling.Name, TagHandling.FullNameNoPrefix)
                };

                YukiMessage foundMessage = Messages.FirstOrDefault(msg => msg.Id == message.Id || msg.Content.ToLower() == message.Content.ToLower());
                int index = Messages.IndexOf(foundMessage);

                if(!foundMessage.Equals(default(YukiMessage)))
                {
                    if(foundMessage.Id == message.Id)
                    {
                        Messages[index] = yukiMessage;
                    }
                }
                else
                {
                    Messages.Add(yukiMessage);
                }


                // clear messages if we have more than the max allowed
                int messageCountForUser = Messages.Where(msg => msg.AuthorId == message.Author.Id).Count();
                if (messageCountForUser > MaxMessages)
                {
                    int messagesToRemove = messageCountForUser - MaxMessages;
                    int removedMessages = 0;

                    for(int i = 0; i < Messages.Count; i++)
                    {
                        if(Messages[i].AuthorId == message.Author.Id)
                        {
                            Messages.RemoveAt(i);
                            removedMessages++;
                            --i;
                        }

                        if(removedMessages == messagesToRemove)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static List<YukiMessage> GetMessagesFromUser(ulong userId)
        {
            return Messages.Where(msg => msg.AuthorId == userId).ToList();
        }

        public static void Delete(SocketMessage message)
        {
            Delete(message.Id);
        }
        
        public static void Delete(ulong id)
        {
            YukiMessage _message = Messages.FirstOrDefault(msg => msg.Id == id);
            
            if(!_message.Equals(default(YukiMessage)))
            {
                Messages.Remove(_message);
            }
        }
        
        public static void Delete(YukiMessage msg)
        {
            if(Messages.Contains(msg))
            {
                Messages.Remove(msg);
            }
        }

        public static void DeleteWithChannelId(ulong id)
        {
            YukiMessage[] msgs = Messages.Where(msg => msg.ChannelId == id).ToArray();

            for(int i = 0; i < msgs.Length; i++)
            {
                Messages.Remove(msgs[i]);
            }
        }

        public static void DeleteFromUser(ulong id)
        {
            YukiMessage[] msgs = Messages.Where(msg => msg.AuthorId == id).ToArray();

            for (int i = 0; i < msgs.Length; i++)
            {
                Messages.Remove(msgs[i]);
            }
        }

        public static void LoadFromFile()
        {
            if(File.Exists(FileDirectories.Messages))
            {
                Messages = JsonConvert.DeserializeObject<List<YukiMessage>>(File.ReadAllText(FileDirectories.Messages));
            }
        }

        public static void SaveToFile()
        {
            File.WriteAllText(FileDirectories.Messages, JsonConvert.SerializeObject(Messages, Formatting.Indented));
        }
    }
}
