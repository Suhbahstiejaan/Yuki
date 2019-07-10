using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yuki.Data.Objects
{
    public struct YukiReminder
    {
        //public int Id { get; set; }

        public ulong AuthorId { get; set; }

        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
