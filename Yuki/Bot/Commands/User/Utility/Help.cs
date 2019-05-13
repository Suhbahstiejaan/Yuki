using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Bot.Common;
using Yuki.Bot.Misc;
using Yuki.Bot.Misc.Extensions;
using Yuki.Bot.Services.Localization;

namespace Yuki.Bot.Helper
{
    public class Help
    {
        private static EmbedBuilder generatedEmbed;
        
        private static List<Data> help;
        
        public static int GetTotalCommands
        {
            get 
            {
                int total = 0;

                foreach (KeyValuePair<string, List<ModuleInfo>> pair in Localizer.modules)
                    foreach (ModuleInfo module in pair.Value)
                        foreach (CommandInfo command in module.Commands)
                            total++;
                
                return total;
            }
        }
        
        public static async Task GetHelp(IMessageChannel channel, string query, string lang, DiscordSocketClient client)
        {
            help = Localizer.GetStrings(lang).help;
            string getHelp = Localizer.GetLocalizedStringFromData(help, "info");

            if(query != null)
            {
                generatedEmbed = GenerateEmbed(query, lang, getHelp, client);

                if(query != getHelp)
                {
                    if(Search(query, lang))
                        await channel.SendMessageAsync("", false, generatedEmbed.Build());
                    else
                        await channel.SendMessageAsync(Localizer.GetLocalizedStringFromData(help, "no_command"));
                }
                else
                    await channel.SendMessageAsync("", false, generatedEmbed.Build());
            }
            else
                await channel.SendMessageAsync("", false, GenerateCommandList(lang, getHelp));
        }

        private static EmbedBuilder GenerateEmbed(string query, string lang, string term, DiscordSocketClient client)
        {
            EmbedBuilder embed = new EmbedBuilder
            {
                Color = Colors.Pink,
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Yuki " + YukiClient.version + " | y!help " + term
                },
            };

            if (query == term)
            {
                List<Data> data_info = Localizer.GetStrings(Localizer.YukiStrings.default_lang).info;
                
                embed.Title = Localizer.GetLocalizedStringFromData(help, "looking_for_help");
                embed.Description = Localizer.GetLocalizedStringFromData(help, "description") + "\n** **";
                embed.AddField(Localizer.GetLocalizedStringFromData(data_info, "creator"), "Vee#0003", true)
                     .AddField(Localizer.GetLocalizedStringFromData(data_info, "shard"), client.ShardId + (YukiClient.Instance.ConnectedShards.Count > 1 ? "/" + YukiClient.Instance.ConnectedShards.Count : ""), true)
                     .AddField(Localizer.GetLocalizedStringFromData(data_info, "servers"), client.Guilds.Count + (YukiClient.Instance.ConnectedShards.Count > 1 ? " (" + YukiClient.Instance.Client.Guilds.Count + " total)" : ""), true)
                     .AddField(Localizer.GetLocalizedStringFromData(data_info, "unique"), YukiClient.Instance.ConnectedShards[client.ShardId].Members.Count + (YukiClient.Instance.ConnectedShards.Count > 1 ? " (" + YukiClient.Instance.ConnectedShards.SelectMany(shard => shard.Members).Count() + " total)" : ""), true)
                     .AddField(Localizer.GetLocalizedStringFromData(help, "useful_links"),
                               $"[{Localizer.GetLocalizedStringFromData(help, "server")}]({Localizer.GetURLs.server_invite_url})\n\t" +
                               $"[{Localizer.GetLocalizedStringFromData(help, "invitation")}]({Localizer.GetURLs.bot_invite_url})\n\t" +
                               $"[{Localizer.GetLocalizedStringFromData(help, "donate_patreon")}]({Localizer.GetURLs.donation_patreon})\n\t" + 
                               $"[{Localizer.GetLocalizedStringFromData(help, "donate_paypal")}]({Localizer.GetURLs.donation_paypal})\n\t" + 
                               $"[{Localizer.GetLocalizedStringFromData(help, "source")}]({Localizer.GetURLs.source_url})\n** **");
            }

