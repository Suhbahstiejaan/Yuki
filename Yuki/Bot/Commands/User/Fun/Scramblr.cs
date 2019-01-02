using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Extensions;

namespace Yuki.Bot.Services
{
    public class Scramblr
    {
        private int MaxLikeMessages = 500;

        /// <summary>
        /// Get a scrambled message
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        /// <param name="loopCount"></param>
        /// <returns></returns>
        public string GetMessage(IGuildUser user1, IGuildUser user2 = null, int loopCount = 0)
        {
            string scrambled = "";

            ScramblrData[] data;
            CachedMessage[] user1_data = MessageCache.Messages(user1.Id).ToArray();
            YukiRandom yRandom = new YukiRandom();

            if (user1.IsBot || (user2 != null && user2.IsBot))
                return "I can't scramble bots, silly!";

            if (user1_data.Length <= 3)
                return "Sorry! I couldn't find enough/any messages from you :(";

            if (user2 != null)
                data = GetLikeMessages(user1_data, MessageCache.Messages(user2.Id).ToArray());
            else
                data = GetLikeMessages(user1_data, user1_data);

            if (data.Length < 1)
                return "I couldn't find any like words :/";

            if (user2 != null)
                return ScrambledMessage(data[yRandom.Next(data.Length)], user1.Guild, user2);

            scrambled = ScrambledMessage(data[yRandom.Next(data.Length)], user1.Guild, null);

            //we want to make sure that if the user didnt mention anyone, the message the bot generates isnt the same as a message the user has sent.   
            for (int i = 0; i < user1_data.Length; i++)
            {
                if (user1_data[i].Content == scrambled)
                {
                    if (loopCount < 25)
                        return GetMessage(user1, null, loopCount++);
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
        private string ScrambledMessage(ScramblrData data, IGuild guild, IGuildUser user2)
        {
            YukiRandom yRandom = new YukiRandom();

            //split the messages each into 2 separete strings where the like word is
            string[] sanitized_dat1 = Regex.Split(guild.SanitizeMentions(data.user1_msg.Content), $@"\b{Regex.Escape(data.likeWord)}\b");
            string[] sanitized_dat2 = Regex.Split(guild.SanitizeMentions(data.user2_msg.Content), $@"\b{Regex.Escape(data.likeWord)}\b");

            string scrambled(string[] dat1, string[] dat2)
            {
                string str = dat1[0];

                if (guild.GetUserId(data.likeWord) != 0)
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
        private ScramblrData[] GetLikeMessages(CachedMessage[] dat1, CachedMessage[] dat2)
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
        private void VerifyData(ref List<ScramblrData> data, CachedMessage msg1, CachedMessage msg2)
        {
            if (msg1.MessageId != msg2.MessageId)
            {
                //Split the message at every space, selecting every substring that isn't a url
                string[] dat1_split = msg1.Content.Split(' ').ToList().Where(_str => !StringHelper.IsUrl(_str)).ToArray();
                string[] dat2_split = msg2.Content.Split(' ').ToList().Where(_str => !StringHelper.IsUrl(_str)).ToArray();

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
        public CachedMessage user1_msg;
        public CachedMessage user2_msg;
        public string likeWord;

        public ScramblrData(string word, CachedMessage msg1, CachedMessage msg2)
        {
            likeWord = word;
            user1_msg = msg1;
            user2_msg = msg2;
        }
    }
}