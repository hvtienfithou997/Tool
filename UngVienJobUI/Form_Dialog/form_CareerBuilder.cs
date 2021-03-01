using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UngVienJobUI.Utils;

namespace UngVienJobUI.Form_Dialog
{
    public partial class form_CareerBuilder : Form
    {
        public static CheckedListBox check = new CheckedListBox();
        public static ComboBox cb_hinh_thuc = new ComboBox();
        public static ComboBox cb_Kn = new ComboBox();
        public static ComboBox cb_capBac = new ComboBox();
        public static ComboBox district = new ComboBox();
        public static ComboBox province = new ComboBox();
        public static TextBox luong_from = new TextBox();
        public static TextBox luong_to = new TextBox();
        public form_CareerBuilder()
        {
            InitializeComponent();
            groupBox1.Hide();
            check = checkedListBox1;
            luong_from = exp_from;
            luong_to = exp_to;
            cb_hinh_thuc = cb_hinh_thuc_lv;
            cb_Kn = cb_kinh_nghiem;
            cb_capBac = cb_cap_bac;
            district = cb_district;
            province = cbProvince;
        }
        private void form_CareerBuilder_Load(object sender, EventArgs e)
        {
            cb_hinh_thuc_lv.SelectedIndex = 0;
            cb_Kn.SelectedIndex = 0;
            cb_capBac.SelectedIndex = 0;
            ShowData();
            cbProvince.DataSource = Data.provinces.Select(x => x.name).ToList();
            cbProvince.SelectedIndex = 38;
            
        }
        private void cbProvince_SelectedValueChanged(object sender, EventArgs e)
        {
            var name = cbProvince.Text;
            ShowDistrict(name);
        }
        private void ShowDistrict(string prov)
        {
            List<District> diss = new List<District>();
            foreach (var dis in Data.provinces)
            {
                if (dis.name == prov)
                {
                    diss.AddRange(dis.Districts);
                }
            }

            cb_district.DataSource = diss.Select(x => x.name).ToList();
        }


        
        public void ShowData()
        {
            checkedListBox1.MultiColumn = true;
            Config.list_nganh_nghe_careerbuilder = Config.list_nganh_nghe_careerbuilder.OrderBy(x => x).ToList();
            Config.list_nganh_nghe_careerbuilder.Remove("Ngành khác");
            Config.list_nganh_nghe_careerbuilder.Add("Ngành khác");
            foreach (var item in Config.list_nganh_nghe_careerbuilder)
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

        private void button2_Click(object sender, EventArgs e)
        {
            var list_box6 = Form1.list_box6;
            if (list_box6.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box6.Items)
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

        private void cb_kinh_nghiem_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cb_Kn.Text != "Có kinh nghiệm")
            {
                groupBox1.Hide();
            }
            else
            {
                groupBox1.Show();
            }
        }
    }
}
