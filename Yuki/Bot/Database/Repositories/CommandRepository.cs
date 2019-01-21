using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database.Repositories.Interface;

namespace Yuki.Bot.Misc.Database.Repositories
{
    public class CommandRepository : ICommandRepository, IDisposable
    {
        private YukiContext context;

        public CommandRepository(YukiContext context)
        {
            this.context = context;
        }

        public void AddCommand(Command cmd)
        {
            context.Commands.Add(cmd);
        }

        public void RemoveCommand(Command cmd)
        {
            context.Commands.Remove(cmd);
        }

        public Command GetCommand(string cmdName, ulong guildId)
        {
            Command cmd = context.Commands.FirstOrDefault(x => x.CmdName == cmdName && x.ServerId == guildId);
            return cmd;
        }

        public IEnumerable<Command> GetCommands(ulong guildId)
        {
            IEnumerable<Command> cmds = context.Commands.Where(x => x.ServerId == guildId);
            return cmds;
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
