using System.Collections.Generic;

namespace UngVienJobModel
{
    public class DangTin : BaseET
    {
       
        public string chuc_danh { get; set; }
        public string ten_cong_ty { get; set; }
        public string dia_chi { get; set; }
        public string dia_chi_chi_tiet { get; set; }
        public string quy_mo_doanh_nghiep { get; set; }
        public int so_luong_tuyen { get; set; }
        public string muc_luong { get; set; }
        public string cap_bac { get; set; }
        public string loai_hinh_cong_viec { get; set; }
        public List<string> nganh_nghe { get; set; }
        public string mo_ta_cong_viec { get; set; }
        public string yeu_cau_cong_viec { get; set; }
        public string quyen_loi_ung_vien { get; set; }
        public string link_jd { get; set; }
        public string yeu_cau_kinh_nghiem { get; set; }
        public string bang_cap { get; set; }
        public string gioi_tinh { get; set; }
        public string so_dien_thoai { get; set; }
        public string goi_tin_tuyen_dung { get; set; }
        public string lien_he { get; set; }
        public string kinh_nghiem_from { get; set; }
        public string kinh_nghiem_to { get; set; }
        public string district { get; set; }
        public int thuoc_tinh { get; set; }
       
        public string url_tin_tuc { get; set; }
        
    }
}