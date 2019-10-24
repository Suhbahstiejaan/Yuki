using LiteDB;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class Messages
    {
        public static readonly int MaxMessages = 100;

        public static void InsertOrUpdate(YukiMessage message)
        {
            if(UserSettings.CanGetMsgs(message.AuthorId))
            {
                using (LiteDatabase db = new LiteDatabase(FileDirectories.CacheDB))
                {
                    LiteCollection<YukiMessage> messages = db.GetCollection<YukiMessage>("messages");

                    if (messages.FindAll().Any(x => x.Id == message.Id))
                    {
                        messages.Update(message);
                    }
                    else
                    {
                        if(messages != null && messages.FindAll().Count() > 0)
                        {
                            List<YukiMessage> msgs = messages.FindAll().Where(msg => msg.AuthorId == message.AuthorId).OrderBy(msg => msg.SendDate).ToList();

                            if (msgs.Count > MaxMessages)
                            {
                                foreach (YukiMessage m in msgs.Take(msgs.Count - MaxMessages))
                                {
                                    Remove(m.Id);
                                }
                            }
                        }
                        
                        messages.Insert(message);
                    }
                }
            }
        }

        public static bool Remove(ulong messageId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.CacheDB))
            {
                return db.GetCollection<YukiMessage>("messages").Delete(messageId);
            }
        }

        public static List<YukiMessage> GetFrom(ulong userId)
        {
            if (UserSettings.CanGetMsgs(userId))
            {
                using (LiteDatabase db = new LiteDatabase(FileDirectories.CacheDB))
                {
                    return db.GetCollection<YukiMessage>("messages").Find(msg => msg.AuthorId == userId).ToList();
                }
            }

            return default;
        }
    }
}
