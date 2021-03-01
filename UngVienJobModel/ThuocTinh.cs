using Nest;
using System;
using System.ComponentModel;

namespace UngVienJobModel
{
    public class ThuocTinh : BaseET
    {
        [Keyword]
        public string id_tt { get; set; }

        [DisplayName("Giá trị")]
        public int gia_tri { get; set; }

        [DisplayName("Tên")]
        public string ten { get; set; }

        [DisplayName("Nhóm")]
        public int nhom { get; set; }


        public ThuocTinh()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
    }
}