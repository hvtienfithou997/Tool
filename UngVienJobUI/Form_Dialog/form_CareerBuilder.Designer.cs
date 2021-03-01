namespace UngVienJobUI.Form_Dialog
{
    partial class form_CareerBuilder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cb_hinh_thuc_lv = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_kinh_nghiem = new System.Windows.Forms.ComboBox();
            this.cb_cap_bac = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbProvince = new System.Windows.Forms.ComboBox();
            this.cb_provinces = new System.Windows.Forms.Label();
            this.cb_district = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.exp_to = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.exp_from = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(61, 116);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(675, 276);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(113, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(584, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bạn vui lòng điền thêm thông tin để đăng lên Career Builder";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(625, 404);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 34);
            this.button1.TabIndex = 2;
            this.button1.Text = "Lưu lại";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(61, 404);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(111, 34);
            this.button2.TabIndex = 3;
            this.button2.Text = "Hủy bỏ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cb_hinh_thuc_lv
            // 
            this.cb_hinh_thuc_lv.FormattingEnabled = true;
            this.cb_hinh_thuc_lv.Items.AddRange(new object[] {
            "Nhân viên chính thức",
            "Bán thời gian",
            "Thời vụ - Nghề tự do",
            "Thực tập"});
            this.cb_hinh_thuc_lv.Location = new System.Drawing.Point(61, 50);
            this.cb_hinh_thuc_lv.Name = "cb_hinh_thuc_lv";
            this.cb_hinh_thuc_lv.Size = new System.Drawing.Size(133, 24);
            this.cb_hinh_thuc_lv.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Hình thức làm việc";
            // 
            // cb_kinh_nghiem
            // 
            this.cb_kinh_nghiem.FormattingEnabled = true;
            this.cb_kinh_nghiem.Items.AddRange(new object[] {
            "Không yêu cầu kinh nghiệm",
            "Có kinh nghiệm",
            "Chưa có kinh nghiệm"});
            this.cb_kinh_nghiem.Location = new System.Drawing.Point(200, 50);
            this.cb_kinh_nghiem.Name = "cb_kinh_nghiem";
            this.cb_kinh_nghiem.Size = new System.Drawing.Size(132, 24);
            this.cb_kinh_nghiem.TabIndex = 6;
            this.cb_kinh_nghiem.SelectedValueChanged += new System.EventHandler(this.cb_kinh_nghiem_SelectedValueChanged);
            // 
            // cb_cap_bac
            // 
            this.cb_cap_bac.FormattingEnabled = true;
            this.cb_cap_bac.Items.AddRange(new object[] {
            "Sinh viên/ Thực tập sinh",
            "Mới tốt nghiệp",
            "Nhân viên",
            "Trưởng nhóm / Giám sát",
            "Quản lý",
            "Phó Giám đốc",
            "Giám đốc ",
            "Tổng giám đốc",
            "Chủ tịch / Phó Chủ tịch"});
            this.cb_cap_bac.Location = new System.Drawing.Point(338, 50);
            this.cb_cap_bac.Name = "cb_cap_bac";
            this.cb_cap_bac.Size = new System.Drawing.Size(146, 24);
            this.cb_cap_bac.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Kinh nghiệm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(335, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Cấp bậc";
            // 
            // cbProvince
            // 
            this.cbProvince.FormattingEnabled = true;
            this.cbProvince.Location = new System.Drawing.Point(490, 50);
            this.cbProvince.Name = "cbProvince";
            this.cbProvince.Size = new System.Drawing.Size(121, 24);
            this.cbProvince.TabIndex = 10;
            this.cbProvince.SelectedValueChanged += new System.EventHandler(this.cbProvince_SelectedValueChanged);
            // 
            // cb_provinces
            // 
            this.cb_provinces.AutoSize = true;
            this.cb_provinces.Location = new System.Drawing.Point(487, 30);
            this.cb_provinces.Name = "cb_provinces";
            this.cb_provinces.Size = new System.Drawing.Size(117, 17);
            this.cb_provinces.TabIndex = 11;
            this.cb_provinces.Text = "Tỉnh / Thành phố";
            // 
            // cb_district
            // 
            this.cb_district.FormattingEnabled = true;
            this.cb_district.Location = new System.Drawing.Point(617, 50);
            this.cb_district.Name = "cb_district";
            this.cb_district.Size = new System.Drawing.Size(119, 24);
            this.cb_district.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(614, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 17);
            this.label6.TabIndex = 13;
            this.label6.Text = "Quận / huyện";
            // 
            // exp_to
            // 
            this.exp_to.Location = new System.Drawing.Point(99, 9);
            this.exp_to.Name = "exp_to";
            this.exp_to.Size = new System.Drawing.Size(31, 22);
            this.exp_to.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "Đến";
            // 
            // exp_from
            // 
            this.exp_from.Location = new System.Drawing.Point(25, 8);
            this.exp_from.Name = "exp_from";
            this.exp_from.Size = new System.Drawing.Size(31, 22);
            this.exp_from.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-2, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 17);
            this.label7.TabIndex = 18;
            this.label7.Text = "Từ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.exp_to);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.exp_from);
            this.groupBox1.Location = new System.Drawing.Point(200, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 35);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // form_CareerBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cb_district);
            this.Controls.Add(this.cb_provinces);
            this.Controls.Add(this.cbProvince);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cb_cap_bac);
            this.Controls.Add(this.cb_kinh_nghiem);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_hinh_thuc_lv);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedListBox1);
            this.Name = "form_CareerBuilder";
            this.Text = "Thông tin bổ sung";
            this.Load += new System.EventHandler(this.form_CareerBuilder_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cb_hinh_thuc_lv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_kinh_nghiem;
        private System.Windows.Forms.ComboBox cb_cap_bac;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbProvince;
        private System.Windows.Forms.Label cb_provinces;
        private System.Windows.Forms.ComboBox cb_district;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox exp_to;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox exp_from;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}