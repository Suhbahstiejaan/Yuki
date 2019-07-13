using Discord;
using Discord.WebSocket;
using InteractivityAddon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Extensions;

namespace Yuki.Data.Objects.Settings
{
    public interface ISettingPage
    {
        string Name { get; set; }
        
        Task Run(YukiModule Module, YukiCommandContext Context);
        Task<IUserMessage> Display(YukiModule Module, YukiCommandContext Context, IUserMessage message);
    }
}
