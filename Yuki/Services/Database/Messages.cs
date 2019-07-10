using LiteDB;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data.Objects;

namespace Yuki.Services.Database
{
    public static class Messages
    {
        public static void InsertOrUpdate(YukiMessage message)
        {
            if(UserSettings.CanGetMsgs(message.AuthorId))
            {
                using (LiteDatabase db = new LiteDatabase(FileDirectories.CacheDB))
                {
                    LiteCollection<YukiMessage> messages = db.GetCollection<YukiMessage>("messages");

                    YukiMessage userMessage = messages.Find(x => x.Id == message.Id).FirstOrDefault();

                    if (!userMessage.Equals(default))
                    {
                        messages.Update(message);
                    }
                    else
                    {
                        messages.Insert(message);
                    }
                }
            }
        }

        public static bool Remove(YukiMessage message)
        {
            if (UserSettings.CanGetMsgs(message.AuthorId))
            {
                using (LiteDatabase db = new LiteDatabase(FileDirectories.CacheDB))
                {
                    return db.GetCollection<YukiMessage>("messages").Delete(message.Id);
                }
            }
            return false;
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
