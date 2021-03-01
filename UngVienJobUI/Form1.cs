using CareerBuilderHelper;
using ES;
using JobsGoHelper;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopCVHelper;
using UngVienJobModel;
using UngVienJobUI.Form_Dialog;
using UngVienJobUI.Site;
using UngVienJobUI.Utils;
using UVJHelper;
using Keys = System.Windows.Forms.Keys;

namespace UngVienJobUI
{
    public partial class Form1 : Form, IDisposable
    {
        private string curent_exe_path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        private string user_profile_path =
            $"{System.IO.Path.GetDirectoryName(Application.ExecutablePath)}\\profiles\\";

        private string folder_cv = $"{System.IO.Path.GetDirectoryName(Application.ExecutablePath)}\\CV\\";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Form1).Name);
        protected int page = 1;
        private ContextMenuStrip mnuJob = new ContextMenuStrip();
        private ContextMenuStrip mnuUngVien = new ContextMenuStrip();
        private ToolStripMenuItem rmRefreshLinkJob = new ToolStripMenuItem("Làm mới");
        private ToolStripMenuItem rmRefreshUngVien = new ToolStripMenuItem("Làm mới");
        private ToolStripMenuItem rmGetCVFromLink = new ToolStripMenuItem("Lấy CV các link này");
        private ToolStripMenuItem rmGetFolderCV = new ToolStripMenuItem("Mở CV ứng viên này");
        private ContextMenuStrip mnuGoodCv = new ContextMenuStrip();
        private ToolStripMenuItem RefreshGoodCv = new ToolStripMenuItem("Cập nhật trạng thái");
        private ToolStripMenuItem GetCvGoodCv = new ToolStripMenuItem("Lấy CV trang này");

        private void LoadMenuJob()
        {
            rmRefreshLinkJob.ShortcutKeys = Keys.F5;
            rmRefreshLinkJob.Click += new EventHandler(rmRefresh_Click);
            mnuJob.Items.Add(rmRefreshLinkJob);
            mnuJob.Items.Add(new ToolStripSeparator());
            rmGetCVFromLink.Click += RmGetCVFromLink_Click;
            rmGetFolderCV.Click += RmGetFolderCv_Click;
            mnuJob.Items.Add(rmGetCVFromLink);
            rmRefreshUngVien.ShortcutKeys = Keys.F5;
            rmRefreshUngVien.Click += new EventHandler(rmRefresh_Click);
            mnuUngVien.Items.Add(rmRefreshUngVien);
            mnuUngVien.Items.Add(new ToolStripSeparator());
            mnuUngVien.Items.Add(rmGetFolderCV);
        }

        private void RmGetFolderCv_Click(object sender, EventArgs e)
        {
            try
            {
                var link_cv = GetSelectedLinkCv();
                foreach (var cv in link_cv)
                {
                    if (!File.Exists(folder_cv + cv))
                    {
                        Process.Start(folder_cv + cv);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void RmGetCVFromLink_Click(object sender, EventArgs e)
        {
            var links = GetSelectedJobLink();
            var list_job_link = new List<JobLink>();
            JobLink job_link;
            foreach (var link in links)
            {
                job_link = new JobLink(LoaiLink.JOB_LINK) { link = link };

                list_job_link.Add(job_link);
            }

            var lst = new List<JobLink>();
            if (list_job_link.Count > 0)
            {
                foreach (var link in list_job_link)
                {
                    if (!string.IsNullOrEmpty(link.link))
                    {
                        string url = link.link;
                        var uri = new Uri(url);
                        string url_host = uri.Host.Replace("www.", "");
                        var job = new JobLink(LoaiLink.JOB_LINK) { link = url, app_id = url_host };
                        lst.Add(job);
                    }
                }
            }

            if (lst.Count > 0)
            {
                foreach (var item in lst.GroupBy(x => x.app_id))
                {
                    switch (item.Key)
                    {
                        case "mywork.com.vn":
                            await Task.Run(() => XuLyCvSingle("MyWork", item.ToList()));
                            break;

                        case "careerlink.vn":
                            await Task.Run(() => XuLyCvSingle("CareerLink", item.ToList()));
                            break;

                        case "employer.jobstreet.vn":
                            await Task.Run(() => XuLyCvSingle("JobStreet", item.ToList()));
                            break;

                        case "topcv.vn":
                            await Task.Run(() => XuLyCvSingle("TopCv", item.ToList()));
                            break;

                        case "jobsgo.vn":
                            await Task.Run(() => XuLyCvSingle("JobsGo", item.ToList()));
                            break;

                        case "careerbuilder.vn":
                            await Task.Run(() => XuLyCvSingle("CareerBuilder", item.ToList()));
                            break;

                        case "vn.joboko.com":
                            await Task.Run(() => XuLyCvSingle("Joboko", item.ToList()));
                            break;
                    }
                }
            }
        }

        private void rmRefresh_Click(object sender, EventArgs e)
        {
            GetDataJobLink();
            SearchCv();
        }

        private string[] GetSelectedJobLink()
        {
            HashSet<string> hs = new HashSet<string>();
            try
            {
                for (int i = 0; i < grvJobLink.SelectedCells.Count; i++)
                {
                    DataGridViewRow row = grvJobLink.Rows[grvJobLink.SelectedCells[i].RowIndex];
                    string id = row.Cells["link_job"].Value.ToString();
                    hs.Add(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return hs.ToArray();
        }

        private string[] GetSelectedLinkCv()
        {
            HashSet<string> hs = new HashSet<string>();
            try
            {
                for (int i = 0; i < grvData.SelectedCells.Count; i++)
                {
                    DataGridViewRow row = grvData.Rows[grvData.SelectedCells[i].RowIndex];
                    string id = row.Cells["colCV"].Value.ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        hs.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return hs.ToArray();
        }

        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(user_profile_path))
                Directory.CreateDirectory(user_profile_path);
            Dictionary<string, string> dic_sort_field_cv = new Dictionary<string, string>()
            {
                {"ngay_tao", "Ngày tạo"},
                {"ho_ten.keyword", "Họ tên"},
                {"so_dien_thoai.keyword", "Số điện thoại"}
            };
            cboSortFieldCV.DataSource = new BindingSource(dic_sort_field_cv, null);
            cboSortFieldCV.DisplayMember = "Value";
            cboSortFieldCV.ValueMember = "Key";

            Dictionary<string, string> dic_sort_field_job_link = new Dictionary<string, string>()
            {
                {"ten_job.keyword", "Tên Job"},
                {"ngay_sua", "Ngày sửa"},
                {"tong_so_cv", "Tổng CV"}
            };
            cboSortFieldJobLink.DataSource = new BindingSource(dic_sort_field_job_link, null);
            cboSortFieldJobLink.DisplayMember = "Value";
            cboSortFieldJobLink.ValueMember = "Key";

            Dictionary<string, string> dic_sort_order_cv = new Dictionary<string, string>()
            {
                {"0", "Giảm dần"},
                {"1", "Tăng dần"}
            };
            cboSortOrderCV.DataSource = new BindingSource(dic_sort_order_cv, null);
            cboSortOrderCV.DisplayMember = "Value";
            cboSortOrderCV.ValueMember = "Key";

            Dictionary<string, string> dic_sort_order_job_link = new Dictionary<string, string>()
            {
                {"0", "Giảm dần"},
                {"1", "Tăng dần"}
            };
            cboSortOrderJobLink.DataSource = new BindingSource(dic_sort_order_job_link, null);
            cboSortOrderJobLink.DisplayMember = "Value";
            cboSortOrderJobLink.ValueMember = "Key";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(folder_cv))
            {
                Directory.CreateDirectory(folder_cv);
            }

            list_box2 = listBox2;
            list_box3 = listBox3;
            list_box4 = listBox4;
            list_box5 = listBox5;
            list_box6 = listBox6;
            lb_tenct = lb_company_name;
            BindAddress();
            BindNganhNge();

            cbLoaiHinhJob.SelectedIndex = 0;
            cbCapBacJob.SelectedIndex = 1;
            txtNganhNgheJob.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtNganhNgheJob.AutoCompleteSource = AutoCompleteSource.ListItems;
            Dictionary<string, string> process_status = new Dictionary<string, string>
            {
                {"-1", "Tất cả"},
                {"0", "Chưa xử lý"},
                {"1", "Đang xử lý"},
                {"2", "Đã xử lý"},
                {"3", "Lỗi"}
            };
            cbProcessStatus.DataSource = new BindingSource(process_status, null);
            cbProcessStatus.DisplayMember = "Value";
            cbProcessStatus.ValueMember = "Key";
            //
            Dictionary<string, string> site_name = new Dictionary<string, string>
            {
                {"-1", "Tất cả"},
                {"jobstreet", "JobStreet"},
                {"topcv", "TopCv"},
                {"mywork", "MyWork"},
                {"careerlink", "CareerLink"},
                {"jobsgo", "JobsGo"},
                {"careerbuilder", "CareerBuilder"},
                {"joboko", "Joboko"}
            };
            cbSiteName.DataSource = new BindingSource(site_name, null);
            cbSiteName.DisplayMember = "Value";
            cbSiteName.ValueMember = "Key";

            //

            Dictionary<string, string> site_name_uv = new Dictionary<string, string>
            {
                {"-1", "Tất cả"},
                {"jobstreet", "JobStreet"},
                {"topcv", "TopCv"},
                {"mywork", "MyWork"},
                {"careerlink", "CareerLink"},
                {"jobsgo", "JobsGo"},
                {"careerbuilder", "CareerBuilder"},
                {"joboko", "Joboko"}
            };
            cbSiteNameUv.DataSource = new BindingSource(site_name_uv, null);
            cbSiteNameUv.DisplayMember = "Value";
            cbSiteNameUv.ValueMember = "Key";
            //

            Dictionary<string, string> GioiTinh = new Dictionary<string, string>
            {
                {"0", "Tất cả"},
                {"1", "Nam"},
                {"2", "Nữ"}
            };
            cbGioiTinh.DataSource = new BindingSource(GioiTinh, null);
            cbGioiTinh.DisplayMember = "Value";
            cbGioiTinh.ValueMember = "Key";

            /**/

            LoadAccountConfig();
            dtTaoTu.Value = DateTime.Now.AddMonths(-1);
            if (_ucPaging != null)
            {
                _ucPaging.NextClick += _ucPaging_NextClick;
                _ucPaging.PrevClick += _ucPaging_PrevClick;
            }

            if (ucPagingJoblink1 != null)
            {
                ucPagingJoblink1.NextClick += _UcPagingJob_NextClick;
                ucPagingJoblink1.PrevClick += _UcPagingJob_PrevClick;
            }

            dtFromJobLink.Value = new DateTime(2000, 01, 01);
            dtTaoTu.Value = new DateTime(2000, 01, 01);

            SearchCv();
            GetDataJobLink();
            LoadMenuJob();
#if !DEBUG
            button1.Visible = false; button2.Visible = false; label15.Visible = false;
            btn_upload_file.Visible = false;
            textBox1.Visible = false;
            label2.Visible = false;
#endif
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //ucPagingGoodCv1 uc = new ucPagingGoodCv1();
        }

        private List<string> pdfFiles = new List<string>();

        private void GetTextFromFile(string file_path)
        {
            try
            {
                string text = File.ReadAllText(file_path);

                string[] lines = File.ReadAllLines(file_path);
                foreach (string line in lines)
                    Console.WriteLine(line);

                using (StreamReader file = new StreamReader(file_path))
                {
                    string ln;

                    List<string> lst_text = new List<string>();
                    while ((ln = file.ReadLine()) != null)
                    {
                        lst_text.Add(ln);
                    }

                    var jd = string.Join(" ", lst_text);

                    string chuc_danh = FillText(jd, Config.chuc_danh_start, Config.chuc_danh_end);
                    string mo_ta = FillText(jd, Config.mo_ta_start, Config.mo_ta_end);
                    string yeu_cau = FillText(jd, Config.yeu_cau_start, Config.yeu_cau_end);
                    string thoi_gian_lam_viec = FillText(jd, Config.thoi_gian_start, Config.thoi_gian_end);
                    string muc_luong = FillText(jd, Config.luong_start, Config.luong_end);
                    string quyen_loi = FillText(jd, Config.quyen_loi_start, Config.quyen_loi_end);
                    string lien_he = FillText(jd, Config.lien_he_start, Config.lien_he_end);
                    string dia_chi_lam_viec = FillText(jd, Config.dia_chi_lam_viec_start, Config.dia_chi_lam_viec_end);

                    rtbMoTaJob.Text = mo_ta;
                    txtChucDanhJob.Text = chuc_danh;
                    rtbYeuCauJob.Text = yeu_cau;
                    rtbThoiGianJob.Text = thoi_gian_lam_viec;
                    cbMucLuongJob.Text = muc_luong;
                    rtbQuyenLoiJob.Text = quyen_loi;
                    rtbLienHeJob.Text = lien_he;
                    txtDiaChiJob.Text = dia_chi_lam_viec;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string FillText(string jd, string text_start, string text_end)
        {
            string need = string.Empty;
            if (jd.Contains(text_start) && jd.Contains(text_end))
            {
                int start = jd.IndexOf(text_start);
                int end = jd.IndexOf(text_end) - text_end.Length;
                var total_string = end - start;
                start = start + text_start.Length + 1;
                need = jd.Substring(start, total_string);
            }

            return need;
        }

        private void BindAddress()
        {
            lstboxTinhThanhJob.DataSource = Config.all_tinh_thanh;
        }

        private void BindNganhNge()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = Config.list_nganh_nghe_top_cv;
            txtNganhNgheJob.DataSource = bs;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private async void btn_post_Click(object sender, EventArgs e)
        {
            if (ValidationFields())
            {
                bool is_debug = cboDebug.SelectedIndex == 0;
                btn_post.Enabled = false;
                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng";
                string msg = "";
                UngVienJobModel.ChiTietTinModel dang_tin = new UngVienJobModel.ChiTietTinModel();
                try
                {
                    dang_tin.chuc_danh = txtChucDanhJob.Text;
                    dang_tin.so_luong_tuyen = Convert.ToInt32(txtSoLuongJob.Text);
                    dang_tin.cap_bac = cbCapBacJob.Text;
                    dang_tin.loai_hinh_cong_viec = cbLoaiHinhJob.Text;
                    dang_tin.dia_chi_chi_tiet = txtDiaChiJob.Text;
                    dang_tin.nganh_nghe = new List<string> { txtNganhNgheJob.Text };
                    dang_tin.muc_luong = cbMucLuongJob.Text;
                    dang_tin.dia_chi = lstboxTinhThanhJob.SelectedItem.ToString();
                    dang_tin.yeu_cau_cong_viec = rtbYeuCauJob.Text;
                    dang_tin.mo_ta_cong_viec = rtbMoTaJob.Text;
                    dang_tin.quyen_loi_ung_vien = rtbQuyenLoiJob.Text;
                    dang_tin.so_dien_thoai = txtSdtJob.Text;
                    dang_tin.lien_he = rtbLienHeJob.Text;
                    dang_tin.email = txt_email.Text;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    if (CountCheckBox() < 1)
                    {
                        MessageBox.Show("Chưa chọn Site");
                    }
                    else
                    {
                        Dictionary<string, bool> check_status_post = new Dictionary<string, bool>();

                        if (check_my_work.Checked)
                        {
                            var task_my_work = await Task.Run(() =>
                            {
                                var kvp = ReadAccountSite("MyWork");
                                var conf = ES.JobLinkRepository.Instance.GetCauHinh("mywork.com.vn");
                                conf.username = kvp.username;
                                conf.password = kvp.password;
                                conf.xpath_username = kvp.xpath_username;
                                conf.xpath_password = kvp.xpath_password;
                                if (listBox3.Items.Count > 0)
                                {
                                    List<string> lst = new List<string>();
                                    foreach (string item in listBox3.Items)
                                    {
                                        lst.Add(item);
                                    }

                                    dang_tin.nganh_nghe = lst;
                                }

                                if (!string.IsNullOrEmpty(lb_gioi_tinh.Text))
                                {
                                    dang_tin.gioi_tinh = lb_gioi_tinh.Text;
                                }

                                if (!string.IsNullOrEmpty(lb_bang_cap.Text))
                                {
                                    dang_tin.bang_cap = lb_bang_cap.Text;
                                }

                                if (!string.IsNullOrEmpty(lb_exp.Text))
                                {
                                    dang_tin.yeu_cau_kinh_nghiem = lb_exp.Text;
                                }

                                if (!string.IsNullOrWhiteSpace(lb_goi_tin.Text))
                                {
                                    dang_tin.goi_tin_tuyen_dung = lb_goi_tin.Text;
                                }

                                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng: MyWork";
                                MyWorkPost my_work = new MyWorkPost();
                                var myWork = my_work.ExtractThongTin(conf, "https://mywork.com.vn/nha-tuyen-dung/dang-tin",
                                    "xmedia.vn", out msg, dang_tin, is_debug);
                                check_status_post.Add("MyWork", myWork);
                                return check_status_post;
                            });
                        }

                        if (check_job_street.Checked)
                        {
                            var task_job_street = await Task.Run(() =>
                            {
                                var kvp = ReadAccountSite("JobStreet");
                                var conf = ES.JobLinkRepository.Instance.GetCauHinh("jobstreet.vn");
                                conf.username = kvp.username;
                                conf.password = kvp.password;
                                conf.xpath_username = kvp.xpath_username;
                                conf.xpath_password = kvp.xpath_password;
                                if (listBox2.Items.Count > 0)
                                {
                                    List<string> lst = new List<string>();
                                    foreach (string item in listBox2.Items)
                                    {
                                        lst.Add(item);
                                    }

                                    dang_tin.nganh_nghe = lst;
                                }

                                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng: JobStreet";
                                dang_tin.quy_mo_doanh_nghiep = lb_quymo.Text;
                                dang_tin.ten_cong_ty = lb_company_name.Text;
                                JobStreetPost job_street = new JobStreetPost();
                                var jobStreet = job_street.ExtractThongTin(conf,
                                    "https://employer.jobstreet.vn/vn/post-job",
                                    "xmedia.vn", out msg, dang_tin, is_debug);

                                check_status_post.Add("JobStreet", jobStreet);
                                return check_status_post;
                            });
                        }

                        if (check_top_cv.Checked)
                        {
                            var task_topcv = await System.Threading.Tasks.Task.Run(() =>
                            {
                                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng: TopCv";
                                var kvp = ReadAccountSite("TopCv");
                                var conf = ES.JobLinkRepository.Instance.GetCauHinh("topcv.vn");
                                conf.username = kvp.username;
                                conf.password = kvp.password;
                                conf.xpath_username = kvp.xpath_username;
                                conf.xpath_password = kvp.xpath_password;
                                dang_tin.yeu_cau_kinh_nghiem = lb_exp.Text;
                                var top_cv = new TopCvPost();
                                var topCv = top_cv.ExtractThongTin(conf,
                                    "https://tuyendung.topcv.vn/tin-tuyen-dung/tao-moi",
                                    "xmedia.vn", out msg, dang_tin, is_debug);

                                check_status_post.Add("TopCv", topCv);
                                return check_status_post;
                            });
                        }

                        if (check_career.Checked)
                        {
                            var task_check_career = await Task.Run(() =>
                            {
                                var kvp = ReadAccountSite("CareerLink");
                                var conf = ES.JobLinkRepository.Instance.GetCauHinh("careerlink.vn");
                                conf.username = kvp.username;
                                conf.password = kvp.password;
                                conf.xpath_username = kvp.xpath_username;
                                conf.xpath_password = kvp.xpath_password;
                                if (listBox4.Items.Count > 0)
                                {
                                    List<string> lst = new List<string>();
                                    foreach (string item in listBox4.Items)
                                    {
                                        lst.Add(item);
                                    }

                                    dang_tin.nganh_nghe = lst;
                                }

                                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng: CareerLink";
                                var career = new CareerLinkPost();
                                var careerLink = career.ExtractThongTin(conf,
                                    "https://www.careerlink.vn/nha-tuyen-dung/vieclam/moi", "xmedia.vn",
                                    dang_tin, out msg, is_debug);

                                check_status_post.Add("CareerLink", careerLink);
                                return check_status_post;
                            });
                        }

                        if (check_jobsgo.Checked)
                        {
                            var task_jobsgo = await Task.Run(() =>
                            {
                                var kvp = ReadAccountSite("JobsGo");
                                var conf = ES.JobLinkRepository.Instance.GetCauHinh("jobsgo.vn");
                                conf.username = kvp.username;
                                conf.password = kvp.password;
                                conf.xpath_username = kvp.xpath_username;
                                conf.xpath_password = kvp.xpath_password;
                                if (listBox5.Items.Count > 0)
                                {
                                    List<string> lst = new List<string>();
                                    foreach (string item in listBox5.Items)
                                    {
                                        lst.Add(item);
                                    }

                                    dang_tin.nganh_nghe = lst;
                                }

                                dang_tin.bang_cap = lb_bc.Text;
                                dang_tin.cap_bac = lb_vitri.Text;
                                dang_tin.kinh_nghiem_from = lb_exp_from.Text;
                                dang_tin.kinh_nghiem_to = lb_exp_to.Text;
                                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng: JobsGo";
                                var jobsgo = new JobsGoPost();
                                var jobsgo_post = jobsgo.ExtractThongTin(conf, "https://employer.jobsgo.vn/job/create",
                                    dang_tin, "xmedia.vn", out msg, is_debug);
                                check_status_post.Add("JobsGo", jobsgo_post);
                                return check_status_post;
                            });
                        }

                        if (check_career_builder.Checked)
                        {
                            var task_check_cr_builder = await System.Threading.Tasks.Task.Run(() =>
                            {
                                var kvp = ReadAccountSite("CareerBuilder");
                                var conf = ES.JobLinkRepository.Instance.GetCauHinh("careerbuilder.vn");
                                conf.username = kvp.username;
                                conf.password = kvp.password;
                                conf.xpath_username = kvp.xpath_username;
                                conf.xpath_password = kvp.xpath_password;
                                if (listBox6.Items.Count > 0)
                                {
                                    List<string> lst = new List<string>();
                                    foreach (string item in listBox6.Items)
                                    {
                                        lst.Add(item);
                                    }

                                    dang_tin.nganh_nghe = lst;
                                }

                                dang_tin.kinh_nghiem_from = lb_exp_from.Text;
                                dang_tin.kinh_nghiem_to = lb_exp_to.Text;
                                dang_tin.loai_hinh_cong_viec = lb_hinh_thuc.Text;
                                dang_tin.cap_bac = lb_cb_cb.Text;
                                dang_tin.yeu_cau_kinh_nghiem = lb_kn_cb.Text;
                                dang_tin.district = lb_district.Text;
                                dang_tin.dia_chi = lb_province_cb.Text;
                                tssLbMessage.Text = "Bắt đầu đẩy tin tuyển dụng: CareerBuilder";
                                var car_builder = new CareerBuilderPost();
                                var car_builder_post = car_builder.ExtractThongTin(conf,
                                    "https://careerbuilder.vn/vi/employers/postjobs", dang_tin,
                                    "xmedia.vn", out msg, is_debug);
                                check_status_post.Add("Career Builder", car_builder_post);
                                return check_status_post;
                            });
                        }

                        if (check_joboko.Checked)
                        {
                            var kvp = ReadAccountSite("Joboko");
                            var user = kvp.username;
                            var pass = kvp.password;

                            string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");
                            var jtoken = JObject.Parse(app_json);

                            if (jtoken["Joboko"] != null && jtoken["Joboko"]["Post"] != null)
                            {
                                var api_post = jtoken["Joboko"]["Post"].ToString();

                                dang_tin.nguoi_tao = user;
                                PostTin(dang_tin, user, pass, api_post);
                            }
                            else
                            {
                                MessageBox.Show("Không có đường dẫn API");
                            }
                        }

                        var sb = new StringBuilder();

                        foreach (var dic_check in check_status_post)
                        {
                            string stt = dic_check.Value ? "Thành công" : "Thất bại";
                            sb.AppendFormat("Đăng tin {0} - {1}\n", dic_check.Key, stt);
                        }

                        MessageBox.Show(sb.ToString());
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                tssLbMessage.Text = "Hoàn thành đẩy tin tuyển dụng";
                btn_post.Enabled = true;
            }
        }

        private int CountCheckBox()
        {
            int counter = 0;
            foreach (Control c in groupBox1.Controls)
                if (c is CheckBox && ((CheckBox)c).Checked)
                    counter++;
            return counter;
        }

        public static ListBox list_box2 = new ListBox();
        public static ListBox list_box3 = new ListBox();
        public static ListBox list_box4 = new ListBox();
        public static ListBox list_box5 = new ListBox();
        public static ListBox list_box6 = new ListBox();
        public static Label lb_tenct = new Label();

        private void check_job_street_CheckedChanged(object sender, EventArgs e)
        {
            form_JobStreet street = new form_JobStreet();

            var nganh_nghe = txtNganhNgheJob.Text;
            if (check_job_street.Checked)
            {
                if (Config.list_nganh_nghe_jobstreet.Contains(nganh_nghe)) return;
                street.ShowDialog();
                var item = form_JobStreet.check.CheckedItems;
                lb_quymo.Text = form_JobStreet.cb_quymo.Text;
                lb_company_name.Text = form_JobStreet.txt_tenct.Text;
                listBox2.Items.Clear();

                foreach (string text in item)
                {
                    listBox2.Items.Add(text);
                }

                if (listBox2.Items.Count > 0)
                {
                    check_job_street.BackColor = Color.Green;
                    check_job_street.ForeColor = Color.White;
                }
                else
                {
                    check_job_street.BackColor = Color.White;
                    check_job_street.ForeColor = Color.Black;
                    check_job_street.Checked = false;
                }
            }
        }

        private void check_career_CheckedChanged(object sender, EventArgs e)
        {
            form_CareerLink career = new form_CareerLink();
            var nganh_nghe = txtNganhNgheJob.Text;
            if (check_career.Checked)
            {
                if (Config.list_nganh_nghe_careerlink.Contains(nganh_nghe)) return;
                career.ShowDialog();
                var item = form_CareerLink.check.CheckedItems;
                listBox4.Items.Clear();
                foreach (string text in item)
                {
                    listBox4.Items.Add(text);
                }

                if (listBox4.Items.Count > 0)
                {
                    check_career.BackColor = Color.Green;
                    check_career.ForeColor = Color.White;
                }
                else
                {
                    check_career.BackColor = Color.White;
                    check_career.ForeColor = Color.Black;
                    check_career.Checked = false;
                }
            }
        }

        private void check_my_work_CheckedChanged(object sender, EventArgs e)
        {
            form_MyWork work = new form_MyWork();
            string nganh_nghe = txtNganhNgheJob.Text;
            if (check_my_work.Checked)
            {
                if (Config.list_nganh_nghe_my_work.Contains(nganh_nghe)) return;
                work.ShowDialog();
                var item = form_MyWork.check.CheckedItems;
                lb_exp.Text = form_MyWork.yc_kinh_nghiem.Text;
                lb_bang_cap.Text = form_MyWork.yc_bang_cap.Text;
                lb_gioi_tinh.Text = form_MyWork.gioi_tinh.Text;
                lb_goi_tin.Text = form_MyWork.goi_tin.Text;
                listBox3.Items.Clear();
                foreach (string text in item)
                {
                    listBox3.Items.Add(text);
                }

                if (listBox3.Items.Count > 0)
                {
                    check_my_work.BackColor = Color.Green;
                    check_my_work.ForeColor = Color.White;
                }
                else
                {
                    check_my_work.BackColor = Color.White;
                    check_my_work.ForeColor = Color.Black;
                    check_my_work.Checked = false;
                }
            }
        }

        private void check_jobsgo_CheckedChanged(object sender, EventArgs e)
        {
            var jobsgo = new form_Jobsgo();
            var nganh_nghe = txtNganhNgheJob.Text;
            if (check_jobsgo.Checked)
            {
                if (!Config.list_nganh_nghe_jobsgo.Contains(nganh_nghe))
                {
                    jobsgo.ShowDialog();
                    var item = form_Jobsgo.check.CheckedItems;
                    lb_bc.Text = form_Jobsgo.yc_bang_cap.Text;
                    lb_vitri.Text = form_Jobsgo.vi_tri.Text;
                    lb_exp_from.Text = form_Jobsgo.luong_from.Text;
                    lb_exp_to.Text = form_Jobsgo.luong_to.Text;
                    listBox5.Items.Clear();
                    foreach (string text in item)
                    {
                        foreach (var dic in Config.lst_jobsgo())
                        {
                            if (dic.Value == text)
                            {
                                listBox5.Items.Add(dic.Key);
                            }
                        }
                    }

                    if (listBox5.Items.Count > 0)
                    {
                        check_jobsgo.BackColor = Color.Green;
                        check_jobsgo.ForeColor = Color.White;
                    }
                    else
                    {
                        check_jobsgo.BackColor = Color.White;
                        check_jobsgo.ForeColor = Color.Black;
                        check_jobsgo.Checked = false;
                    }
                }
            }
        }

        private void check_career_builder_CheckedChanged(object sender, EventArgs e)
        {
            var career_builder = new form_CareerBuilder();
            var nganh_nghe = txtNganhNgheJob.Text;
            if (check_career_builder.Checked)
            {
                if (Config.list_nganh_nghe_careerbuilder.Contains(nganh_nghe)) return;
                career_builder.ShowDialog();
                var item = form_CareerBuilder.check.CheckedItems;
                lb_hinh_thuc.Text = form_CareerBuilder.cb_hinh_thuc.Text;
                lb_kn_cb.Text = form_CareerBuilder.cb_Kn.Text;
                lb_cb_cb.Text = form_CareerBuilder.cb_capBac.Text;
                lb_district.Text = form_CareerBuilder.district.Text;
                lb_province_cb.Text = form_CareerBuilder.province.Text;
                lb_exp_from.Text = form_CareerBuilder.luong_from.Text;
                lb_exp_to.Text = form_CareerBuilder.luong_to.Text;
                listBox6.Items.Clear();
                foreach (var text in item)
                {
                    listBox6.Items.Add(text);
                }

                if (listBox6.Items.Count > 0)
                {
                    check_career_builder.BackColor = Color.Green;
                    check_career_builder.ForeColor = Color.White;
                }
                else
                {
                    check_career_builder.BackColor = Color.White;
                    check_career_builder.ForeColor = Color.Black;
                    check_career_builder.Checked = false;
                }
            }
        }

        private void check_joboko_CheckedChanged(object sender, EventArgs e)
        {
            if (check_joboko.Checked)
            {
                check_joboko.BackColor = Color.Green;
                check_joboko.ForeColor = Color.White;
            }
            else
            {
                check_joboko.BackColor = Color.White;
                check_joboko.ForeColor = Color.Black;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btn_upload_file_Click_1(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.AddExtension = true;
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "All Files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pdfFiles = new List<string>();
                    foreach (string fileName in openFileDialog.FileNames)
                        pdfFiles.Add(fileName);
                }

                string installedPath = Application.StartupPath + "pdf";

                if (!Directory.Exists(installedPath))
                {
                    Directory.CreateDirectory(installedPath);
                }

                // check file upload exist

                foreach (string sourceFileName in pdfFiles)
                {
                    string destinationFileName = Path.Combine(installedPath, Path.GetFileName(sourceFileName));
                    if (File.Exists(destinationFileName))
                    {
                        File.Delete(destinationFileName);
                    }

                    label2.Text = destinationFileName;
                    File.Copy(sourceFileName, destinationFileName);
                    GetTextFromFile(destinationFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private bool ValidationFields()
        {
            if (txtChucDanhJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập chức danh");
                return false;
            }

            if (txtSoLuongJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập số lượng cần tuyển");
                return false;
            }

            if (rtbMoTaJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập mô tả công việc");
                return false;
            }

            if (rtbYeuCauJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập yêu cầu công việc");
                return false;
            }

            if (rtbThoiGianJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập thời gian làm việc");
                return false;
            }

            if (rtbQuyenLoiJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập quyền lợi ứng viên");
                return false;
            }

            if (rtbLienHeJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập liên hệ");
                return false;
            }

            if (txtSdtJob.Text.Length < 10 && txtSdtJob.Text.Length > 11)
            {
                MessageBox.Show("Số điện thoại phải nằm trong khoảng từ 9-11 ký tự");
                return false;
            }

            if (txtDiaChiJob.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập địa chỉ làm việc");
                return false;
            }

            if (!Config.list_luong.Contains(cbMucLuongJob.Text.Trim()))
            {
                MessageBox.Show("Bạn vui lòng chọn mức lương phù hợp");
                return false;
            }

            if (txt_email.Text.Length == 0)
            {
                MessageBox.Show("Bạn chưa nhập email liên hệ");
                return false;
            }

            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private bool SaveAccountConfig()
        {
            string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");
            bool success = false;
            var jtoken = JObject.Parse(app_json);
            bool is_debug = cboDebug.SelectedIndex == 0;
            try
            {
                if (!string.IsNullOrEmpty(jg_user.Text) && !string.IsNullOrEmpty(jg_pass.Text))
                {
                    using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
                    {
                        browser.GoTo("https://employer.jobsgo.vn/site/login");
                        System.Threading.Thread.Sleep(1000);
                        browser.FindAndClick(".//div[@id='modal']//button");
                        var login_form = browser.Find(".//div[@class='panel panel-body login-form']");
                        if (login_form.Count > 0)
                        {
                            foreach (var element in login_form)
                            {
                                element.FindElement(By.Id("loginform-user_name")).SendKeys(jg_user.Text);
                                element.FindElement(By.Id("loginform-password")).SendKeys(jg_pass.Text);
                                element.FindElement(By.XPath(".//button[@type='submit']")).Click();
                                System.Threading.Thread.Sleep(2000);
                            }
                            var url = browser.GetUrl();
                            success = url == "https://employer.jobsgo.vn/dashboard/index";
                        }
                        else
                        {
                            var find = browser.Find(".//li[@class='dropdown dropdown-user']");
                            if (find.Count > 0)
                            {
                                success = find.Count > 0;
                            }
                        }
                    }

                    if (success)
                    {
                        if (jtoken["JobsGo"] == null)
                        {
                            jtoken.Add("JobsGo",
                                JToken.Parse(
                                    $"{{\"username\":\"{jg_user.Text}\",\"password\":\"{XMedia.XUtil.Encode(jg_pass.Text)}\"}}"));
                        }
                        else
                        {
                            jtoken["JobsGo"] =
                                JToken.Parse(
                                    $"{{\"username\":\"{jg_user.Text}\",\"password\":\"{XMedia.XUtil.Encode(jg_pass.Text)}\"}}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lưu cấu hình JobsGo không thành công");
                    }
                }

                if (!string.IsNullOrEmpty(cb_user.Text) && !string.IsNullOrEmpty(cb_pass.Text))
                {
                    using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
                    {
                        browser.GoTo("https://careerbuilder.vn/vi/employers/login");
                        System.Threading.Thread.Sleep(1000);
                        var login_form = browser.Find(".//div[@id='Login_Employer']");
                        if (login_form.Count > 0)
                        {
                            foreach (var element in login_form)
                            {
                                element.FindElement(By.XPath("//form[@name='frmLogin']//input[@name='username']"))
                                    .SendKeys(cb_user.Text);
                                element.FindElement(By.XPath("//form[@name='frmLogin']//input[@name='password']"))
                                    .SendKeys(cb_pass.Text);
                                element.FindElement(
                                        By.XPath("//form[@name='frmLogin']/ul/li//span[@class='btn_submit']/input"))
                                    .Click();
                                System.Threading.Thread.Sleep(2000);
                            }

                            var url = browser.GetUrl();
                            success = url == "https://careerbuilder.vn/vi/employers/hrcentral";
                        }
                        else
                        {
                            var find = browser.Find("//div[@class='kv_login']");
                            if (find.Count > 0)
                            {
                                success = find.Count > 0;
                            }
                        }
                    }

                    if (success)
                    {
                        if (jtoken["CareerBuilder"] == null)
                        {
                            jtoken.Add("CareerBuilder",
                                JToken.Parse(
                                    $"{{\"username\":\"{cb_user.Text}\",\"password\":\"{XMedia.XUtil.Encode(cb_pass.Text)}\"}}"));
                        }
                        else
                        {
                            jtoken["CareerBuilder"] =
                                JToken.Parse(
                                    $"{{\"username\":\"{cb_user.Text}\",\"password\":\"{XMedia.XUtil.Encode(cb_pass.Text)}\"}}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Lưu cấu hình CareerBuilder không thành công");
                    }
                }

                if (!string.IsNullOrEmpty(joboko_user.Text) && !string.IsNullOrEmpty(joboko_pass.Text))
                //&& !string.IsNullOrEmpty(txtJobokoPostAPI.Text) && !string.IsNullOrEmpty(txtJobokoGetAllAPI.Text))
                {
                    if (jtoken["Joboko"] == null)
                    {
                        jtoken.Add("Joboko",
                            JToken.Parse(
                                $"{{\"username\":\"{joboko_user.Text}\"," +
                                $"\"password\":\"{XMedia.XUtil.Encode(joboko_pass.Text)}\"," +
                                $"\"Post\":\"{txtJobokoPostAPI.Text}\"," +
                                $"\"GetAll\":\"{txtJobokoGetAllAPI.Text}\"," +
                                $"\"Update\":\"{txtJobokoUpdateAPI.Text}\"," +
                                $"\"GetCvByIdTin\":\"{txtJobokoGetCvAPI.Text}\"}}"));
                    }
                    else
                    {
                        jtoken["Joboko"] = JToken.Parse(
                                $"{{\"username\":\"{joboko_user.Text}\"," +
                                $"\"password\":\"{XMedia.XUtil.Encode(joboko_pass.Text)}\"," +
                                $"\"Post\":\"{txtJobokoPostAPI.Text}\"," +
                                $"\"GetAll\":\"{txtJobokoGetAllAPI.Text}\"," +
                                $"\"Update\":\"{txtJobokoUpdateAPI.Text}\"," +
                                $"\"GetCvByIdTin\":\"{txtJobokoGetCvAPI.Text}\"}}");
                    }
                }

                File.WriteAllText($"{curent_exe_path}\\appsettings.json", jtoken.ToString());
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return success;
        }

        private void LoadAccountConfig()
        {
            string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");

            var jtoken = JObject.Parse(app_json);
            try
            {
                if (jtoken["JobStreet"]?["username"] != null)
                {
                    js_user.Text = jtoken["JobStreet"]["username"].ToString();
                    if (jtoken["JobStreet"]["password"] != null)
                        js_pass.Text = XMedia.XUtil.DecodeToken(jtoken["JobStreet"]["password"].ToString());
                    js_xpath_user.Text = jtoken["JobStreet"]["xpath_user"].ToString();
                    js_xpath_pass.Text = jtoken["JobStreet"]["xpath_pass"].ToString();
                }

                if (jtoken["MyWork"]?["username"] != null)
                {
                    mw_user.Text = jtoken["MyWork"]["username"].ToString();
                    if (jtoken["MyWork"]["password"] != null)
                        mw_pass.Text = XMedia.XUtil.DecodeToken(jtoken["MyWork"]["password"].ToString());

                    mw_xpath_user.Text = jtoken["MyWork"]["xpath_user"].ToString();
                    mw_xpath_pass.Text = jtoken["MyWork"]["xpath_pass"].ToString();
                }

                if (jtoken["TopCv"]?["username"] != null)
                {
                    top_user.Text = jtoken["TopCv"]["username"].ToString();
                    if (jtoken["TopCv"]["password"] != null)
                        top_pass.Text = XMedia.XUtil.DecodeToken(jtoken["TopCv"]["password"].ToString());

                    top_xpath_user.Text = jtoken["TopCv"]["xpath_user"].ToString();
                    top_xpath_pass.Text = jtoken["TopCv"]["xpath_pass"].ToString();
                }

                if (jtoken["CareerLink"]?["username"] != null)
                {
                    cl_user.Text = jtoken["CareerLink"]["username"].ToString();
                    if (jtoken["CareerLink"]["password"] != null)
                        cl_pass.Text = XMedia.XUtil.DecodeToken(jtoken["CareerLink"]["password"].ToString());
                    cl_xpath_user.Text = jtoken["CareerLink"]["xpath_user"].ToString();
                    cl_xpath_pass.Text = jtoken["CareerLink"]["xpath_pass"].ToString();
                }

                if (jtoken["JobsGo"]?["username"] != null)
                {
                    jg_user.Text = jtoken["JobsGo"]["username"].ToString();
                    if (jtoken["JobsGo"]["password"] != null)
                        jg_pass.Text = XMedia.XUtil.DecodeToken(jtoken["JobsGo"]["password"].ToString());
                    jg_xpath_user.Text = jtoken["JobsGo"]["xpath_user"].ToString();
                    jg_xpath_pass.Text = jtoken["JobsGo"]["xpath_pass"].ToString();
                }

                if (jtoken["CareerBuilder"]?["username"] != null)
                {
                    cb_user.Text = jtoken["CareerBuilder"]["username"].ToString();
                    if (jtoken["CareerBuilder"]["password"] != null)
                        cb_pass.Text = XMedia.XUtil.DecodeToken(jtoken["CareerBuilder"]["password"].ToString());

                    cb_xpath_user.Text = jtoken["CareerBuilder"]["xpath_user"].ToString();
                    cb_xpath_pass.Text = jtoken["CareerBuilder"]["xpath_pass"].ToString();
                }

                if (jtoken["Joboko"]?["username"] != null)
                {
                    joboko_user.Text = jtoken["Joboko"]["username"].ToString();
                    if (jtoken["Joboko"]["password"] != null)
                        joboko_pass.Text = XMedia.XUtil.DecodeToken(jtoken["Joboko"]["password"].ToString());

                    txtJobokoPostAPI.Text = jtoken["Joboko"]["Post"].ToString();
                    txtJobokoGetAllAPI.Text = jtoken["Joboko"]["GetAll"].ToString();
                    txtJobokoGetCvAPI.Text = jtoken["Joboko"]["GetCvByIdTin"].ToString();
                    txtJobokoUpdateAPI.Text = jtoken["Joboko"]["Update"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="site">JobStreet|MyWork|TopCv|CareerLink</param>
        /// <returns></returns>
        ///
        private CauHinh ReadAccountSite(string site)
        {
            var ch = new CauHinh();
            try
            {
                //string user_name = "", password = "", xpath_user = "", xpath_pass = "";

                string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");
                var jtoken = JObject.Parse(app_json);
                switch (site)
                {
                    case "JobStreet":
                        if (jtoken["JobStreet"] != null && jtoken["JobStreet"]["username"] != null &&
                            jtoken["JobStreet"]["password"] != null)
                        {
                            ch.username = jtoken["JobStreet"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["JobStreet"]["password"].ToString());
                            ch.xpath_username = jtoken["JobStreet"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["JobStreet"]["xpath_pass"].ToString();
                        }

                        break;

                    case "MyWork":
                        if (jtoken["MyWork"] != null && jtoken["MyWork"]["username"] != null &&
                            jtoken["MyWork"]["password"] != null)
                        {
                            ch.username = jtoken["MyWork"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["MyWork"]["password"].ToString());
                            ch.xpath_username = jtoken["MyWork"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["MyWork"]["xpath_pass"].ToString();
                        }

                        break;

                    case "TopCv":
                        if (jtoken["TopCv"] != null && jtoken["TopCv"]["username"] != null &&
                            jtoken["TopCv"]["password"] != null)
                        {
                            ch.username = jtoken["TopCv"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["TopCv"]["password"].ToString());
                            ch.xpath_username = jtoken["TopCv"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["TopCv"]["xpath_pass"].ToString();
                        }

                        break;

                    case "CareerLink":
                        if (jtoken["CareerLink"] != null && jtoken["CareerLink"]["username"] != null &&
                            jtoken["CareerLink"]["password"] != null)
                        {
                            ch.username = jtoken["CareerLink"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["CareerLink"]["password"].ToString());
                            ch.xpath_username = jtoken["CareerLink"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["CareerLink"]["xpath_pass"].ToString();
                        }

                        break;

                    case "JobsGo":
                        if (jtoken["JobsGo"] != null && jtoken["JobsGo"]["username"] != null &&
                            jtoken["JobsGo"]["password"] != null)
                        {
                            ch.username = jtoken["JobsGo"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["JobsGo"]["password"].ToString());
                            ch.xpath_username = jtoken["JobsGo"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["JobsGo"]["xpath_pass"].ToString();
                        }

                        break;

                    case "CareerBuilder":
                        if (jtoken["CareerBuilder"] != null && jtoken["CareerBuilder"]["username"] != null &&
                            jtoken["CareerBuilder"]["password"] != null)
                        {
                            ch.username = jtoken["CareerBuilder"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["CareerBuilder"]["password"].ToString());
                            ch.xpath_username = jtoken["CareerBuilder"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["CareerBuilder"]["xpath_pass"].ToString();
                        }

                        break;

                    case "Joboko":
                        if (jtoken["Joboko"] != null && jtoken["Joboko"]["username"] != null &&
                            jtoken["Joboko"]["password"] != null)
                        {
                            ch.username = jtoken["Joboko"]["username"].ToString();
                            ch.password = XMedia.XUtil.DecodeToken(jtoken["Joboko"]["password"].ToString());
                            ch.xpath_username = jtoken["Joboko"]["xpath_user"].ToString();
                            ch.xpath_password = jtoken["Joboko"]["xpath_pass"].ToString();
                        }

                        break;
                }

                //return new KeyValuePair<string, string>(user_name, password);
                return ch;
            }
            catch (Exception)
            {
            }

            return ch;
        }

        private void GetCVJobStreet()
        {
            var kvp = ReadAccountSite("JobStreet");
            if (!string.IsNullOrEmpty(kvp.username) && !string.IsNullOrEmpty(kvp.password))
            {
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("jobstreet.vn");

                conf.username = kvp.username;
                conf.password = kvp.password;
                conf.xpath_username = kvp.xpath_username;
                conf.xpath_password = kvp.xpath_password;
                JobStreetHelper.JobStreet job_street = new JobStreetHelper.JobStreet(user_profile_path,
                    $"{folder_cv}\\jobstreet.vn");

                string msg;
                var lst_tin_tuyen_dung =
                    job_street.ExtractJobLink(conf, "https://employer.jobstreet.vn/vn/home", "xmedia.vn", out msg);

                var list_job_link = new List<JobLink>();
                foreach (var tin in lst_tin_tuyen_dung)
                {
                    if (!ES.JobLinkRepository.Instance.IsExistJobLink("xmedia.vn", tin.link))
                    {
                        list_job_link.Add(tin);
                    }
                }

                if (list_job_link.Count > 0)
                {
                    ES.JobLinkRepository.Instance.IndexMany(list_job_link);
                }

                tssLbMessage.Text = $"Hoàn thành lấy tin tuyển dụng: {lst_tin_tuyen_dung.Count}";
                //lst_tin_tuyen_dung = ES.JobLinkRepository.Instance.GetJobChuaXuLy("employer.jobstreet.vn",100, false);

                var lst_ung_vien = job_street.Run(conf, lst_tin_tuyen_dung);

                var list_ung_vien_must = new List<UngVien>();
                foreach (var uv in lst_ung_vien)
                {
                    if (!ES.UngVienRepository.Instance.IsExistUngVien("xmedia.vn", uv.so_dien_thoai))
                    {
                        list_ung_vien_must.Add(uv);
                    }
                }

                if (list_ung_vien_must.Count > 0)
                {
                    ES.UngVienRepository.Instance.IndexMany(list_ung_vien_must);
                }
            }
        }

        private void GetCVMyWork()
        {
            var kvp = ReadAccountSite("MyWork");
            if (!string.IsNullOrEmpty(kvp.username) && !string.IsNullOrEmpty(kvp.password))
            {
                string msg = "";
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("mywork.com.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;
                conf.xpath_username = kvp.xpath_username;
                conf.xpath_password = kvp.xpath_password;
                MyWorkHelper.MyWork my_work = new MyWorkHelper.MyWork(user_profile_path,
                    $"{folder_cv}\\mywork.vn");

                var lst_tin_tuyen_dung = my_work.ExtractJobLink(conf, "https://mywork.com.vn/nha-tuyen-dung/quan-ly-tin-dang?",
                    "xmedia.vn", out msg);
                var list_job_link = new List<JobLink>();
                foreach (var tin in lst_tin_tuyen_dung)
                {
                    if (!ES.JobLinkRepository.Instance.IsExistJobLink("xmedia.vn", tin.link))
                    {
                        list_job_link.Add(tin);
                    }
                }

                if (list_job_link.Count > 0)
                {
                    ES.JobLinkRepository.Instance.IndexMany(list_job_link);
                }

                tssLbMessage.Text = $"Hoàn thành lấy tin tuyển dụng: {lst_tin_tuyen_dung.Count}";
                //lst_tin_tuyen_dung = ES.JobLinkRepository.Instance.GetJobChuaXuLy("mywork.com.vn", 5, true);

                var lst_ung_vien = my_work.Run(conf, lst_tin_tuyen_dung);
                var list_ung_vien_must = new List<UngVien>();
                foreach (var uv in lst_ung_vien)
                {
                    if (!ES.UngVienRepository.Instance.IsExistUngVien("xmedia.vn", uv.so_dien_thoai))
                    {
                        list_ung_vien_must.Add(uv);
                    }
                }

                if (list_ung_vien_must.Count > 0)
                {
                    ES.UngVienRepository.Instance.IndexMany(list_ung_vien_must);
                }
            }
        }

        private void GetDataCV()
        {
            long total_rec = 1;
            int page_size = 8;

            string term = txtTerm.Text;
            var ngay_tao_tu = XMedia.XUtil.TimeInEpoch(dtTaoTu.Value);
            var ngay_tao_den = XMedia.XUtil.TimeInEpoch(dtTaoDen.Value);

            var lst_data = ES.UngVienRepository.Instance.GetByNguoiTao(term, ngay_tao_tu, ngay_tao_den, page, page_size,
                new string[] { }, out total_rec);
            grvData.DataSource = lst_data.Select(x => new
            {
                ho_ten = x.ho_ten,
                lien_lac = $"{x.email} / {x.so_dien_thoai}",
                dia_chi = x.dia_chi,
                hoc_van = x.hoc_van,
                kinh_nghiem = x.kinh_nghiem,
                cv = x.link_cv_offline,
                gioi_tinh = x.gioi_tinh,
                job = x.job_link,
                vi_tri = x.vi_tri,
                site = x.domain,
                ngay_tao = XMedia.XUtil.EpochToTime(x.ngay_tao).ToString("dd-MM-yyyy")
            }).ToList();
            //grvData.Columns[0].DataPropertyName = "ho_ten";
            //grvData.Columns[1].DataPropertyName = "lien_lac";
            //grvData.Columns[2].DataPropertyName = "dia_chi";
            //grvData.Columns[4].DataPropertyName = "hoc_van";
            //grvData.Columns[5].DataPropertyName = "kinh_nghiem";
            //grvData.Columns[6].DataPropertyName = "cv";
            //grvData.Columns[7].DataPropertyName = "gioi_tinh";
            //grvData.Columns[8].DataPropertyName = "job";
            //grvData.Columns[9].DataPropertyName = "vi_tri";
            //grvData.Columns[10].DataPropertyName = "site";
            //grvData.Columns[11].DataPropertyName = "ngay_tao";
            _ucPaging.Total_recs = total_rec;
            _ucPaging.Page = page;
            _ucPaging.Page_size = page_size;
            _ucPaging.DisplayPaging();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchCv();
        }

        private void SearchCv()
        {
            try
            {
                Dictionary<string, string> sort = new Dictionary<string, string>();
                sort.Add(((KeyValuePair<string, string>)cboSortFieldCV.SelectedItem).Key,
                    ((KeyValuePair<string, string>)cboSortOrderCV.SelectedItem).Key);
                int page_size = 20;
                string term = txtTerm.Text;
                long ngay_tao_tu = 0;
                long ngay_tao_den = 0;
                if (ck_ngayTao.Checked)
                {
                    ngay_tao_tu = XMedia.XUtil.TimeInEpoch(dtTaoTu.Value.Date);
                    ngay_tao_den = XMedia.XUtil.TimeInEpoch(dtTaoDen.Value.Date.AddDays(1).AddSeconds(-1));
                }

                string site_name = ((KeyValuePair<string, string>)cbSiteNameUv.SelectedItem).Key;
                if (site_name == "-1")
                    site_name = string.Empty;

                var vi_tri = txtViTriUngTuyen.Text;
                var hoc_van = txtHocVan.Text;
                var dia_chi = txtDiaChi.Text;
                var gioi_tinh = ((KeyValuePair<string, string>)cbGioiTinh.SelectedItem).Key;

                if (gioi_tinh == "0")
                    gioi_tinh = string.Empty;

                var list_account = GetListAccountConfig();
                var data = ES.UngVienRepository.Instance.Search("xmedia.vn", term, list_account, ngay_tao_tu,
                    ngay_tao_den,
                    site_name, vi_tri, hoc_van, dia_chi, gioi_tinh, page, out var total_rec, page_size, sort);
                grvData.DataSource = data.Select(x => new
                {
                    ho_ten = x.ho_ten.ToUpper(),
                    lien_lac = $"{x.email}",
                    dia_chi = x.dia_chi,
                    hoc_van = x.hoc_van,
                    so_dien_thoai = x.so_dien_thoai,
                    cv = x.link_cv_offline,
                    cv_online = x.link_cv_online,
                    gioi_tinh = x.gioi_tinh,
                    job = x.job_link,
                    vi_tri = x.vi_tri,
                    site = x.domain,
                    ngay_tao = XMedia.XUtil.EpochToTime(x.ngay_tao).ToString("dd-MM-yyyy"),
                    nguoi_tao = x.nguoi_tao
                }).ToList();

                _ucPaging.Total_recs = total_rec;
                _ucPaging.Page = page;
                _ucPaging.Page_size = page_size;
                _ucPaging.DisplayPaging();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void _ucPaging_PrevClick(object sender, EventArgs e)
        {
            page -= 1;
            SearchCv();
        }

        private void _ucPaging_NextClick(object sender, EventArgs e)
        {
            page += 1;
            SearchCv();
        }

        private void _UcPagingJob_NextClick(object sender, EventArgs e)
        {
            page += 1;
            GetDataJobLink();
        }

        private void _UcPagingJob_PrevClick(object sender, EventArgs e)
        {
            page -= 1;
            GetDataJobLink();
        }

        private List<string> GetListAccountConfig()
        {
            string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");
            var jtoken = JObject.Parse(app_json);
            var list_account = new List<string>();
            if (jtoken["JobStreet"] != null && jtoken["JobStreet"]["username"] != null)
            {
                list_account.Add(jtoken["JobStreet"]["username"].ToString());
            }

            if (jtoken["MyWork"] != null && jtoken["MyWork"]["username"] != null)
            {
                list_account.Add(jtoken["MyWork"]["username"].ToString());
            }

            if (jtoken["TopCv"] != null && jtoken["TopCv"]["username"] != null)
            {
                list_account.Add(jtoken["TopCv"]["username"].ToString());
            }

            if (jtoken["CareerLink"] != null && jtoken["CareerLink"]["username"] != null)
            {
                list_account.Add(jtoken["CareerLink"]["username"].ToString());
            }

            if (jtoken["JobsGo"] != null && jtoken["JobsGo"]["username"] != null)
            {
                list_account.Add(jtoken["JobsGo"]["username"].ToString());
            }

            if (jtoken["CareerBuilder"] != null && jtoken["CareerBuilder"]["username"] != null)
            {
                list_account.Add(jtoken["CareerBuilder"]["username"].ToString());
            }

            if (jtoken["Joboko"] != null && jtoken["Joboko"]["username"] != null)
            {
                list_account.Add(jtoken["Joboko"]["username"].ToString());
            }

            return list_account.Distinct().ToList();
        }

        private void grvJobLink_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mnuJob.Show(grvJobLink, e.Location);
            }
        }

        private void grvJobLink_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    GetDataJobLink();
                    break;
            }
        }

        private void XuLyCvSingle(string account_site, List<JobLink> lst)
        {
            Dictionary<string, int> total_result = new Dictionary<string, int>();
            if (account_site == "JobStreet")
            {
                var kvp = ReadAccountSite("JobStreet");

                var conf = ES.JobLinkRepository.Instance.GetCauHinh("jobstreet.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;
                JobStreetHelper.JobStreet job_street = new JobStreetHelper.JobStreet(user_profile_path + account_site,
                    $"{Path.GetDirectoryName(Application.ExecutablePath)}\\profiles\\downloads\\jobstreet.vn");
                var lst_ung_vien = job_street.Run(conf, lst);
                int total = ES.UngVienRepository.Instance.IndexMany(lst_ung_vien);
                if (total > 0)
                {
                    total_result.Add("JobStreet", total);
                }
            }
            else if (account_site == "MyWork")
            {
                var kvp = ReadAccountSite("MyWork");
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("mywork.com.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;
                MyWorkHelper.MyWork my_work = new MyWorkHelper.MyWork(user_profile_path + account_site,
                    $"{Path.GetDirectoryName(Application.ExecutablePath)}\\profiles\\downloads\\mywork.com.vn");
                var lst_ung_vien = my_work.Run(conf, lst);
                int total = ES.UngVienRepository.Instance.IndexMany(lst_ung_vien);
                if (total > 0)
                {
                    total_result.Add("MyWork", total);
                }
            }
            else if (account_site == "TopCv")
            {
                var kvp = ReadAccountSite("TopCv");
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("topcv.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;

                TopCV top_cv = new TopCV(user_profile_path + account_site, $"{folder_cv}\\topcv.vn");
                var lst_ung_vien = top_cv.Run(conf, lst);
                int total = ES.UngVienRepository.Instance.IndexMany(lst_ung_vien);
                if (total > 0)
                {
                    total_result.Add("TopCv", total);
                }
            }
            else if (account_site == "CareerLink")
            {
                var kvp = ReadAccountSite("CareerLink");
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("careerlink.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;

                CareerLinkHelper.CareerLink career_link = new CareerLinkHelper.CareerLink(user_profile_path + account_site,
                    $"{Path.GetDirectoryName(Application.ExecutablePath)}\\profiles\\downloads\\careerlink.vn");
                var lst_ung_vien = career_link.Run(conf, lst);
                int total = ES.UngVienRepository.Instance.IndexMany(lst_ung_vien);
                if (total > 0)
                {
                    total_result.Add("CareerLink", total);
                }
            }
            else if (account_site == "JobsGo")
            {
                var kvp = ReadAccountSite("JobsGo");
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("jobsgo.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;

                JobsGoHelper.JobsGo jobsgo = new JobsGoHelper.JobsGo(user_profile_path + account_site,
                    $"{Path.GetDirectoryName(Application.ExecutablePath)}\\profiles\\downloads\\jobsgo.vn");
                var lst_ung_vien = jobsgo.Run(conf, lst);
                int total = ES.UngVienRepository.Instance.IndexMany(lst_ung_vien);
                if (total > 0)
                {
                    total_result.Add("JobsGo", total);
                }
            }
            else if (account_site == "CareerBuilder")
            {
                var kvp = ReadAccountSite("CareerBuilder");
                var conf = ES.JobLinkRepository.Instance.GetCauHinh("careerbuilder.vn");
                conf.username = kvp.username;
                conf.password = kvp.password;

                CareerBuilder career_builder = new CareerBuilderHelper.CareerBuilder(
                    user_profile_path + account_site,
                    $"{Path.GetDirectoryName(Application.ExecutablePath)}\\profiles\\downloads\\careerbuilder.vn");
                var lst_ung_vien = career_builder.Run(conf, lst, new ToolStripStatusLabel());
                int total = UngVienRepository.Instance.IndexMany(lst_ung_vien);
                if (total > 0)
                {
                    total_result.Add("CareerBuilder", total);
                }
            }
            else if (account_site == "Joboko")
            {
                // gọi vào api để lấy cv trong tin.
                var list_id = new List<string>();
                foreach (var itemLink in lst)
                {
                    list_id.Add(itemLink.link);
                }

                string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");
                var jtoken = JObject.Parse(app_json);

                if (jtoken["Joboko"] != null && jtoken["Joboko"]["GetCvByIdTin"] != null)
                {
                    var api_getcv = jtoken["Joboko"]["GetCvByIdTin"].ToString();
                    var cv_ung_vien = LayCvUngVienTinDaDang(list_id, api_getcv);
                    int total = UngVienRepository.Instance.IndexMany(cv_ung_vien);
                    if (total > 0)
                    {
                        total_result.Add("Joboko", total);
                    }
                }
            }

            var sb = new StringBuilder();

            foreach (var total in total_result)
            {
                sb.AppendFormat("Tổng số CV lấy được: {0} - {1}\n", total.Key, total.Value);
            }

            tssLbMessage.Text = sb.ToString();
        }

        private void btn_TimJob_Click(object sender, EventArgs e)
        {
            page = 1;
            GetDataJobLink();
        }

        public void GetDataJobLink()
        {
            Dictionary<string, string> sort = new Dictionary<string, string>();

            sort.Add(((KeyValuePair<string, string>)cboSortFieldJobLink.SelectedItem).Key,
                ((KeyValuePair<string, string>)cboSortOrderJobLink.SelectedItem).Key);

            int page_size = 20;
            string term = txt_search.Text;
            long ngay_tao_tu = 0, ngay_tao_den = 0, ngay_xu_ly_tu = 0, ngay_xu_ly_den = 0;

            if (chkCreated.Checked)
            {
                ngay_tao_tu = XMedia.XUtil.TimeInEpoch(dtFromJobLink.Value.Date);
                ngay_tao_den = XMedia.XUtil.TimeInEpoch(dtToJobLink.Value.Date.AddDays(1).AddSeconds(-1));
            }

            if (chkNgayXuLy.Checked)
            {
                ngay_xu_ly_tu = XMedia.XUtil.TimeInEpoch(dtNgayXuLyTu.Value.Date);
                ngay_xu_ly_den = XMedia.XUtil.TimeInEpoch(dtNgayXuLyDen.Value.Date.AddDays(1).AddSeconds(-1));
            }

            string trang_thai_xu_ly = ((KeyValuePair<string, string>)cbProcessStatus.SelectedItem).Key;
            if (trang_thai_xu_ly == "-1")
                trang_thai_xu_ly = string.Empty;

            string site_name = ((KeyValuePair<string, string>)cbSiteName.SelectedItem).Key;
            if (site_name == "-1")
                site_name = string.Empty;

            var list_account = GetListAccountConfig();
            var data = ES.JobLinkRepository.Instance.Search("xmedia.vn", term, list_account, ngay_tao_tu, ngay_tao_den,
                ngay_xu_ly_tu, ngay_xu_ly_den,
                trang_thai_xu_ly, site_name, page, out var total_rec, page_size, sort);
            grvJobLink.DataSource = data.Select(x => new
            {
                ten_job = x.ten_job,
                link_job = x.link,
                trang_thai_xu_ly = x.trang_thai_xu_ly,
                tong_so_cv = x.tong_so_cv,
                ngay_xu_ly = x.ngay_xu_ly > 0 ? XMedia.XUtil.EpochToTimeString(x.ngay_xu_ly) : "",
                ngay_tao = XMedia.XUtil.EpochToTime(x.ngay_tao).ToString("dd-MM-yyyy"),
                nguoi_tao_link = x.nguoi_tao
            }).ToList();

            ucPagingJoblink1.Total_recs = total_rec;
            ucPagingJoblink1.Page = page;
            ucPagingJoblink1.Page_size = page_size;
            ucPagingJoblink1.DisplayPaging();
        }

        private void chkCreated_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(chkCreated, grpCreated);
        }

        private void ck_ngayTao_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(ck_ngayTao, grpCvCreate);
        }

        protected void ManageCheckGroupBox(CheckBox chk, GroupBox grp)
        {
            // Make sure the CheckBox isn't in the GroupBox.
            // This will only happen the first time.
            if (chk.Parent == grp)
            {
                // Reparent the CheckBox so it's not in the GroupBox.
                grp.Parent.Controls.Add(chk);

                // Adjust the CheckBox's location.
                chk.Location = new Point(
                    chk.Left + grp.Left,
                    chk.Top + grp.Top);

                // Move the CheckBox to the top of the stacking order.
                chk.BringToFront();
            }

            // Enable or disable the GroupBox.
            grp.Enabled = chk.Checked;
            foreach (Control item in grp.Controls)
            {
                if (item.GetType() == typeof(DateTimePicker))
                {
                    var chkx = (DateTimePicker)item;
                    chkx.Enabled = grp.Enabled;
                }
            }
        }

        private void chkNgayXuLy_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(chkNgayXuLy, grpNgayXuLy);
        }

        private void btnJobstreet_Click(object sender, EventArgs e)
        {
            GetCVJobStreet();
        }

        private void btnMyWork_Click(object sender, EventArgs e)
        {
            GetCVMyWork();
        }

        private void btnTopCv_Click(object sender, EventArgs e)
        {
            var conf = ES.JobLinkRepository.Instance.GetCauHinh("topcv.vn");
            var kvp = ReadAccountSite("TopCv");
            conf.username = kvp.username;
            conf.password = kvp.password;
            conf.xpath_username = kvp.xpath_username;
            conf.xpath_password = kvp.xpath_password;
            TopCV top_cv = new TopCV(user_profile_path,
                $"{folder_cv}\\topcv.vn");

            var lst_tin_tuyen_dung =
                top_cv.ExtractJobLink(conf, "https://tuyendung.topcv.vn/tin-tuyen-dung", "xmedia.vn", out string msg);
            if (lst_tin_tuyen_dung.Count > 0)
            {
                var list_job_link = new List<JobLink>();
                foreach (var tin in lst_tin_tuyen_dung)
                {
                    if (!JobLinkRepository.Instance.IsExistJobLink("xmedia.vn", tin.link))
                    {
                        list_job_link.Add(tin);
                    }
                }

                if (list_job_link.Count > 0)
                {
                    ES.JobLinkRepository.Instance.IndexMany(list_job_link);
                }

                tssLbMessage.Text = $"Hoàn thành lấy tin tuyển dụng: {lst_tin_tuyen_dung.Count}";

                var lst_ung_vien = top_cv.Run(conf, lst_tin_tuyen_dung);
                var list_ung_vien_must = new List<UngVien>();
                foreach (var uv in lst_ung_vien)
                {
                    if (!ES.UngVienRepository.Instance.IsExistUngVien("xmedia.vn", uv.so_dien_thoai))
                    {
                        list_ung_vien_must.Add(uv);
                    }
                }

                if (list_ung_vien_must.Count > 0)
                {
                    ES.UngVienRepository.Instance.IndexMany(list_ung_vien_must);
                }
            }
            else
            {
                MessageBox.Show("Không có tin tuyển dụng nào trên TopCV đang được đăng trực tuyến");
            }
        }

        private async void btnCareer_Click(object sender, EventArgs e)
        {
            string msg = "";
            btnCareer.Text = $"{btnCareer.Text}...";
            btnCareer.Enabled = false;
            CareerLinkHelper.CareerLink career_link = new CareerLinkHelper.CareerLink(user_profile_path,
                $"{folder_cv}\\careerlink.vn");
            var conf = ES.JobLinkRepository.Instance.GetCauHinh("careerlink.vn");
            var kvp = ReadAccountSite("CareerLink");
            conf.username = kvp.username;
            conf.password = kvp.password;
            conf.xpath_username = kvp.xpath_username;
            conf.xpath_password = kvp.xpath_password;
            tssLbMessage.Text = "Đang lấy toàn bộ Job link tại: careerlink.vn";
            var task = await System.Threading.Tasks.Task.Run(() =>
            {
                var lst_tin_tuyen_dung = career_link.ExtractJobLink(conf,
                    "https://www.careerlink.vn/nha-tuyen-dung/vieclam", "xmedia.vn", out msg);
                var list_job_link = new List<JobLink>();
                foreach (var tin in lst_tin_tuyen_dung)
                {
                    if (!ES.JobLinkRepository.Instance.IsExistJobLink("xmedia.vn", tin.link))
                    {
                        list_job_link.Add(tin);
                    }
                }

                if (list_job_link.Count > 0)
                {
                    ES.JobLinkRepository.Instance.IndexMany(list_job_link);
                }

                tssLbMessage.Text = $"Hoàn thành lấy tin tuyển dụng: {lst_tin_tuyen_dung.Count}";
                return lst_tin_tuyen_dung;
            });
            tssLbMessage.Text = $"Đang thực hiện lấy CV: careerlink.vn";
            var task_ung_vien = await System.Threading.Tasks.Task.Run(() =>
            {
                var lst_ung_vien = career_link.Run(conf, task, tssLbMessage);
                var list_ung_vien_must = new List<UngVien>();
                foreach (var uv in lst_ung_vien)
                {
                    if (!ES.UngVienRepository.Instance.IsExistUngVien("xmedia.vn", uv.so_dien_thoai))
                    {
                        list_ung_vien_must.Add(uv);
                    }
                }

                int ungvien = 0;
                if (list_ung_vien_must.Count > 0)
                {
                    ungvien = ES.UngVienRepository.Instance.IndexMany(list_ung_vien_must);
                }

                return ungvien;
            });
            tssLbMessage.Text = $"Hoàn thành get full cv ứng viên: {task_ung_vien}";
            btnCareer.Enabled = true;
            btnCareer.Text = $@"{btnCareer.Text.Replace("...", "")}";
        }

        private async void btnJobsGo_Click(object sender, EventArgs e)
        {
            btnJobsGo.Enabled = false;
            var conf = ES.JobLinkRepository.Instance.GetCauHinh("jobsgo.vn");
            var kvp = ReadAccountSite("JobsGo");
            conf.username = kvp.username;
            conf.password = kvp.password;
            conf.xpath_username = kvp.xpath_username;
            conf.xpath_password = kvp.xpath_password;
            var lst_link = new List<string>()
            {
                "https://employer.jobsgo.vn/job/active", "https://employer.jobsgo.vn/job/pause",
                "https://employer.jobsgo.vn/job/expire", "https://employer.jobsgo.vn/job/inactive"
            };
            JobsGo jobsgo = new JobsGo(user_profile_path,
                $"{folder_cv}\\jobsgo.vn");

            string msg;
            tssLbMessage.Text = $"Bắt đầu lấy tin tuyển dụng: JobsGo";
            var task_job_link = await System.Threading.Tasks.Task.Run(() =>
            {
                var lst_tin_tuyen_dung = jobsgo.ExtractJobLink(conf, lst_link, "xmedia.vn", out msg);
                return lst_tin_tuyen_dung;
            });
            var list_job_link = new List<JobLink>();

            foreach (var tin in task_job_link)
            {
                if (!ES.JobLinkRepository.Instance.IsExistJobLink("xmedia.vn", tin.link))
                {
                    list_job_link.Add(tin);
                }
            }

            if (list_job_link.Count > 0)
            {
                ES.JobLinkRepository.Instance.IndexMany(list_job_link);
            }

            tssLbMessage.Text = $"Hoàn thành lấy tin tuyển dụng: {task_job_link.Count}";
            tssLbMessage.Text = $"Bắt đầu lấy CV: JobsGo";
            var task_ung_vien = await System.Threading.Tasks.Task.Run(() =>
            {
                var lst_ung_vien = jobsgo.Run(conf, task_job_link, tssLbMessage);
                var list_ung_vien_must = new List<UngVien>();
                foreach (var uv in lst_ung_vien)
                {
                    if (!ES.UngVienRepository.Instance.IsExistUngVien("xmedia.vn", uv.so_dien_thoai))
                    {
                        list_ung_vien_must.Add(uv);
                    }
                }

                if (list_ung_vien_must.Count > 0)
                {
                    ES.UngVienRepository.Instance.IndexMany(list_ung_vien_must);
                }

                return list_ung_vien_must;
            });

            btnJobsGo.Enabled = true;
            btnJobsGo.Text = $"{btnJobsGo.Text.Replace("...", "")}";
            tssLbMessage.Text = "Hoàn thành lấy CV: JobsGo";
        }

        private async void btnCareerBuilder_Click(object sender, EventArgs e)
        {
            btnCareerBuilder.Enabled = false;
            var conf = ES.JobLinkRepository.Instance.GetCauHinh("careerbuilder.vn");
            var kvp = ReadAccountSite("CareerBuilder");
            conf.username = kvp.username;
            conf.password = kvp.password;
            conf.xpath_username = kvp.xpath_username;
            conf.xpath_password = kvp.xpath_password;
            CareerBuilder career_builder = new CareerBuilder(user_profile_path,
                $"{folder_cv}\\careerbuilder.vn");

            string msg;
            tssLbMessage.Text = "Đang lấy toàn bộ Job link tại: careerbuilder.vn";
            var task = await System.Threading.Tasks.Task.Run(() =>
            {
                var lst_tin_tuyen_dung = career_builder.ExtractJobLink(conf,
                    "https://careerbuilder.vn/vi/employers/hrcentral/posting/", "xmedia.vn",
                    out msg);

                var list_job_link = new List<JobLink>();

                foreach (var tin in lst_tin_tuyen_dung)
                {
                    if (!ES.JobLinkRepository.Instance.IsExistJobLink("xmedia.vn", tin.link))
                    {
                        list_job_link.Add(tin);
                    }
                }

                if (list_job_link.Count > 0)
                {
                    ES.JobLinkRepository.Instance.IndexMany(list_job_link);
                }

                tssLbMessage.Text = $"Hoàn thành lấy tin tuyển dụng: {lst_tin_tuyen_dung.Count}";
                return lst_tin_tuyen_dung;
            });
            tssLbMessage.Text = $"Đang thực hiện lấy CV: careerbuilder.vn";
            var task_ung_vien = await System.Threading.Tasks.Task.Run(() =>
            {
                var lst_ung_vien = career_builder.Run(conf, task, tssLbMessage);
                var list_ung_vien_must = new List<UngVien>();
                foreach (var uv in lst_ung_vien)
                {
                    if (!ES.UngVienRepository.Instance.IsExistUngVien("xmedia.vn", uv.so_dien_thoai))
                    {
                        list_ung_vien_must.Add(uv);
                    }
                }

                if (list_ung_vien_must.Count > 0)
                {
                    ES.UngVienRepository.Instance.IndexMany(list_ung_vien_must);
                }

                return list_ung_vien_must;
            });
            tssLbMessage.Text = $"Hoàn thành get full cv ứng viên: careerbuilder";
            btnCareerBuilder.Enabled = true;
            btnCareerBuilder.Text = $"{btnCareerBuilder.Text.Replace("...", "")}";
        }

        private void btnJoboko_Click(object sender, EventArgs e)
        {
            string app_json = File.ReadAllText($"{curent_exe_path}\\appsettings.json");
            var jtoken = JObject.Parse(app_json);
            string api_get_all = string.Empty;
            string user = string.Empty;
            if (jtoken["Joboko"] != null && jtoken["Joboko"]["username"] != null && jtoken["Joboko"]["GetAll"] != null)
            {
                api_get_all = jtoken["Joboko"]["GetAll"].ToString();
                user = jtoken["Joboko"]["username"].ToString();
            }

            if (!string.IsNullOrEmpty(api_get_all) && !string.IsNullOrEmpty(user))
            {
                TinGoodCv(api_get_all, user);
            }
            else
            {
                tssLbMessage.Text = "Bạn hãy xem lại cấu hình";
            }
        }

        private void btnShowFolder_Click(object sender, EventArgs e)
        {
            Process.Start(folder_cv);
        }

        private void grvData_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    SearchCv();
                    break;
            }
        }

        private void grvData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mnuUngVien.Show(grvData, e.Location);
            }
        }

        private void txtTerm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13))
            {
                SearchCv();
            }
        }

        private void txtViTriUngTuyen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13))
            {
                SearchCv();
            }
        }

        private void txtHocVan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13))
            {
                SearchCv();
            }
        }

        private void txtDiaChi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13))
            {
                SearchCv();
            }
        }

        private void txt_search_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13))
            {
                GetDataJobLink();
            }
        }

        private DataGridViewCellStyle GetHyperLinkStyleForGridCell()
        {
            {
                DataGridViewCellStyle l_objDGVCS = new DataGridViewCellStyle();
                Font l_objFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Underline);
                l_objDGVCS.Font = l_objFont;
                l_objDGVCS.ForeColor = Color.Blue;
                return l_objDGVCS;
            }
        }

        private void SetHyperLinkOnGrid()
        {
            if (grvData.Columns.Contains("cv_online"))
            {
                grvData.Columns["cv_online"].DefaultCellStyle = GetHyperLinkStyleForGridCell();
            }
        }

        private void grvData_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            SetHyperLinkOnGrid();
        }

        private void grvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var cell = grvData.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (grvData.Columns[e.ColumnIndex].Name.Contains("cv_online") && cell.Value != null &&
                cell.Value.ToString() != "")
            {
                try
                {
                    string link = cell.Value.ToString();
                    if (!link.StartsWith(("http")))
                    {
                        link = "http://" + grvData.CurrentCell.EditedFormattedValue;
                        Process.Start(link);
                    }
                    else
                    {
                        link = grvData.CurrentCell.EditedFormattedValue.ToString();
                        Process.Start(link);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private DangTin GetDataForm() =>
            new DangTin
            {
                chuc_danh = txtChucDanhJob.Text,
                so_luong_tuyen = Convert.ToInt32(txtSoLuongJob.Text),
                cap_bac = cbCapBacJob.Text,
                loai_hinh_cong_viec = cbLoaiHinhJob.Text,
                dia_chi_chi_tiet = txtDiaChiJob.Text,
                nganh_nghe = new List<string>() { txtNganhNgheJob.Text },
                muc_luong = cbMucLuongJob.Text,
                dia_chi = lstboxTinhThanhJob.SelectedItem.ToString(),
                yeu_cau_cong_viec = rtbYeuCauJob.Text,
                mo_ta_cong_viec = rtbMoTaJob.Text,
                quyen_loi_ung_vien = rtbQuyenLoiJob.Text,
                so_dien_thoai = txtSdtJob.Text,
                lien_he = rtbLienHeJob.Text,
                thuoc_tinh = 1,
                app_id = app_id
            };

        private string app_id = "xmedia.vn";

        private int page_size = 8;

        // lấy danh sách tất cả tin bằng người tạo
        private void TinGoodCv(string api_get_all, string user)
        {
            string term = txt_search.Text;
            //var filter = ((KeyValuePair<string, string>)cbFilterGoodCv.SelectedItem).Key;
            var filter = "";
            if (filter == "-1")
                filter = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var result = client.GetAsync($"{api_get_all}?term={term}&nguoi_tao={user}&page={page}&page_size={page_size}");
                    if (result.Result.IsSuccessStatusCode)
                    {
                        string resultContent = result.Result.Content.ReadAsStringAsync().Result;
                        var ob = JsonConvert.DeserializeObject(resultContent);
                        var jToken = JToken.Parse(ob.ToString());
                        var dang_tin = jToken["data"].ToObject<List<DangTin>>();
                        var total = (int)jToken["total"];
                        var totalPage = (int)Math.Ceiling((double)total / page_size);
                        if (page < totalPage)
                        {
                            for (var i = page + 1; i <= totalPage; i++)
                            {
                                result = client.GetAsync(
                                    $"{api_get_all}?term={term}&nguoi_tao={user}&page={page}&page_size={page_size}");
                                if (result.Result.IsSuccessStatusCode)
                                {
                                    resultContent = result.Result.Content.ReadAsStringAsync().Result;
                                    ob = JsonConvert.DeserializeObject(resultContent);
                                    jToken = JToken.Parse(ob.ToString());
                                    dang_tin.AddRange(jToken["data"].ToObject<List<DangTin>>());
                                }
                            }
                        }
                        List<JobLink> lst_job_link = new List<JobLink>();
                        JobLink job = new JobLink(LoaiLink.JOB_LINK);
                        foreach (var item in dang_tin)
                        {
                            if (!JobLinkRepository.Instance.IsExistJobLink(app_id, item.url_tin_tuc))
                            {
                                job.json = null;
                                job.link = item.url_tin_tuc;
                                job.trang_thai = TrangThai.DANG_SU_DUNG;
                                job.trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY;
                                job.app_id = item.app_id;
                                job.ten_job = item.chuc_danh;
                                job.nguoi_tao = item.nguoi_tao;
                                lst_job_link.Add(job);
                            }
                        }
                        var kq = JobLinkRepository.Instance.IndexMany(lst_job_link);
                        tssLbMessage.Text = $"Số link lấy được {kq}";

                        //lst_user.Add(user);
                        //var get_all_good_cv =
                        //    JobLinkRepository.Instance.Search(app_id, term, lst_user, 0, 0, 0, 0, "", "", page, out var total_recs, page_size, null);

                        //grvGoodcv.DataSource = get_all_good_cv.Select(x => new
                        //{
                        //    ten_job = x.ten_job,
                        //    link_job = x.link,
                        //    trang_thai_xu_ly = x.trang_thai_xu_ly,
                        //    tong_so_cv = x.tong_so_cv,
                        //    ngay_xu_ly = x.ngay_xu_ly > 0 ? XMedia.XUtil.EpochToTimeString(x.ngay_xu_ly) : "",
                        //    ngay_tao = XMedia.XUtil.EpochToTime(x.ngay_tao).ToString("dd-MM-yyyy"),
                        //    nguoi_tao_link = x.nguoi_tao
                        //}).ToList();
                        //ucPagingGoodCv1.Total_recs = total_recs;
                        //ucPagingGoodCv1.Page = page;
                        //ucPagingGoodCv1.Page_size = page_size;
                        //ucPagingGoodCv1.DisplayPaging();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // post tin lên goodcv
        private void btnPostTin_Click(object sender, EventArgs e)
        {
            //PostTinJoboko();
        }

        private void PostTin(UngVienJobModel.ChiTietTinModel dangtin, string user, string pass, string api_post)
        {
            try
            {
                // đọc từ file config
                string id_tin = string.Empty;
                JobLink job = new JobLink(LoaiLink.JOB_LINK)
                {
                    json = null
                };

                using (var client = new HttpClient())
                {
                    //GetDataForm()
                    var json = JsonConvert.SerializeObject(dangtin, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var res = client.PostAsync($"{api_post}", data);
                    if (res.Result.IsSuccessStatusCode)
                    {
                        string resultContent = res.Result.Content.ReadAsStringAsync().Result;
                        var json_re = JsonConvert.DeserializeObject(resultContent);
                        var ob = JToken.Parse(json_re.ToString());
                        var tin = JsonConvert.DeserializeObject<DangTin>(ob["data"].ToString());

                        if (tin != null)
                        {
                            var save_uv = NewRepository.Instance.Index(tin, out id_tin);
                            MessageBox.Show($"{ob["msg"]}");
                            job.ten_job = tin.chuc_danh;
                            job.nguoi_tao = tin.nguoi_tao;
                            job.app_id = tin.app_id;
                            job.link = tin.url_tin_tuc;
                            //job.app_id = app_id;
                        }

                        if (!string.IsNullOrEmpty(id_tin))
                        {
                            // check xem job đã tồn tại hay chưa
                            //if (!ES.JobLinkRepository.Instance.IsExistJobLinkByChucDanh(app_id, job.ten_job))
                            var save = JobLinkRepository.Instance.Index(job);
                            MessageBox.Show(save ? "Đăng tin thành công" : "Đăng tin thất bại");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        // lấy các cv đã đăng ở trong url
        private List<UngVien> LayCvUngVienTinDaDang(List<string> lst_id, string api_getcv)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json_id = JsonConvert.SerializeObject(lst_id);
                    var res = client.GetAsync(
                        $"{api_getcv}?id={json_id}&page={page}&page_size={page_size}");
                    if (res.Result.IsSuccessStatusCode)
                    {
                        string resultContent = res.Result.Content.ReadAsStringAsync().Result;
                        var ob = JsonConvert.DeserializeObject(resultContent);
                        JToken jToken = JToken.Parse(ob.ToString());
                        var lst_ung_vien = jToken["data"].ToObject<List<UngVien>>();
                        var total = (int)jToken["total"];

                        //var num = Math.Ceiling(total / page_size);

                        var totalPage = (int)Math.Ceiling((double)total / page_size);

                        if (page <= totalPage)
                        {
                            for (var i = page + 1; i <= totalPage; i++)
                            {
                                res = client.GetAsync(
                                    $"{api_getcv}?id={json_id}&page={i}&page_size={page_size}");
                                if (res.Result.IsSuccessStatusCode)
                                {
                                    resultContent = res.Result.Content.ReadAsStringAsync().Result;
                                    ob = JsonConvert.DeserializeObject(resultContent);
                                    jToken = JToken.Parse(ob.ToString());
                                    lst_ung_vien.AddRange(jToken["data"].ToObject<List<UngVien>>());
                                }
                            }
                        }

                        return lst_ung_vien;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        //private void grvGoodcv_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        mnuGoodCv.Show(grvGoodcv, e.Location);
        //    }
        //}

        //private void btnUpdateTinGoodCv_Click(object sender, EventArgs e)
        //{
        //    var lst_id = grvGoodcv.Rows.OfType<DataGridViewRow>().Select(r => r.Cells[1].Value.ToString()).ToList();
        //    if (lst_id.Count > 0)
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var json = JsonConvert.SerializeObject(lst_id);
        //            var data = new StringContent(json, Encoding.UTF8, "application/json");
        //            var result = client.PostAsync($"{api_url}dangtin/updatetrangthai", data);
        //            if (result.Result.IsSuccessStatusCode)
        //            {
        //                string resultContent = result.Result.Content.ReadAsStringAsync().Result;
        //                var json_re = JsonConvert.DeserializeObject(resultContent);
        //                var ob = JToken.Parse(json_re.ToString());
        //                var lst = JsonConvert.DeserializeObject<List<DangTin>>(ob["data"].ToString());
        //                if (lst.Count > 0)
        //                {
        //                    foreach (var item in lst)
        //                    {
        //                        var update_uv = NewRepository.Instance.UpdateThuocTinh(item.id, item.thuoc_tinh);
        //                        if (update_uv)
        //                        {
        //                            Console.WriteLine("Đã update ");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            btnSaveConfig.Enabled = false;
            MessageBox.Show(SaveAccountConfig() ? "Lưu tài khoản thành công" : "Lỗi trong quá trình lưu tài khoản");
            btnSaveConfig.Enabled = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var s = XMedia.XUtil.Encode("@xmedia1234");
            Console.WriteLine();
        }
    }
}