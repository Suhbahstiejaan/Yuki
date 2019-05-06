using System.Collections.Generic;
using System.Timers;

namespace Yuki.Discord
{
    public struct YukiShard
    {
        public int Id { get; set; }
        public List<ulong> Members { get; set; }
        public Timer Playing { get; set; }
    }
}
