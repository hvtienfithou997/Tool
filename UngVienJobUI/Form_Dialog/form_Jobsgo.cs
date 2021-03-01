using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using UngVienJobUI.Utils;

namespace UngVienJobUI.Form_Dialog
{
    public partial class form_Jobsgo : Form
    {
        public static CheckedListBox check = new CheckedListBox();
        public static ComboBox yc_bang_cap = new ComboBox();
        public static ComboBox vi_tri = new ComboBox();
        public static TextBox luong_from = new TextBox();
        public static TextBox luong_to = new TextBox();

        public form_Jobsgo()
        {
            InitializeComponent();
            check = checkedListBox1;
            yc_bang_cap = comboBox1;
            vi_tri = comboBox2;
            luong_from = exp_from;
            luong_to = exp_to;
            comboBox2.SelectedIndex = 1;
            comboBox1.SelectedIndex = 2;
            foreach (var conf in groupBox1.Controls)
            {
                if (conf is TextBox)
                {
                    ((TextBox)conf).Enabled = false;
                }
            }
            ShowData();
        }

        public void ShowData()
        {
            checkedListBox1.MultiColumn = true;
            Config.list_nganh_nghe_jobsgo = Config.list_nganh_nghe_jobsgo.OrderBy(x => x).ToList();
            
            checkedListBox1.DataSource = new BindingSource(Config.lst_jobsgo().Values,null);



            var list_box5 = Form1.list_box5;

            if (list_box5.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box5.Items)
                    {
                        if (checkedListBox1.Items[i].ToString() == t.ToString().Replace("--- ",""))
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

        private void button2_Click(object sender, EventArgs e)
        {
            var list_box5 = Form1.list_box5;
            if (list_box5.Items.Count > 0)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    //
                    foreach (var t in list_box5.Items)
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

        private void ck_yc_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_yc.Checked)
            {
                foreach (var conf in groupBox1.Controls)
                {
                    if (conf is TextBox)
                    {
                        ((TextBox)conf).Enabled = true;
                    }
                }
                exp_from.Text = string.Empty;
                exp_to.Text = string.Empty;
            }
            else
            {
                foreach (var conf in groupBox1.Controls)
                {
                    if (conf is TextBox)
                    {
                        ((TextBox)conf).Enabled = false;
                    }
                }
            }
        }

       
    }
}