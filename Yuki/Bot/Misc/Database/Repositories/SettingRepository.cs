using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class SettingRepository : ISettingRepository, IDisposable
    {
        private YukiContext context;

        public SettingRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddSetting(Setting setting)
        {
            context.Settings.Add(setting);
        }

        public void RemoveSetting(Setting setting)
        {
            context.Settings.Remove(setting);
        }

        public Setting GetSetting(string settingName, ulong guildId)
        {
            Setting setting = context.Settings.FirstOrDefault(x => x.ServerId == guildId && x.Name == settingName);
            return setting;
        }

        public IEnumerable<Setting> GetSettings(ulong guildId)
        {
            IEnumerable<Setting> settings = context.Settings.Where(x => x.ServerId == guildId);
            return settings;
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
