using System;

namespace Yuki.Data.Objects.Database
{
    public struct YukiReminder
    {
        //public int Id { get; set; }

        public ulong AuthorId { get; set; }

        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
