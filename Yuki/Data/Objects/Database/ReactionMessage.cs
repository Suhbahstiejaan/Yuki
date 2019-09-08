using System.Collections.Generic;

namespace Yuki.Data.Objects.Database
{
    public struct ReactionMessage
    {
        public ulong Id { get; set; }

        public List<MessageReaction> Reactions { get; set; }
    }
}
