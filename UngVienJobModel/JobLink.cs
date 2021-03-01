using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using XMedia;

namespace UngVienJobModel
{
    public class JobLink : BaseET
    {
        public JobLink(LoaiLink _loai)
        {
            link = "";
            tieu_de = "";
            json = "";
            loai = _loai;
            ngay_sua = ngay_tao = XUtil.TimeInEpoch();
            if(_loai== LoaiLink.JOB_LINK)
                json = null;
        }
        public string ten_job { get; set; }
        [Keyword]
        public string link { get; set; }
        public TrangThai trang_thai { get; set; }
        public TrangThaiXuLy trang_thai_xu_ly { get; set; }
        [Text]
        public string thong_tin_xu_ly { get; set; }
        public long ngay_xu_ly { get; set; }
        public long tong_so_cv { get; set; }
        public string tieu_de { get; set; }
        public LoaiLink loai { get; set; }

        [Object]
        public dynamic json { get; set; }
    }
}
