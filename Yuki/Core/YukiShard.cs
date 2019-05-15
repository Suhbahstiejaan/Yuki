using System.Collections.Generic;
using System.Timers;

namespace Yuki.Core
{
    public struct YukiShard
    {
        public int Id { get; set; }
        public List<ulong> Members { get; set; }
        public Timer Playing { get; set; }
    }
}
