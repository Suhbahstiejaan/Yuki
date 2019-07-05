using System;
using System.Collections.Generic;
using System.Text;

namespace Yuki.Data.Objects.API.ImageObject
{
    public class Rule34
    {
        public string directory { get; set; }
        public string hash { get; set; }
        public int? height { get; set; }
        public int? id { get; set; }
        public string image { get; set; }
        public int? change { get; set; }
        public string owner { get; set; }
        public int? parent_id { get; set; }
        public string rating { get; set; }
        public bool sample { get; set; }
        public int? sample_height { get; set; }
        public int? sample_width { get; set; }
        public int? score { get; set; }
        public string tags { get; set; }
        public int? width { get; set; }
    }
}
