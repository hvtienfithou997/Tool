using System;
using System.Collections.Generic;
using Nest;

namespace UngVienJobModel
{
    public class UngVien:BaseET
    {
        [Text]
        public string vi_tri { get; set; }
        public List<string> vi_tri_phan_tich { get; set; }
        public string ho_ten { get; set; }
        public string email { get; set; }
        public string so_dien_thoai { get; set; }
        public string zalo { get; set; }
        public GioiTinh gioi_tinh { get; set; }
        [Text]
        public string dia_chi { get; set; }
        [Text]
        public string kinh_nghiem { get; set; }
        [Text]
        public string ky_nang { get; set; }
        [Text]
        public string hoc_van { get; set; }
        public long ngay_sinh { get; set; }
        [Keyword]
        public string link_cv_online { get; set; }
        [Keyword]
        public string link_cv_offline { get; set; }
        [Text]
        public string full_text { get; set; }
        [Keyword]
        public string owner { get; set; }
        /// <summary>
        /// http://www.topcv.vn/viec-lam/nhan-vien-tester/50911.html
        /// </summary>
        [Keyword]
        public string job_link { get; set; }
        [Nest.Ignore]
        public byte[] cv_byte { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ThongTinChungUngVien thong_tin_chung { get; set; }
        [Keyword]
        public string domain { get; set; }
        [Keyword]
        public string custom_id { get; set; }
    }
}
