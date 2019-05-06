using Discord;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yuki.Core.Extensions;
using Yuki.Data.MessageDatabase;

namespace Yuki.Core
{
    public class Scramblr_
    {
        const int MaxLikeMessages = 500;

        private ulong id1;
        private ulong id2;

        private IGuild guild;

        public Scramblr(ulong user, IGuild guild) :
            this(user, user, guild) { }

        public Scramblr(ulong user1, ulong user2, IGuild guild)
        {
            id1 = user1;
            id2 = user2;

            this.guild = guild;
        }

        /// <summary>
        /// Get a scrambled message
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        /// <param name="loopCount"></param>
        /// <returns></returns>
        public string GetMessage(int loopCount = 0)
        {
            string scrambled = "";
            ScramblrData[] data;

            MDatabase mDatabase = YukiBot.Services.GetRequiredService<MDatabase>();

            Message[] user1 = mDatabase.GetUser(id1).Messages.ToArray();
            Message[] user2 = mDatabase.GetUser(id2).Messages.ToArray();

            YukiRandom yRandom = new YukiRandom();

            if (user1.Length <= 3)
                return "Sorry! I couldn't find enough/any messages from you :(";

            data = GetLikeMessages(user1, user2);

            if (data.Length < 1)
                return "I couldn't find any like words :/";

            if (user2 != null)
                return ScrambledMessage(data[yRandom.Next(data.Length)]);

            scrambled = ScrambledMessage(data[yRandom.Next(data.Length)]);

            //we want to make sure that if the user didnt mention anyone, the message the bot generates isnt the same as a message the user has sent.   
            for (int i = 0; i < user1.Length; i++)
            {
                if (user1[i].Content == scrambled)
                {
                    if (loopCount < 25)
                        return GetMessage(loopCount++);
                    else
                        break;
                }
            }

            return scrambled;
        }

        /// <summary>
        /// Scramble the message!
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guild"></param>
        /// <param name="user2"></param>
        /// <returns></returns>
        private string ScrambledMessage(ScramblrData data)
        {
            YukiRandom yRandom = new YukiRandom();

            //split the messages each into 2 separete strings where the like word is
            string[] sanitized_dat1 = Regex.Split(guild.SanitizeMentions(data.user1_msg.Content), $@"\b{Regex.Escape(data.likeWord)}\b");
            string[] sanitized_dat2 = Regex.Split(guild.SanitizeMentions(data.user2_msg.Content), $@"\b{Regex.Escape(data.likeWord)}\b");

            string scrambled(string[] dat1, string[] dat2)
            {
                string str = dat1[0];

                if (MentionUtils.ParseUser(data.likeWord) != 0)
                    str += guild.SanitizeMentions(data.likeWord);
                else
                    str += data.likeWord;

                if (dat2.Length > 1)
                    str += dat2[1];
                else
                    str += dat2[0];

                return str;
            }

            if (yRandom.Next(100) <= 49)
                return scrambled(sanitized_dat1, sanitized_dat2);
            else
                return scrambled(sanitized_dat2, sanitized_dat1);
        }

        /// <summary>
        /// Get messages that have at least one word in common
        /// </summary>
        /// <param name="dat1"></param>
        /// <param name="dat2"></param>
        /// <returns></returns>
        private ScramblrData[] GetLikeMessages(Message[] dat1, Message[] dat2)
        {
            List<ScramblrData> data = new List<ScramblrData>();

            for (int i_u1 = 0; i_u1 < dat1.Length; i_u1++)
                for (int i_u2 = 0; i_u2 < dat2.Length; i_u2++)
                    VerifyData(ref data, dat1[i_u1], dat2[i_u2]);

            return data.ToArray();
        }

        /// <summary>
        /// Adds like words to our list
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg1"></param>
        /// <param name="msg2"></param>
        private void VerifyData(ref List<ScramblrData> data, Message msg1, Message msg2)
        {
            if (msg1.Id != msg2.Id)
            {
                //Split the message at every space, selecting every substring that isn't a url
                string[] dat1_split = msg1.Content.Split(' ').ToList().Where(_str => !_str.IsUrl()).ToArray();
                string[] dat2_split = msg2.Content.Split(' ').ToList().Where(_str => !_str.IsUrl()).ToArray();

                for (int i_u1s = 0; i_u1s < dat1_split.Length; i_u1s++)
                {
                    for (int i_u2s = 0; i_u2s < dat2_split.Length; i_u2s++)
                    {
                        if (dat1_split[i_u1s].Equals(dat2_split[i_u2s]))
                        {
                            ScramblrData scramblrData = null;

                            //verify the like word isnt at the beginning of the string
                            if (msg1.Content.IndexOf(dat1_split[i_u1s]) != 0 && msg2.Content.IndexOf(dat1_split[i_u1s]) != 0)
                                scramblrData = new ScramblrData(dat1_split[i_u1s], msg1, msg2);

                            if (scramblrData != null && !data.Contains(scramblrData) && data.Count < MaxLikeMessages)
                                data.Add(scramblrData);
                        }
                    }
                }
            }
        }
    }

    public class ScramblrData
    {
        public Message user1_msg;
        public Message user2_msg;
        public string likeWord;

        public ScramblrData(string word, Message msg1, Message msg2)
        {
            likeWord = word;
            user1_msg = msg1;
            user2_msg = msg2;
        }
    }
}
