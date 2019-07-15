using System.Threading.Tasks;
using Yuki.Commands;

namespace Yuki.Data.Objects.Settings
{
    public interface ISettingPage
    {
        string Name { get; set; }
        
        Task Run(YukiModule Module, YukiCommandContext Context);
        void Display(YukiModule Module, YukiCommandContext Context);
    }
}
