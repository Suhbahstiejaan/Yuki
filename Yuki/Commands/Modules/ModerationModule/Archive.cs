using Discord;
using Qmmands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("archive")]
        public class Archive : YukiModule
        {
            [Command]
            public async Task ArchiveChannelAsync()
            {
                await ReplyAsync(Language.GetString("archiving"));
                IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(int.MaxValue, CacheMode.AllowDownload, RequestOptions.Default).FlattenAsync()).Reverse();

                string filename = Path.Combine(FileDirectories.TempArchiveRoot, $"{Context.Channel.Name} ({Context.Channel.Id}).txt");

                using (StreamWriter file = new StreamWriter(filename))
                {
                    foreach (IMessage message in messages)
                    {
                        string attachments = string.Join("\n", message.Attachments.Select(att => att.Url));

                        file.WriteLine($"[{message.Timestamp}]{ (message.EditedTimestamp.HasValue ? $" [{Language.GetString("edited")} {message.EditedTimestamp}]" : "")} <{message.Author}> {message.Content}{(!string.IsNullOrEmpty(attachments) ? $"\n{Language.GetString("message_attachments")}: {attachments})" : "")}");
                    }
                }
                await SendFileAsync(filename, Language.GetString("archiving_done"));
                File.Delete(filename);
            }
            
            [Command("pins")]
            public async Task ArchivePinsInChannelAsync()
            {
                await ReplyAsync(Language.GetString("archiving"));
                IEnumerable<IMessage> messages = (await Context.Channel.GetPinnedMessagesAsync()).Reverse();

                string filename = Path.Combine(FileDirectories.TempArchiveRoot, $"{Context.Channel.Name}_pins ({Context.Channel.Id}).txt");

                using (StreamWriter file = new StreamWriter(filename))
                {
                    foreach (IMessage message in messages)
                    {
                        string attachments = string.Join("\n", message.Attachments.Select(att => att.Url));

                        file.WriteLine($"[{message.Timestamp}]{ (message.EditedTimestamp.HasValue ? $" [{Language.GetString("edited")} {message.EditedTimestamp}]" : "")} <{message.Author}> {message.Content}{(!string.IsNullOrEmpty(attachments) ? $"\n{Language.GetString("message_attachments")}: {attachments})" : "")}");
                    }
                }
                await SendFileAsync(filename, Language.GetString("archiving_done"));
                File.Delete(filename);
            }
        }
    }
}
