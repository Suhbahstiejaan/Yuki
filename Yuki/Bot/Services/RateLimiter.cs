using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Yuki.Bot.Services
{
    /* Use: To (hopefully) discourage
       people from spamming commands */
    public class RateLimiter
    {
        public static List<LimitedUser> limitedUsers = new List<LimitedUser>();

        private static int maxMsgs = 4;

        public static bool Limited(IGuildUser guildUser, ITextChannel channel)
        {
            int userIndex = 0;

            LimitedUser user = limitedUsers.FirstOrDefault(x => x.id == guildUser.Id);
            
            if (user == null)
            {
                user = CreateNewUser(guildUser.Id);
                userIndex = limitedUsers.Count - 1;
            }
            
            /* If the user has sent the maximum amount of messages */
            if (user.msgCount >= maxMsgs)
            {
                if (user.seconds == 0)
                    user.seconds = 2;

                user.timer.Interval = user.seconds * 1000;

                user.timer.Start();

                SendLimitMessage(guildUser, channel, user.seconds);

                UpdateUser(user, userIndex);

                return true;
            }
            else
            {
                /* if the user's used a command in the last 2 seconds, increase message count by 1 */
                if (Math.Round(DateTime.Now.Subtract(user.LastMessageSent).TotalSeconds) <= 2)
                    user.msgCount++;
                else if (user.msgCount > 0) /* otherwise, decrease the message count to avoid the user being limited unnecessarily */
                    user.msgCount--;

                UpdateUser(user, userIndex);
            }
            return false;
        }

        private static LimitedUser CreateNewUser(ulong id)
        {
            LimitedUser user = new LimitedUser()
            {
                id = id,
                seconds = 0,
                msgCount = 0,
                LastMessageSent = DateTime.Now,
                timer = new Timer()
            };
            user.timer.Elapsed += new ElapsedEventHandler((EventHandler)delegate (object sender, EventArgs e)
            {
                user.msgCount--;
                user.timer.Stop();
            });
            limitedUsers.Add(user);

            return user;
        }

        private static void SendLimitMessage(IGuildUser guildUser, ITextChannel channel, int sec)
        {
            
            IUserMessage msg = channel.SendMessageAsync(guildUser.Username + ", slow down! Cooldown: **" + sec + "** seconds").Result;

            /* delete msg after 4 seconds */
            Timer t = new Timer() { Interval = 4000 };

            t.Elapsed += new ElapsedEventHandler((EventHandler)delegate (object sender, EventArgs e)
            {
                msg.DeleteAsync();
                t.Stop();
                t.Dispose();
            });

            t.Start();
        }

        private static void UpdateUser(LimitedUser user, int index)
        {
            user.LastMessageSent = DateTime.Now;
            limitedUsers[index] = user;
        }
    }

    public class LimitedUser
    {
        public ulong id;
        public int seconds;
        public int msgCount;
        public Timer timer;
        public DateTime LastMessageSent;
    }
}
