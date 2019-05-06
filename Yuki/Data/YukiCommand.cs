using System;
using System.Collections.Generic;
using System.Text;

namespace Yuki.Data
{
    public class YukiCommand
    {
        public string Name;
        public string Description;
        public string Usage;

        public List<string> Aliases;
    }
}
