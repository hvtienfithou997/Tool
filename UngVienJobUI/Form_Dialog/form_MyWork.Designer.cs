namespace UngVienJobUI.Form_Dialog
{
    partial class form_MyWork
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
            this.button1 = new System.Windows.Forms.Button();
            this.cb_gioi_tinh = new System.Windows.Forms.ComboBox();
            this.cb_bang_cap = new System.Windows.Forms.ComboBox();
            this.cb_kinh_nghiem = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_goi_tin = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_cancel_mywork = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(53, 103);
            this.checkedListBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(695, 276);
            this.checkedListBox1.TabIndex = 2;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(633, 383);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 34);
            this.button1.TabIndex = 3;
            this.button1.Text = "Lưu lại";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cb_gioi_tinh
            // 
            this.cb_gioi_tinh.FormattingEnabled = true;
            this.cb_gioi_tinh.Items.AddRange(new object[] {
            "Không yêu cầu",
            "Nam",
            "Nữ"});
            this.cb_gioi_tinh.Location = new System.Drawing.Point(363, 74);
            this.cb_gioi_tinh.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cb_gioi_tinh.Name = "cb_gioi_tinh";
            this.cb_gioi_tinh.Size = new System.Drawing.Size(121, 24);
            this.cb_gioi_tinh.TabIndex = 6;
            // 
            // cb_bang_cap
            // 
            this.cb_bang_cap.FormattingEnabled = true;
            this.cb_bang_cap.Items.AddRange(new object[] {
            "Lao động phổ thông",
            "Trung học",
            "Trung cấp",
            "Cao đẳng",
            "Đại học",
            "Trên đại học"});
            this.cb_bang_cap.Location = new System.Drawing.Point(235, 74);
            this.cb_bang_cap.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cb_bang_cap.Name = "cb_bang_cap";
            this.cb_bang_cap.Size = new System.Drawing.Size(121, 24);
            this.cb_bang_cap.TabIndex = 7;
            // 
            // cb_kinh_nghiem
            // 
            this.cb_kinh_nghiem.FormattingEnabled = true;
            this.cb_kinh_nghiem.Items.AddRange(new object[] {
            "Chưa có kinh nghiệm",
            "Dưới 1 năm",
            "1 năm",
            "2 năm",
            "3 năm",
            "4 năm",
            "5 năm",
            "Hơn 5 năm",
            "Không yêu cầu"});
            this.cb_kinh_nghiem.Location = new System.Drawing.Point(108, 74);
            this.cb_kinh_nghiem.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cb_kinh_nghiem.Name = "cb_kinh_nghiem";
            this.cb_kinh_nghiem.Size = new System.Drawing.Size(121, 24);
            this.cb_kinh_nghiem.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Kinh nghiệm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Bằng cấp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(359, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Giới tính";
            // 
            // cb_goi_tin
            // 
            this.cb_goi_tin.FormattingEnabled = true;
            this.cb_goi_tin.Items.AddRange(new object[] {
            "Việc làm tuyển gấp",
            "Việc làm hấp dẫn",
            "Việc làm lương cao",
            "Tuyển dụng nhanh",
            "Tuyển dụng tiêu điểm",
            "Tuyển dụng hấp dẫn (ngành)"});
            this.cb_goi_tin.Location = new System.Drawing.Point(489, 74);
            this.cb_goi_tin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cb_goi_tin.Name = "cb_goi_tin";
            this.cb_goi_tin.Size = new System.Drawing.Size(153, 24);
            this.cb_goi_tin.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(485, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Chọn gói đăng tin";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(93, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(589, 25);
            this.label5.TabIndex = 14;
            this.label5.Text = "Bạn vui lòng điền thêm các thông tin dưới đây để đăng lên MyWork";
            // 
            // btn_cancel_mywork
            // 
            this.btn_cancel_mywork.Location = new System.Drawing.Point(53, 383);
            this.btn_cancel_mywork.Name = "btn_cancel_mywork";
            this.btn_cancel_mywork.Size = new System.Drawing.Size(89, 34);
            this.btn_cancel_mywork.TabIndex = 15;
            this.btn_cancel_mywork.Text = "Hủy bỏ";
            this.btn_cancel_mywork.UseVisualStyleBackColor = true;
            this.btn_cancel_mywork.Click += new System.EventHandler(this.btn_cancel_mywork_Click);
            // 
            // form_MyWork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_cancel_mywork);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cb_goi_tin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_kinh_nghiem);
            this.Controls.Add(this.cb_bang_cap);
            this.Controls.Add(this.cb_gioi_tinh);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkedListBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "form_MyWork";
            this.Text = "Thông tin bổ sung";
            this.Load += new System.EventHandler(this.form_MyWork_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cb_gioi_tinh;
        private System.Windows.Forms.ComboBox cb_bang_cap;
        private System.Windows.Forms.ComboBox cb_kinh_nghiem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_goi_tin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_cancel_mywork;
    }
}