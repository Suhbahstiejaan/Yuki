using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class UserSettings
    {
        private const string collection = "user_settings";

        private static YukiUser DefaultUser(ulong userId)
        {
            return new YukiUser()
            {
                Id = userId,
                CanGetMsgs = false,
                IsPatron = false,
                langCode = Config.GetConfig().default_lang,
                Reminders = new List<YukiReminder>()
            };
        }

        public static void AddOrUpdate(YukiUser user)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
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
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
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
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Id == userId))
                {
                    return users.FindAll().FirstOrDefault(usr => usr.Id == userId).IsPatron;
                }
            }

            return false;
        }

        public static ulong[] GetPatrons()
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.IsPatron))
                {
                    return users.FindAll().Where(usr => usr.IsPatron).Select(usr => usr.Id).ToArray();
                }
            }

            return null;
        }

        public static bool CanGetMsgs(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                YukiUser user = users.FindAll().FirstOrDefault(usr => usr.Id == userId);
                
                return !user.Equals(default(YukiUser)) ? user.CanGetMsgs : false;
            }
        }

        public static void AddReminder(YukiReminder reminder)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (!users.FindAll().Any(usr => usr.Id == reminder.AuthorId))
                {
                    AddOrUpdate(DefaultUser(reminder.AuthorId));

                    AddReminder(reminder);
                }


                YukiUser user = users.Find(usr => usr.Id == reminder.AuthorId).FirstOrDefault();
                user.Reminders.Add(reminder);

                users.Update(user);
            }
        }

        public static void RemoveReminder(YukiReminder reminder)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
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
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.FindAll().Any(usr => usr.Reminders != null && usr.Reminders.Count > 0))
                {
                    return users.FindAll().SelectMany(usr => usr.Reminders).Where(reminder => reminder.Time <= dateTime).ToList();
                }
            }

            return new List<YukiReminder>();
        }

        public static void SetCanGetMessages(ulong userId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if(!users.FindAll().Any(usr => usr.Id == userId))
                {
                    AddOrUpdate(DefaultUser(userId));
                }

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();
                user.CanGetMsgs = state;

                users.Update(user);
            }
        }

        public static void AddPatron(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (!users.FindAll().Any(usr => usr.Id == userId))
                {
                    AddOrUpdate(DefaultUser(userId));
                }

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();
                user.IsPatron = true;

                users.Update(user);
            }
        }

        public static void RemovePatron(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (!users.FindAll().Any(usr => usr.Id == userId))
                {
                    AddOrUpdate(DefaultUser(userId));
                }

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();
                user.IsPatron = false;

                users.Update(user);
            }
        }
    }
}
