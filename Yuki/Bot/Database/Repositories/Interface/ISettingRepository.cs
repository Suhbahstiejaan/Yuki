using System;
using System.Collections.Generic;

namespace Yuki.Bot.Misc.Database.Repositories.Interface
{
    interface ISettingRepository : IDisposable
    {
        Setting GetSetting(string settingName, ulong guildId);
        IEnumerable<Setting> GetSettings(ulong guildId);
        void AddSetting(Setting setting);
        void RemoveSetting(Setting setting);
    }
}
