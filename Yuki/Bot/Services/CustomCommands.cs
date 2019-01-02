using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services.Localization;
using Yuki.Bot.Misc.Database;

namespace Yuki.Bot.Services
{
    public class CustomCommands
    {
        public static async Task Check(SocketMessage message)
        {
            if (message.Channel is IDMChannel)
                return;

            using (UnitOfWork uow = new UnitOfWork())
            {
                if (message.Content != null && message.Embeds.Count == 0)
                {
                    ulong guild = ((IGuildChannel)message.Channel).GuildId;

                    string prefix = Localizer.YukiStrings.prefix;
                    string prefix_string = Localizer.YukiStrings.prefix_string;

                    CustomPrefix customPrefix = uow.CustomPrefixRepository.GetPrefix(guild);

                    if((message.Content.IndexOf(prefix) == 0 || message.Content.IndexOf(prefix_string) == 0 || message.Content.IndexOf(customPrefix.prefix) == 0))
                    {
                        string msg = message.Content;
                        bool commandExists = false;

                        if (message.Content.Length >= prefix.Length && message.Content.Substring(0, prefix.Length) == prefix)
                            msg = message.Content.Replace(prefix, "");
                        else if(message.Content.Length >= prefix_string.Length && message.Content.Split(new char[] { ' ' }, 2)[0] + " " == prefix_string)
                            msg = message.Content.Replace(prefix_string, "");
                        else if(message.Content.Length >= customPrefix.prefix.Length && message.Content.Substring(0, customPrefix.prefix.Length) == customPrefix.prefix)
                            msg = message.Content.Replace(customPrefix.prefix, "");

                        foreach (var command in YukiClient.Instance.CommandService.Commands)
                            if (command.Name == msg)
                                commandExists = true;

                        if (!commandExists)
                        {
                            Command cmd = uow.CommandsRepository.GetCommand(msg, guild);
                            if (cmd != null)
                            {
                                string img = null;
                                string[] split = cmd.CmdResponse.Split(' ');

                                //check if CmdResponse has an image
                                for (int i = 0; i < split.Length; i++)
                                    if (StringHelper.IsImage(split[i]))
                                    {
                                        img = split[i];
                                        break;
                                    }

                                //clear the image url from the string
                                if (img != null)
                                    cmd.CmdResponse.Replace(img, "");

                                cmd.CmdResponse = cmd.CmdResponse.Replace("%user%", message.Author.Username).Replace("%muser%", message.Author.Mention);

                                string response = (cmd.CmdResponse.StartsWith(prefix) ||
                                                   Regex.IsMatch(cmd.CmdResponse, "\\Ay!") ||
                                                   (uow.CustomPrefixRepository.GetPrefix(guild) != null &&
                                                    Regex.IsMatch(cmd.CmdResponse, "\\A" + uow.CustomPrefixRepository.GetPrefix(guild).prefix)))
                                                        ? "Detected a prefix at the beginning of the string!\n\nNice try."
                                                        : cmd.CmdResponse;

                                if (img != null)
                                    await message.Channel.SendMessageAsync("", false, new EmbedBuilder() { ImageUrl = cmd.CmdResponse }.Build());
                                else
                                    await message.Channel.SendMessageAsync(((IGuildChannel)message.Channel).Guild.SanitizeMentions(response, true));
                            }
                        }
                    }
                }
            }
        }
    }
}