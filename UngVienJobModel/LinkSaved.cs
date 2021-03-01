using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using XMedia;
namespace UngVienJobModel
{
    public class LinkSaved : BaseET
    {
        public string link_post { get; set; }
        public string domain { get; set; }
        public string ten_job { get; set; }
        public int thuoc_tinh { get; set; }

        public LinkSaved()
        {
            ngay_tao = XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XUtil.TimeInEpoch(DateTime.Now);
        }

    }
}