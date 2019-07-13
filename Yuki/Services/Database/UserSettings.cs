using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class UserSettings
    {
        public const string path = "data/settings.db";

        private const string collection = "user_settings";

        public static void AddOrUpdate(YukiUser user)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (!users.FindAll().Any(usr => usr.Id == user.Id))
                {
                    users.Insert(user);
                }
                else
                {
                    users.Update(user);
                }
            }
        }

        public static void Remove(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Id == userId))
                {
                    users.Delete(userId);
                }
            }
        }

        public static bool IsPatron(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Id == userId))
                {
                    return users.FindAll().FirstOrDefault(usr => usr.Id == userId).IsPatron;
                }
            }

            return false;
        }

        public static bool CanGetMsgs(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Id == userId))
                {
                    return users.FindAll().FirstOrDefault(usr => usr.Id == userId).CanGetMsgs;
                }
            }

            return false;
        }

        public static void AddReminder(YukiReminder reminder)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (!users.FindAll().Any(usr => usr.Id == reminder.AuthorId))
                {
                    AddOrUpdate(new YukiUser()
                    {
                        Id = reminder.AuthorId,
                        CanGetMsgs = false,
                        IsPatron = false,
                        Reminders = new List<YukiReminder>()
                    });

                    AddReminder(reminder);
                }


                YukiUser user = users.Find(usr => usr.Id == reminder.AuthorId).FirstOrDefault();
                user.Reminders.Add(reminder);

                users.Update(user);
            }
        }

        public static void RemoveReminder(YukiReminder reminder)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Id == reminder.AuthorId))
                {
                    YukiUser user = users.Find(usr => usr.Id == reminder.AuthorId).FirstOrDefault();
                    user.Reminders.Remove(reminder);

                    users.Update(user);
                }
            }
        }

        public static List<YukiReminder> GetReminders(DateTime dateTime)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Reminders.Count > 0))
                {
                    return users.FindAll().SelectMany(usr => usr.Reminders).Where(reminder => reminder.Time <= dateTime).ToList();
                }
            }

            return new List<YukiReminder>();
        }
    }
}
