using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class WarningActionRepository : IWarningActionRepository, IDisposable
    {
        private YukiContext context;

        public WarningActionRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddAction(GuildWarningAction action)
        {
            context.WarningActions.Add(action);
        }

        public void RemoveAction(GuildWarningAction action)
        {
            context.WarningActions.Remove(action);
        }

        public GuildWarningAction GetAction(ulong guildId, int warning)
        {
            GuildWarningAction action = context.WarningActions.FirstOrDefault(x => x.Warning == warning && x.ServerId == guildId);
            return action;
        }

        public IEnumerable<GuildWarningAction> GetActions(ulong guildId)
        {
            IEnumerable<GuildWarningAction> actions = context.WarningActions.Where(x => x.ServerId == guildId);
            return actions;
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
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
