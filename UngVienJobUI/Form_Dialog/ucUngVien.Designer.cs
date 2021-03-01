namespace UngVienJobUI.Form_Dialog
{
    partial class ucUngVien
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grvData = new System.Windows.Forms.DataGridView();
            this.colTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLienLac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDiaChi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHocVan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKinhNghiem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGioiTinh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colJobLink = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colViTri = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDomain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgayTao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ucPhanTrang1 = new UngVienJobUI.Form_Dialog.ucPhanTrang();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtTerm = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtTaoTu = new System.Windows.Forms.DateTimePicker();
            this.dtTaoDen = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.grvData)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grvData
            // 
            this.grvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTen,
            this.colLienLac,
            this.colDiaChi,
            this.colHocVan,
            this.colKinhNghiem,
            this.colCV,
            this.colGioiTinh,
            this.colJobLink,
            this.colViTri,
            this.colDomain,
            this.colNgayTao});
            this.grvData.Location = new System.Drawing.Point(241, 54);
            this.grvData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grvData.Name = "grvData";
            this.grvData.RowHeadersWidth = 51;
            this.grvData.Size = new System.Drawing.Size(895, 500);
            this.grvData.TabIndex = 0;
            // 
            // colTen
            // 
            this.colTen.HeaderText = "Họ tên";
            this.colTen.MinimumWidth = 6;
            this.colTen.Name = "colTen";
            this.colTen.Width = 125;
            // 
            // colLienLac
            // 
            this.colLienLac.HeaderText = "Liên lạc";
            this.colLienLac.MinimumWidth = 6;
            this.colLienLac.Name = "colLienLac";
            this.colLienLac.Width = 125;
            // 
            // colDiaChi
            // 
            this.colDiaChi.HeaderText = "Đ/chỉ";
            this.colDiaChi.MinimumWidth = 6;
            this.colDiaChi.Name = "colDiaChi";
            this.colDiaChi.Width = 125;
            // 
            // colHocVan
            // 
            this.colHocVan.HeaderText = "Học vấn";
            this.colHocVan.MinimumWidth = 6;
            this.colHocVan.Name = "colHocVan";
            this.colHocVan.Width = 125;
            // 
            // colKinhNghiem
            // 
            this.colKinhNghiem.HeaderText = "K/nghiệm";
            this.colKinhNghiem.MinimumWidth = 6;
            this.colKinhNghiem.Name = "colKinhNghiem";
            this.colKinhNghiem.Width = 125;
            // 
            // colCV
            // 
            this.colCV.HeaderText = "CV";
            this.colCV.MinimumWidth = 6;
            this.colCV.Name = "colCV";
            this.colCV.Width = 125;
            // 
            // colGioiTinh
            // 
            this.colGioiTinh.HeaderText = "G/tính";
            this.colGioiTinh.MinimumWidth = 6;
            this.colGioiTinh.Name = "colGioiTinh";
            this.colGioiTinh.Width = 50;
            // 
            // colJobLink
            // 
            this.colJobLink.HeaderText = "Job";
            this.colJobLink.MinimumWidth = 6;
            this.colJobLink.Name = "colJobLink";
            this.colJobLink.Width = 125;
            // 
            // colViTri
            // 
            this.colViTri.HeaderText = "V/trí";
            this.colViTri.MinimumWidth = 6;
            this.colViTri.Name = "colViTri";
            this.colViTri.Width = 125;
            // 
            // colDomain
            // 
            this.colDomain.HeaderText = "Site";
            this.colDomain.MinimumWidth = 6;
            this.colDomain.Name = "colDomain";
            this.colDomain.Width = 70;
            // 
            // colNgayTao
            // 
            this.colNgayTao.HeaderText = "Tạo";
            this.colNgayTao.MinimumWidth = 6;
            this.colNgayTao.Name = "colNgayTao";
            this.colNgayTao.Width = 125;
            // 
            // ucPhanTrang1
            // 
            this.ucPhanTrang1.Location = new System.Drawing.Point(934, 5);
            this.ucPhanTrang1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ucPhanTrang1.Name = "ucPhanTrang1";
            this.ucPhanTrang1.Page = 0;
            this.ucPhanTrang1.Page_size = 0;
            this.ucPhanTrang1.Size = new System.Drawing.Size(201, 41);
            this.ucPhanTrang1.TabIndex = 1;
            this.ucPhanTrang1.Total_recs = ((long)(0));
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(61, 393);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Tìm";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtTerm
            // 
            this.txtTerm.Location = new System.Drawing.Point(61, 18);
            this.txtTerm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTerm.Name = "txtTerm";
            this.txtTerm.Size = new System.Drawing.Size(165, 22);
            this.txtTerm.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tên";
            // 
            // dtTaoTu
            // 
            this.dtTaoTu.CustomFormat = "dd-MM-yyyy";
            this.dtTaoTu.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTaoTu.Location = new System.Drawing.Point(52, 23);
            this.dtTaoTu.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtTaoTu.Name = "dtTaoTu";
            this.dtTaoTu.Size = new System.Drawing.Size(111, 22);
            this.dtTaoTu.TabIndex = 5;
            // 
            // dtTaoDen
            // 
            this.dtTaoDen.CustomFormat = "dd-MM-yyyy";
            this.dtTaoDen.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTaoDen.Location = new System.Drawing.Point(52, 53);
            this.dtTaoDen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtTaoDen.Name = "dtTaoDen";
            this.dtTaoDen.Size = new System.Drawing.Size(111, 22);
            this.dtTaoDen.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "từ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 58);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "đến";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtTaoDen);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtTaoTu);
            this.groupBox1.Location = new System.Drawing.Point(15, 54);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(219, 89);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ngày tạo";
            // 
            // ucUngVien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTerm);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.ucPhanTrang1);
            this.Controls.Add(this.grvData);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ucUngVien";
            this.Size = new System.Drawing.Size(1140, 558);
            ((System.ComponentModel.ISupportInitialize)(this.grvData)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grvData;
        private ucPhanTrang ucPhanTrang1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtTerm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtTaoTu;
        private System.Windows.Forms.DateTimePicker dtTaoDen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLienLac;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDiaChi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHocVan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colKinhNghiem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCV;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGioiTinh;
        private System.Windows.Forms.DataGridViewTextBoxColumn colJobLink;
        private System.Windows.Forms.DataGridViewTextBoxColumn colViTri;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDomain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgayTao;
    }
}
