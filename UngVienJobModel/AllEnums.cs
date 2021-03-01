using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Nest;

namespace UngVienJobModel
{
    public static class Common
    {
        public const string KHONG_TIM_THAY_UNG_VIEN = "KHONG_TIM_THAY_UNG_VIEN";
    }
    public enum GioiTinh
    {
        [Description("Không")]
        KHONG,
        [Description("Nam")]
        NAM,
        [Description("Nữ")]
        NU
    }
    public enum TrangThai
    {
        DANG_SU_DUNG, KHONG_SU_DUNG, DA_XOA
    }
    public enum TrangThaiXuLy
    {
        CHUA_XU_LY, DANG_XU_LY, DA_XU_LY, LOI
    }
    public enum HanhDong
    {
        LIST, CLICK, WAIT, GO_TO
    }
    public enum LoaiLink
    {
        CONFIG, JOB_LINK
    }

    public enum TrangThaiLink
    {
        NEW, WAIT, OK, COMPLETE
    }

}
