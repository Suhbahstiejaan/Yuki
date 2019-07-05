using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Yuki.API;

namespace Yuki.Data.Objects
{
    public struct YukiImage
    {
        public ImageType type { get; set; }
        public bool isExplicit { get; set; }


        public string url { get; set; }
        public string source { get; set; }
        public string page { get; set; }
        public string[] tags { get; set; }
    }
}
