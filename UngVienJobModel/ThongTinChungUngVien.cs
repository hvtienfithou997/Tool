using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace UngVienJobModel
{
    public class ThongTinChungUngVien
    {
        [Keyword]
        public string domain { get; set; }
        public string full_text { get; set; }
    }
}
