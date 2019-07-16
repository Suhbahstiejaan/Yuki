using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using Yuki.Commands;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Data.Objects.Settings
{
    public class SettingAddWarningAction : ISettingPage
    {
        public string Name { get; set; } = "warnings_add";

        public async void Display(YukiModule Module, YukiCommandContext Context)
        {
            await Module.ReplyAsync(new EmbedBuilder()
                    .WithAuthor(Module.Language.GetString(Name))
                    .WithDescription("setting_warning_add_desc"));
        }

        public async Task Run(YukiModule Module, YukiCommandContext Context)
        {
            InteractivityResult<SocketMessage> result = await Module.Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

            if (result.IsSuccess)
            {
                /* warning, action, roleName */
                string[] vals = result.Value.Content.Split(' ');

                if (int.TryParse(vals[0], out int warningNum))
                {
                    WarningAction type = default;

                    switch(vals[1].ToLower())
                    {
                        case "giverole":
                            type = WarningAction.GiveRole;
                            break;
                        case "ban":
                            type = WarningAction.Ban;
                            break;
                        case "kick":
                            type = WarningAction.Kick;
                            break;
                    }
                    
                    if(type != default)
                    {
                        GuildWarningAction action = new GuildWarningAction()
                        {
                            Warning = warningNum,
                            WarningAction = type,
                        };

                        if (type == WarningAction.GiveRole && vals.Length < 2)
                        {
                            return;
                        }
                        else
                        {
                            action.RoleId = MentionUtils.ParseRole(vals[2]);
                        }

                        GuildSettings.AddWarningAction(action, Context.Guild.Id);
                        await Module.ReplyAsync(Module.Language.GetString("warning_action_added") + ": " + result.Value.Content);
                    }
                }
            }
        }
    }
}
