using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Yuki.Bot.Misc;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.Services
{
    public class Responses
    {
        static YukiRandom random = new YukiRandom();

        public static async Task Check(SocketMessage message)
        {
            string greetName = string.Empty;
            if (message.Content.Split(' ').Length > 1)
                greetName = message.Content.Split(' ')[1].ToLower().Replace("'", "");

            string[] greetings = Localizer.GetStrings(Localizer.YukiStrings.default_lang).greeting.ToArray();

            for (int i = 0; i < greetings.Length; i++)
            {
                if (message.Content.StartsWith(greetings[i].Replace("'", "") + " yuki", StringComparison.OrdinalIgnoreCase) ||
                    message.Content.StartsWith(greetings[i].Replace("'", "") + ", yuki", StringComparison.OrdinalIgnoreCase))
                {
                    if (message.Author.Id != YukiClient.Instance.DiscordClient.CurrentUser.Id || !message.Author.IsBot)
                    {
                        string endmark = "!";
                        string greet = greetings[random.Next(greetings.Length)];
                        if (greet.ToLower() == "what's up")
                            endmark = "?!";

                        await message.Channel.SendMessageAsync(greet + ", " + message.Author.Username + endmark);
                    }
                }
            }
        }
    }
}
