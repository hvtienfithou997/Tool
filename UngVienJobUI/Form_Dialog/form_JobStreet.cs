using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UngVienJobUI.Utils;

namespace UngVienJobUI.Form_Dialog
{
    public partial class form_JobStreet : Form
    {
        public form_JobStreet()
        {
            InitializeComponent();

            var lst = new List<string>();
            foreach (string item in checkedListBox1.CheckedItems)
            {
                lst.Add(item);
            }
            comboBox1.SelectedIndex = 0;
            check = checkedListBox1;
            txt_tenct = textBox1;
            cb_quymo = comboBox1;
        }

        private void form_JobStreet_Load(object sender, EventArgs e)
        {
            ShowData();
            label1.Text = "Chọn 01 ngành nghề vì JobStreet không hỗ trợ chọn nhiều ngành";
        }

        public void ShowData()
        {
            checkedListBox1.MultiColumn = true;
            Config.list_nganh_nghe_jobstreet = Config.list_nganh_nghe_jobstreet.OrderBy(x => x).ToList();
            Config.list_nganh_nghe_jobstreet.Remove("Ngành khác");
            Config.list_nganh_nghe_jobstreet.Add("Ngành khác");
            foreach (var item in Config.list_nganh_nghe_jobstreet)
            {
                checkedListBox1.Items.Add(item);
            }
            var list_box2 = Form1.list_box2;
            var ten_ct = Form1.lb_tenct;
            if (list_box2.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box2.Items)
                    {
                        if (checkedListBox1.Items[i].ToString() == t.ToString())
                        {
                            checkedListBox1.SetItemChecked(i, true);
                        }
                    }
                }
            }

            if (ten_ct.Text.Length > 0)
            {
                textBox1.Text = ten_ct.Text;
            }
        }

        public static CheckedListBox check = new CheckedListBox();
        public static TextBox txt_tenct = new TextBox();
        public static ComboBox cb_quymo = new ComboBox();

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length == 0)
            {
                MessageBox.Show($"Bạn phải nhập đầy đủ các trường");
            }
            else
            {
                Close();
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selNdx = checkedListBox1.SelectedIndex;

            foreach (int cbNdx in checkedListBox1.CheckedIndices)
            {
                if (cbNdx != selNdx)
                {
                    checkedListBox1.SetItemChecked(cbNdx, false);
                }
            }
        }

        private void btn_cancel_jobstreet_Click(object sender, EventArgs e)
        {
            var list_box2 = Form1.list_box2;
            var ten_ct = Form1.lb_tenct;
            if (list_box2.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box2.Items)
                    {
                        if (checkedListBox1.Items[i].ToString() == t.ToString())
                        {
                            checkedListBox1.SetItemChecked(i, false);
                        }
                    }
                }
            }
            if (ten_ct.Text.Length > 0)
            {
                textBox1.Text = "";
            }
            Close();
        }
    }
}