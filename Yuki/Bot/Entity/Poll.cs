using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Bot.Common;

namespace Yuki.Bot.Entity
{
    public class Polls
    {
        public static void AddPoll(string name, string[] optionsArray, DateTime end, ulong guildId)
        {
            Poll pollToAdd = new Poll(name, optionsArray, end, guildId);
            GetPolls.Add(pollToAdd);
        }

        public static void AddPoll(Poll poll)
            => GetPolls.Add(poll);

        public static List<Poll> GetPolls { get; } = new List<Poll>();

        public static Poll GetPoll(string pollId)
            => JSONManager.LoadPoll(pollId);

        public static PollOption GetPollOption(Poll poll, string option)
        {
            foreach (PollOption pollOption in poll.options)
                if (pollOption.name == option || pollOption.number.ToString() == option)
                    return pollOption;
            
            return null;
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class Poll
    {
        public List<ulong> participants;
        public PollOption[] options;
        public string pollTitle;
        public string pollId;
        private int pollIdLength = 10;
        public int totalVotes;
        public ulong guildId;
        public DateTime deadline;

        private static Random random = new Random();

        //Generate a random alphanumerical string with the specified amount of characters
        public static string PollPassphrase(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string phrase = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return phrase;
        }

        public Poll(string title, string[] optionStrings, DateTime deadline, ulong guildId)
        {
            pollTitle = title;
            pollId = PollPassphrase(pollIdLength);
            options = new PollOption[optionStrings.Length];
            totalVotes = 0;
            participants = new List<ulong>();
            this.guildId = guildId;
            this.deadline = deadline;

            for (int i = 0; i < optionStrings.Length; i++)
            {
                options[i] = new PollOption(optionStrings[i]);
                options[i].number = (i + 1);
            }

        }

        public Poll()
        {

        }


        [JsonIgnore]
        public string displayText {
            get
            {
                //Display the poll message.
                string optionList = "";

                for (int i = 0; i < options.Length; i++)
                    optionList += "**" + (i + 1).ToString() + "**" + ". " + options[i].name + "\n";

                return optionList + "\n";
            }
        }


        [JsonIgnore]
        public string statsText {
            get
            {
                //Display the poll message.
                string optionList = "";
                for (int i = 0; i < options.Length; i++)
                    optionList += "**" + (i + 1).ToString() + "**" + ". " + options[i].name + " - **" + options[i].votes + " vote(s)**" + "\n";

                return "**" + pollTitle + "**" + "\n\n" + optionList + "\n\nRemaining time: " + deadlineText;
            }
        }

        [JsonIgnore]
        private string deadlineText {
            get
            {
                if (deadline > DateTime.UtcNow)
                {
                    TimeSpan timeRemaining = deadline - DateTime.Now;
                    return $"{timeRemaining.Days.ToString()} day(s), {timeRemaining.Hours.ToString()} hour(s) and {timeRemaining.Minutes.ToString()} minute(s) remaining.";
                }

                if (deadline < DateTime.UtcNow)
                    return "None";

                return "";
            }
        }
    }
}
