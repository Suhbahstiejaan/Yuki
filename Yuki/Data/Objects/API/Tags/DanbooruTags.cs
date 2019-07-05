using System;
using System.Collections.Generic;
using System.Text;

namespace Yuki.Data.Objects.API.Tags
{
    public class DanbooruTags
    {
        public int id { get; set; }
        public string antecedent_name { get; set; }
        public string reason { get; set; }
        public int creator_id { get; set; }
        public string consequent_name { get; set; }
        public string status { get; set; }
        public object forum_topic_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int post_count { get; set; }
        public object approver_id { get; set; }
        public object forum_post_id { get; set; }
    }
}
