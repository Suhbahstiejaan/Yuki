using Yuki.Commands;

namespace Yuki.Data.Objects.Settings
{
    public interface ISettingPageTogglable : ISettingPage
    {
        string GetState(YukiCommandContext Context);
    }
}
