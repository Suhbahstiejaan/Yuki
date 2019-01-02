using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Yuki.Bot.Misc
{
    public class JSONManager
    {
        public static string jsonPath = FileDirectories.AppDataDirectory + "polls\\";

        public static void SavePoll(Poll pollToSave, string pollId)
            => File.WriteAllText(jsonPath + pollId + ".json", JsonConvert.SerializeObject(pollToSave, Formatting.Indented));

        public static void SavePollList(List<Poll> pollList, string pollId)
            => File.WriteAllText(jsonPath + pollId + ".json", JsonConvert.SerializeObject(pollList, Formatting.Indented));
        
        public static Poll LoadPoll(string pollId)
        {
            string file = jsonPath + pollId + ".json";

            if (File.Exists(file))
            {
                Poll poll = new Poll();
                string json = File.ReadAllText(jsonPath + pollId + ".json");
                poll = JsonConvert.DeserializeObject<Poll>(json);
                return poll;
            }
            else
                return new Poll();
        }
    }
}
