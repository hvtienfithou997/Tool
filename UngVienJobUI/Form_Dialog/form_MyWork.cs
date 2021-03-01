using System;
using System.Linq;
using System.Windows.Forms;
using UngVienJobModel;
using UngVienJobUI.Utils;

namespace UngVienJobUI.Form_Dialog
{
    public partial class form_MyWork : Form
    {
        public form_MyWork()
        {
            InitializeComponent();
            check = checkedListBox1;
            yc_kinh_nghiem = cb_kinh_nghiem;
            yc_bang_cap = cb_bang_cap;
            gioi_tinh = cb_gioi_tinh;
            goi_tin = cb_goi_tin;
        }

        private void form_MyWork_Load(object sender, EventArgs e)
        {
            cb_bang_cap.SelectedIndex = 0;
            cb_gioi_tinh.SelectedIndex = 0;
            cb_kinh_nghiem.SelectedIndex = 0;
            cb_goi_tin.SelectedIndex = 0;
            ShowData();
        }

        public void ShowData()
        {
            checkedListBox1.MultiColumn = true;
            Config.list_nganh_nghe_my_work = Config.list_nganh_nghe_my_work.OrderBy(x => x).ToList();
            Config.list_nganh_nghe_my_work.Remove("Ngành nghề khác");
            Config.list_nganh_nghe_my_work.Add("Ngành nghề khác");
            foreach (var item in Config.list_nganh_nghe_my_work)
            {
                checkedListBox1.Items.Add(item);
            }

            var list_box3 = Form1.list_box3;
            if (list_box3.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box3.Items)
                    {
                        if (checkedListBox1.Items[i].ToString() == t.ToString())
                        {
                            checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }
        }

        public static CheckedListBox check = new CheckedListBox();
        public static ComboBox yc_kinh_nghiem = new ComboBox();
        public static ComboBox yc_bang_cap = new ComboBox();
        public static ComboBox gioi_tinh = new ComboBox();
        public static ComboBox goi_tin = new ComboBox();
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count >= 1 && e.CurrentValue != CheckState.Checked)
            {
                e.NewValue = e.CurrentValue;
                MessageBox.Show("Bạn chỉ được chọn 1 ngành nghề");
            }
        }

        private void btn_cancel_mywork_Click(object sender, EventArgs e)
        {
            var list_box3 = Form1.list_box3;
            if (list_box3.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box3.Items)
                    {
                        if (checkedListBox1.Items[i].ToString() == t.ToString())
                        {
                            checkedListBox1.SetItemChecked(i, false);
                        }
                    }
                }
            }
            Close();
        }
    }
}