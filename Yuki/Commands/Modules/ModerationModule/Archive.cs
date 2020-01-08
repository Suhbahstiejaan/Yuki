using ByteSizeLib;
using Discord;
using Qmmands;
using System;
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
                try
                {
                    Console.WriteLine("Retrieving messages...");
                    List<IMessage> messages = (await Context.Channel.GetMessagesAsync(int.MaxValue, CacheMode.AllowDownload, RequestOptions.Default).FlattenAsync()).Reverse().ToList();
                    Console.WriteLine($"Done! Fount {messages.Count} messages in {((ITextChannel)Context.Channel).Mention}");

                    List<string> files = new List<string>();

                    string filename = Path.Combine(FileDirectories.TempArchiveRoot, $"{Context.Channel.Name} ({Context.Channel.Id}).txt");

                    ByteSize uploadLimit = default;

                    switch(Context.Guild.PremiumTier)
                    {
                        case PremiumTier.None:
                        case PremiumTier.Tier1:
                            uploadLimit = ByteSize.FromMegaBytes(8);
                            break;
                        case PremiumTier.Tier2:
                            uploadLimit = ByteSize.FromMegaBytes(50);
                            break;
                        case PremiumTier.Tier3:
                            uploadLimit = ByteSize.FromMegaBytes(100);
                            break;
                    }

                    ByteSize maxSize = uploadLimit.Subtract(ByteSize.FromKiloBytes(100));

                    Console.WriteLine("Guild max upload size: " + maxSize);

                    while (messages.Count() > 0)
                    {
                        Console.WriteLine("messages size: " + messages.Count);
                        using (StreamWriter file = new StreamWriter(filename))
                        {
                            foreach (IMessage message in messages.ToArray())
                            {
                                string attachments = string.Join("\n", message.Attachments.Select(att => att.Url));

                                file.WriteLine($"[{message.Timestamp.UtcDateTime}]{ (message.EditedTimestamp.HasValue ? $" [{Language.GetString("edited")} {message.EditedTimestamp.Value.UtcDateTime}]" : "")} <{message.Author}> {message.Content}{(!string.IsNullOrEmpty(attachments) ? $"\n{Language.GetString("message_attachments")}: {attachments})" : "")}");
                                file.Flush();

                                messages.Remove(message);

                                if (file.BaseStream.Length > maxSize.Bytes)
                                {
                                    Console.WriteLine($"{filename} has reached its limit! Size: {ByteSize.FromBytes(file.BaseStream.Length)}/{maxSize}");

                                    files.Add(filename);
                                    filename = Path.Combine(FileDirectories.TempArchiveRoot, $"{Context.Channel.Name} ({Context.Channel.Id}) - {files.Count}.txt");

                                    // break out of loop
                                    break;
                                }
                            }
                        }
                    }

                    files.Add(filename);

                    Console.WriteLine("Archive complete! Total files: " + files.Count);

                    for (int i = 0; i < files.Count; i++)
                    {
                        Console.WriteLine("Sending file " + (i + 1) + "...");

                        if (i == 0)
                        {
                            SendFileAsync(files[i], Language.GetString("archiving_done")).Wait();
                        }
                        else
                        {
                            SendFileAsync(files[i], embed: null).Wait();
                        }

                        File.Delete(files[i]);
                    }

                    /*Console.WriteLine("Sending " + filename + "...");
                    await SendFileAsync(filename, embed: null);
                    File.Delete(filename);*/
                }
                catch (Exception e)
                {
                    await ReplyAsync(e);
                }
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

                        file.WriteLine($"[{message.Timestamp.UtcDateTime}]{ (message.EditedTimestamp.HasValue ? $" [{Language.GetString("edited")} {message.EditedTimestamp.Value.UtcDateTime}]" : "")} <{message.Author}> {message.Content}{(!string.IsNullOrEmpty(attachments) ? $"\n{Language.GetString("message_attachments")}: {attachments})" : "")}");
                    }
                }
                await SendFileAsync(filename, Language.GetString("archiving_done"));
                File.Delete(filename);
            }
        }
    }
}
