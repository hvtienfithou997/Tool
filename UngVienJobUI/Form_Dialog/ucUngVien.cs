using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon.RDS.Model;

namespace UngVienJobUI.Form_Dialog
{
    public partial class ucUngVien : UserControl
    {
        int page_size = 50;
        public ucUngVien()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetData();
        }
        void GetData()
        {
            int page = 1;
            long total_rec = 1;


            string term = txtTerm.Text;
            var ngay_tao_tu = XMedia.XUtil.TimeInEpoch(dtTaoTu.Value);
            var ngay_tao_den = XMedia.XUtil.TimeInEpoch(dtTaoDen.Value);

            var lst_data = ES.UngVienRepository.Instance.GetByNguoiTao(term, ngay_tao_tu, ngay_tao_den, page, page_size, new string[] { }, out total_rec);

            grvData.DataSource = lst_data;
        }
    }
}
