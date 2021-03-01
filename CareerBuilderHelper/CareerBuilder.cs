using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UngVienJobModel;
using UVJHelper;
using XMedia;

namespace CareerBuilderHelper
{
    public class CareerBuilder
    {
        private string cv_save_path;
        private string user_profile_path;
        private bool is_authenticated = false;
        private bool show_browser = true;

        public CareerBuilder(string _user_profile_path, string _cv_save_path)
        {
            user_profile_path = _user_profile_path;
            cv_save_path = _cv_save_path;
            if (!Directory.Exists(cv_save_path))
            {
                Directory.CreateDirectory(cv_save_path);
            }
        }

        public List<JobLink> ExtractJobLink(CauHinh ch, string link, string app_id, out string msg)
        {
            is_authenticated = false;
            msg = "";
            List<JobLink> lst = new List<JobLink>();

            using (var browser = new XBrowser(user_profile_path, string.Empty, false, show_browser))
            {
                string url = browser.GoTo(ch.url_login);
                string wait_xpath = ".//div[@class='kv_login']/div/span";
                System.Threading.Thread.Sleep(2000);
                var home = browser.GetUrl();
                if (browser.Find(wait_xpath).Count > 0)
                {
                    is_authenticated = true;
                }
                else
                {
                    is_authenticated = home == "https://careerbuilder.vn/vi/employers/hrcentral" || (link != url
                        ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username,
                            ch.xpath_password, out msg, wait_xpath, false)
                        : browser.GetUrl().Contains(url));
                }

                if (is_authenticated)
                {
                    browser.GoTo(link);
                    System.Threading.Thread.Sleep(2000);
                    List<string> lst_links = new List<string>();

                    var ds_tin =
                        browser.Find(
                            "//div[@id='gird_standard']//dd//span[@class='rc_col_title3 info_jobseeker']/a[1]");

                    foreach (var tin in ds_tin)
                    {
                        var get_tin = browser.GetAttribute(tin, "href");
                        var ten_job = tin.Text;

                        if (!string.IsNullOrEmpty(get_tin))
                        {
                            JobLink job = new JobLink(LoaiLink.JOB_LINK)
                            {
                                app_id = app_id,
                                link = get_tin,
                                trang_thai = TrangThai.DANG_SU_DUNG,
                                trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                nguoi_tao = ch.username,
                                ten_job = ten_job
                            };
                            lst.Add(job);
                        }
                    }
                }
            }
            return lst;
        }

        public List<UngVien> Run(CauHinh ch, List<JobLink> lst_job_link, ToolStripStatusLabel label)
        {
            is_authenticated = false;
            List<UngVien> lst_ung_vien = new List<UngVien>();
            string wait_xpath = ".//div[@class='kv_login']/div/span";
            using (var browser = new XBrowser(user_profile_path, cv_save_path, string.Empty, false, show_browser))
            {
                if (lst_job_link.Count < 1)
                {
                    label.Text = "Không có tin nào đang được đăng trực tuyến";
                }
                else
                {
                    foreach (var job in lst_job_link)
                    {
                        string url = browser.GoTo(job.link);
                        is_authenticated = url.Contains(ch.url_login) ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg, wait_xpath, false) : browser.GetUrl().Contains(url);
                        if (is_authenticated)
                        {
                            //string r_url = browser.GoTo(job.link);
                            System.Threading.Thread.Sleep(1000);
                            browser.FindAndClick("//div[@class='top']/ul//div/a");
                            System.Threading.Thread.Sleep(1000);
                            var lst_url_uv = new List<string>();
                            var ung_vien =
                                browser.Find(".//form[@id='editFrm1']/dd/span[@class='rc_col_310px info_jobseeker']/div/a");
                            if (ung_vien.Count > 0)
                            {
                                foreach (var uv in ung_vien)
                                {
                                    var url_uv = browser.GetAttribute(uv, "href");
                                    lst_url_uv.Add(url_uv);
                                }
                            }
                            if (lst_url_uv.Count > 0)
                            {
                                foreach (var item in lst_url_uv)
                                {
                                    label.Text = $"Đang xử lý link: {job.link}";
                                    browser.GoTo(item);
                                    var lst_cv_by_job = XuLyCV(browser, job);
                                    lst_ung_vien.AddRange(lst_cv_by_job);
                                }
                            }
                            job.tong_so_cv = lst_url_uv.Count;
                            ES.JobLinkRepository.Instance.UpdateTrangThaiXuLy(job);
                        }
                    }
                }
            }

            return new List<UngVien>(lst_ung_vien);
        }

        private List<UngVien> XuLyCV(XBrowser browser, JobLink job)
        {
            job.trang_thai_xu_ly = TrangThaiXuLy.DA_XU_LY;

            List<UngVien> lst_ung_vien = new List<UngVien>();
            try
            {
                UngVien ung_vien = new UngVien();
                ung_vien.ngay_tao = ung_vien.ngay_sua = XUtil.TimeInEpoch(DateTime.Now);
                ung_vien.app_id = job.app_id;
                ung_vien.job_link = job.link;
                ung_vien.vi_tri = job.ten_job;

                ThongTinChungUngVien ttuv = new ThongTinChungUngVien();
                ttuv.domain = "https://careerbuilder.vn/";
                //ttuv.full_text = block_ung_vien.FindElement(By.XPath("//div[@class='Card__root ApplicationCard__root']")).;
                ung_vien.thong_tin_chung = ttuv;
                ung_vien.ho_ten = browser.GetInnerText(".//div[@class='act_more']/a/b", 200);
                ung_vien.link_cv_online = "";
                ung_vien.ky_nang = "";
                ung_vien.hoc_van = browser.GetInnerText(".//ul[@class='block_info bullet block01']/li[2]/div");

                //div[@class='info-list']/ul/li[1]
                ung_vien.ngay_sinh = 0;
                ung_vien.so_dien_thoai = browser.GetInnerText(".//ul[@class='block_info block02']/li[1]/div");
                ung_vien.email = browser.GetInnerText(".//ul[@class='block_info block02']/li[2]/div");
                ung_vien.dia_chi = browser.GetInnerText(".//ul[@class='block_info block02']/li[3]/div");

                Uri uri = new Uri(job.link);
                ung_vien.domain = uri.Host;
                var id = string.Join("/", job.link.Split('/').Skip(5));
                //id = id.Substring(0, id.LastIndexOf('/'));
                ung_vien.custom_id = id;
                //Tai file
                var ifr = browser.FindFirst("//div[@id='tabs-chitiethoso']//iframe");
                if (ifr != null)
                {
                    ung_vien.link_cv_offline = browser.DownloadByBrowserInIFrame("//div[@id='tabs-chitiethoso']//iframe", "//button[@id='download']");
                    if (!string.IsNullOrEmpty(ung_vien.link_cv_offline))
                    {
                        ung_vien.cv_byte = File.ReadAllBytes($"{cv_save_path}\\{ung_vien.link_cv_offline}");
                    }
                }
                ung_vien.full_text = browser.GetPageSource();
                ung_vien.nguoi_tao = job.nguoi_tao;
                lst_ung_vien.Add(ung_vien);
                var count = lst_ung_vien.Count;
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                job.trang_thai_xu_ly = TrangThaiXuLy.LOI;
                job.thong_tin_xu_ly = ex.Message;
            }
            job.ngay_xu_ly = XUtil.TimeInEpoch();

            return lst_ung_vien;
        }

        private void WriteFile(string full_file_name, byte[] by)
        {
            try
            {
                File.WriteAllBytes(full_file_name, by);
            }
            catch (Exception)
            {
            }
        }
    }
}