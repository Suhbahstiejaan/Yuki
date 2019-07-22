using Discord;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("commandlist", "ccommands")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CustomCommandsList()
        {
            if(!(Context.Channel is IDMChannel))
            {
                List<GuildCommand> commands = GuildSettings.GetGuild(Context.Guild.Id).Commands;

                if(commands.Count > 0)
                {
                    await ReplyAsync(Context.CreateEmbedBuilder(Language.GetString("commands_title"))
                            .WithDescription(string.Join("\n", commands.Select(c => $"{commands.IndexOf(c) + 1}. {c.Name}"))));
                }
                else
                {
                    await ReplyAsync(Language.GetString("no_custom_commands"));
                }
            }
        }
    }
}
