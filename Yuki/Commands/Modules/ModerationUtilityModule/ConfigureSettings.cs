using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Services;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("config", "settings")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ConfigureGuildSettingsAsync()
        {
            SettingsConfigurator settings = new SettingsConfigurator(this, Context);

            while (settings.Running)
            {
                try
                {

                    Console.WriteLine("h");
                    await Task.Run(settings.Run);
                }
                catch(Exception e)
                {
                    await ReplyAsync(e);
                }
            }
        }
    }
}
