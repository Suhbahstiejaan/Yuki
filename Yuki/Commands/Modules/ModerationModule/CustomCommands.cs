using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Group("customcommands", "ccommands")]
        public class CustomCommands : YukiModule
        {
            [Command("add")]
            public async Task AddCustomCommandAsync([Remainder] string commandStr)
            {
                string[] split = commandStr.Split(' ', 2);

                GuildCommand command = new GuildCommand()
                {
                    IsParsable = false,
                    Name = split[0].ToLower(),
                    Response = split[1]
                };

                GuildSettings.AddCommand(command, Context.Guild.Id);

                await ReplyAsync(Language.GetString("command_added").Replace("%command%", command.Name));
            }

            [Command("remove", "rem")]
            public async Task RemoveSelfRoleAsync([Remainder] string command)
            {
                GuildSettings.RemoveCommand(command, Context.Guild.Id);

                await ReplyAsync(Language.GetString("command_removed").Replace("%command%", command));
            }
        }
    }
}
