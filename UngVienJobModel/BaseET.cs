using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace UngVienJobModel
{
    public class BaseET
    {
        [Ignore]
        public string id { get; set; }
        [Keyword]
        public string app_id { get; set; }
        public long ngay_tao { get; set; }
        public long ngay_sua { get; set; }
        [Keyword]
        public string nguoi_tao { get; set; }
        [Keyword]
        public string nguoi_sua { get; set; }
    }
}
