using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.Common
{
    public class YukiRandom : Random
    {
        public Color RandomColor {
            get {
                Color[] colors = Colors.Get();
                return colors[Next(1, colors.Length) - 1];
            }
        }

        public string MessageEmpty(string lang) {
            string[] messageEmpty = Localizer.GetStrings(lang).message_empty.ToArray();
            return messageEmpty[Next(messageEmpty.Length)];
        }

        public string EightBall(string lang) {
            string[] responses = Localizer.GetStrings(lang).eight_ball.ToArray();
            return responses[Next(responses.Length)];
        }

        public string SlowmodeDisabled(string lang) {
            string[] slowmodeDisabled = Localizer.GetStrings(lang).slowmode_disabled.ToArray();
            return slowmodeDisabled[Next(slowmodeDisabled.Length)];
        }

        public string AlreadyInVC(string lang) {
            string[] inVC = Localizer.GetStrings(lang).already_in_vc.ToArray();
            return inVC[Next(inVC.Length)];
        }

        public string RandomGame(DiscordSocketClient client) {
            string[] games = Localizer.YukiStrings.info.Select(x => x.Replace("%version%", YukiClient.version)
                                                                     .Replace("%prefix%", Localizer.YukiStrings.prefix)
                                                                     .Replace("%totalusers%", YukiClient.Instance.TotalUsers.ToString())
                                                                     .Replace("%shardusers%", YukiClient.Instance.ConnectedShards.First(shard => shard.ShardId == client.ShardId).Members.Count.ToString())
                                                                     .Replace("%shard%", client.ShardId.ToString())
                                                                     .Replace("%totalservers%", YukiClient.Instance.Client.Guilds.Count.ToString())
                                                                     .Replace("%shardservers%", client.Guilds.Count.ToString())).ToArray();

            return games[Next(games.Length)];
        }

        public string NotInServer(string lang) {
                string[] notServer = Localizer.GetStrings(lang).not_server.ToArray();
                return notServer[Next(notServer.Length)];
        }

        public string GoodNight(string lang) {
            string[] night = Localizer.GetURLs.goodnight.ToArray();
            return night[Next(night.Length)];
        }

        public T RandomEnum<T>()
        {
            Array v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(Next(v.Length));
        }
    }
}
