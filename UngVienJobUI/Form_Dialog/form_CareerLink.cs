using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenQA.Selenium.Chrome;
using UngVienJobUI.Utils;

namespace UngVienJobUI.Form_Dialog
{
    public partial class form_CareerLink : Form
    {
        public form_CareerLink()
        {
            InitializeComponent();
            check = checkedListBox1;
        }

        private void form_CareerLink_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        public void ShowData()
        {
            checkedListBox1.MultiColumn = true;
            Config.list_nganh_nghe_careerlink = Config.list_nganh_nghe_careerlink.OrderBy(x => x).ToList();
            Config.list_nganh_nghe_careerlink.Remove("Khác");
            Config.list_nganh_nghe_careerlink.Add("Khác");
            foreach (var item in Config.list_nganh_nghe_careerlink)
            {
                checkedListBox1.Items.Add(item);
            }
            var list_box4 = Form1.list_box4;
            if (list_box4.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box4.Items)
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
        public static ComboBox cb_capbac = new ComboBox();
        public static ComboBox cb_gioitinh = new ComboBox();
        public static ComboBox cb_hocvan = new ComboBox();
        public static ComboBox cb_kinhnghiem = new ComboBox();
        public static ComboBox cb_ngon_ngu = new ComboBox();

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count >= 3 && e.CurrentValue != CheckState.Checked)
            {
                e.NewValue = e.CurrentValue;
                MessageBox.Show("Bạn chỉ được chọn 3 ngành, Career chỉ cho chọn tối đa 3 ngành.");
            }
        }

        private void btn_cancel_careerink_Click(object sender, EventArgs e)
        {
            var list_box4 = Form1.list_box4;
            if (list_box4.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box4.Items)
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