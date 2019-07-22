using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Services;

namespace Yuki.Commands.Modules.ModerationUtilityModule
{
    public partial class ModerationUtilityModule
    {
        [Command("config", "settings")]
        [RequireAdministrator]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ConfigureGuildSettingsAsync()
        {
            SettingsConfigurator settings = new SettingsConfigurator(this, Context);

            await Task.Run(settings.Run);
        }
    }
}