            return embed;
        }
        
        private static bool Search(string query, string lang)
        {
            LocalizedCommand cmd = Localizer.GetCommands(lang, null, query.Replace(' ', '_')).FirstOrDefault();
            List<CommandInfo> foundCommands = new List<CommandInfo>();

            if (cmd == null)
            {
                string[] split = query.Split(new char[] { ' ' }, 2);

                foreach (KeyValuePair<string, List<ModuleInfo>> pair in Localizer.modules)
                    if (pair.Key.ToLower() == split[0].ToLower())
                        foreach (ModuleInfo module in pair.Value)
                            foundCommands.AddRange(module.Commands);
                    else
                        foreach (ModuleInfo module in pair.Value)
                            if (module.Name.ToLower() == split[0].ToLower())
                                foundCommands.AddRange(module.Commands);

                if (foundCommands.Count > 0)
                {
                    string[] commands = new string[foundCommands.Count];
                    generatedEmbed.Title = Localizer.GetLocalizedStringFromData(help, "commands_found") + " " + query;

                    int index = 0;
                    foreach (CommandInfo command in foundCommands)
                    {
                        commands[index] = Localizer.YukiStrings.prefix;

                        if (command.Module.IsSubmodule)
                            commands[index] += command.Module.Name + " ";

                        if (command.Name != "BaseCommand")
                            commands[index] += command.Name;
                        else
                            commands[index] = commands[index].Remove(commands[index].Length - 1); //get rid of the trailing space

                        index++;
                    }

                    generatedEmbed.Description = string.Join(", ", commands);
                }
            }
            else
            {
                generatedEmbed.Title = cmd.Name.Replace("_", " ").CapitalizeFirst();
                generatedEmbed.Description = cmd.Summary;
                generatedEmbed.AddField(Localizer.GetLocalizedStringFromData(help, "usage"), cmd.Usage);

                string[] likeCommands = Localizer.GetCommands(lang, cmd.Name, query).Where(x => x != null && x.Name != null).Select(y => y.Name).ToArray();
                
                if(likeCommands.Length > 0)
                    generatedEmbed.AddField(Localizer.GetLocalizedStringFromData(help, "suggestions") + ":", Localizer.GetLocalizedStringFromData(help, "try") + " " + string.Join(", ", likeCommands.ToArray()));
            }
            
            return (cmd != null || foundCommands.Count > 0);
        }

        private static Embed GenerateCommandList(string lang, string term)
        {
            EmbedBuilder embed = new EmbedBuilder().WithAuthor(new EmbedAuthorBuilder() { Name = Localizer.GetLocalizedStringFromData(help, "list_of_commands") }).WithColor(Colors.Pink);

            foreach (KeyValuePair<string, List<ModuleInfo>> pair in Localizer.modules)
            {
                ModuleInfo[] module = pair.Value.ToArray();

                string cmds = "";
                
                for(int i = 0; i < module.Length; i++)
                {
                    foreach(CommandInfo command in module[i].Commands.OrderBy(x => x.Name))
                    {
                        cmds += Localizer.YukiStrings.prefix;

                        if(module[i].IsSubmodule)
                            cmds += module[i].Name + " ";

                        if(command.Name != "BaseCommand")
                            cmds += command.Name;
                        else
                            cmds = cmds.Remove(cmds.Length - 1);

                        cmds += ", ";
                    }
                }

                if(!string.IsNullOrEmpty(cmds))
                    embed.AddField(pair.Key.CapitalizeFirst(), cmds.Remove(cmds.Length - 2));
            }

            embed.Footer = new EmbedFooterBuilder() { Text = "Yuki " + YukiClient.version + " | y!help " + term };

            return embed.Build();
        }
    }
}