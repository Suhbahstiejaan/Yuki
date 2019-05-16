using LiteDB;
using System.Linq;
using Yuki.Data.Objects;

namespace Yuki.Services
{
    public class MessageDB
    {
        public const int MAX_MSGS = 50;
        public const int PATREON_ADDITIONAL_MSGS = 50;

        public void Add(YukiUser usr)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.MessageDB))
            {
                LiteCollection<YukiUser> col = db.GetCollection<YukiUser>();

                YukiUser user = col.FindAll().FirstOrDefault(_usr => _usr.Id == usr.Id);

                if (!user.Equals(default(YukiUser)))
                {
                    if (!user.Messages.FirstOrDefault(_msg => _msg.Id == usr.Messages[0].Id).Equals(default(YukiMessage)))
                    {
                        Edit(usr.Messages[0], usr.Id);
                    }
                    else
                    {
                        if(user.Messages.Count > MAX_MSGS)
                        {
                            int msgsToRemove = user.Messages.Count - MAX_MSGS;

                            for(int i = 0; i < msgsToRemove; i++)
                            {
                                Delete(user.Messages[i]);
                            }
                        }

                        user.Messages.Add(usr.Messages[0]);
                        col.Update(user);
                    }
                }
                else
                {
                    col.Insert(usr);
                }
            }
        }
        
        public void Delete(YukiMessage msg)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.MessageDB))
            {
                LiteCollection<YukiUser> col = db.GetCollection<YukiUser>();

                YukiUser user = col.FindAll().Where(usr => !usr.Messages.FirstOrDefault(_msg => _msg.Id == msg.Id).Equals(default(YukiMessage))).FirstOrDefault();

                YukiMessage uMsg = user.Messages.FirstOrDefault(_msg => _msg.Id == msg.Id);

                if (!uMsg.Equals(default(YukiMessage)))
                {
                    user.Messages.Remove(uMsg);
                    
                    col.Update(user);
                }
            }
        }

        public void Delete(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.MessageDB))
            {
                LiteCollection<YukiUser> col = db.GetCollection<YukiUser>();

                YukiUser user = col.FindAll().Where(_msg => _msg.Id == userId).FirstOrDefault();
                
                if (!user.Equals(default(YukiUser)))
                {
                    user.Messages.Clear();
                    col.Update(user);
                }
            }
        }

        public void Edit(YukiMessage msg, ulong author)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.MessageDB))
            {
                LiteCollection<YukiUser> col = db.GetCollection<YukiUser>();

                YukiUser user = col.FindAll().Where(usr => usr.Id == author).FirstOrDefault();

                YukiMessage uMsg;

                if (user.Messages == null || user.Messages.Count < 1)
                {
                    uMsg = default(YukiMessage);
                }
                else
                {
                    uMsg = user.Messages.FirstOrDefault(_msg => _msg.Id == msg.Id);
                }

                if (!uMsg.Equals(default(YukiMessage)))
                {
                    int indexOfX = user.Messages.IndexOf(uMsg);

                    uMsg.Content = msg.Content;

                    user.Messages[indexOfX] = uMsg;

                    col.Update(user);
                }
            }
        }

        public YukiUser GetUser(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.MessageDB))
            {
                LiteCollection<YukiUser> col = db.GetCollection<YukiUser>();

                return col.FindAll().FirstOrDefault(usr => usr.Id == userId);
            }
        }
    }
}