using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UngVienJobUI.Form_Dialog
{
    public partial class ucPagingJoblink : UserControl
    {
        double _total_pages;
        long _total_recs;
        int _page;
        int _page_size;
        public event EventHandler NextClick;
        public event EventHandler PrevClick;
        public ucPagingJoblink()
        {
            InitializeComponent();
        }
        public int Page_size { get => _page_size; set => _page_size = value; }
        public int Page { get => _page; set => _page = value; }
        public long Total_recs { get => _total_recs; set => _total_recs = value; }
        public void DisplayPaging()
        {
            lblTong.Text = Convert.ToDouble(_total_recs).ToString("0,0");

            _total_pages = Math.Ceiling((double)_total_recs / _page_size);
            txtPage.Text = $"{_page}/{_total_pages}";
            btnNext.Enabled = _page < _total_pages;
            btnPrev.Enabled = _page > 1;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (PrevClick != null)
                PrevClick(this, e);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.NextClick != null)
                this.NextClick(this, e);
        }
    }
}
