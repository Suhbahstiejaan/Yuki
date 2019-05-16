using Qmmands;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Commands
{
    public abstract class YukiModule : ModuleBase<YukiCommandContext>
    {
        public Language Language => LocalizationService.GetLanguage(Context);
    }
}
