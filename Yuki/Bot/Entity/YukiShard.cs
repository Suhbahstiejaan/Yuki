using System.Collections.Generic;

namespace Yuki.Bot.Entity
{
    public struct YukiShard
    {
        public int ShardId;
        
        public List<ulong> Members;

        public YukiShard(int shardId, List<ulong> members)
        {
            ShardId = shardId;
            Members = members;
        }

        public void AddMember(ulong id)
        {
            if (!Members.Contains(id))
                Members.Add(id);
        }

        public void RemoveMember(ulong id)
        {
            if (Members.Contains(id))
                Members.Remove(id);
        }
    }
}
