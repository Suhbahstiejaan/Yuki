namespace Yuki.Bot.Entity
{
    public class PollOption
    {
        public int number;
        public string name;
        public int votes;

        public PollOption(string optionName)
        {
            name = optionName;
            votes = 0;
        }

        public PollOption()
        {
            name = "default";
            votes = 0;
        }

        public void Vote()
            => votes++;
    }
}
