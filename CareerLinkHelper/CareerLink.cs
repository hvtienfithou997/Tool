using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UngVienJobModel;
using UVJHelper;

namespace CareerLinkHelper
{
    public class CareerLink
    {
        private string cv_save_path;
        private string user_profile_path;
        private bool is_authenticated = false;
        private bool show_browser = true;

        public CareerLink(string _user_profile_path, string _cv_save_path)
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
                string url = browser.GoTo(link);
                string wait_xpath = ".//a[@class='user-image']";
                is_authenticated = url.Contains(ch.url_login)
                    ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password,
                        out msg, wait_xpath, false)
                    : browser.GetUrl().Contains(url);
                if (is_authenticated)
                {
                    browser.GoTo(link);
                    var eles_tin_tuyen_dung = browser.Find("//form[@id='employer-job-manage']//a[contains(@href,'thu-xin-viec-da-nhan')]");

                    if (eles_tin_tuyen_dung != null)
                    {
                        foreach (var item in eles_tin_tuyen_dung)
                        {
                            var ten_job = item.FindElement(By.XPath(".//parent::div/parent::div/parent::div/parent::div/following-sibling::div/ul[@class='fa-ul']//li[1]")).Text;
                            var url_tin = browser.GetAttribute(item, "href");
                            if (!string.IsNullOrEmpty(url_tin))
                            {
                                JobLink job = new JobLink(LoaiLink.JOB_LINK)
                                {
                                    app_id = app_id,
                                    link = url_tin,
                                    trang_thai = TrangThai.DANG_SU_DUNG,
                                    trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                    nguoi_tao = ch.username,
                                    ten_job = ten_job
                                };

                                lst.Add(job);
                            }
                        }
                    }
                    var ele_paging = browser.Find("//ul[@class='pagination']//a");
                    if (ele_paging != null)
                    {
                        var lst_url_paging = new List<string>();
                        foreach (var ele in ele_paging)
                        {
                            lst_url_paging.Add(browser.GetAttribute(ele, "href"));
                        }
                        lst_url_paging = lst_url_paging.Distinct().ToList();
                        foreach (var lnk_paging in lst_url_paging)
                        {
                            browser.GoTo(lnk_paging);

                            eles_tin_tuyen_dung = browser.Find("//form[@id='employer-job-manage']//a[contains(@href,'thu-xin-viec-da-nhan')]");

                            if (eles_tin_tuyen_dung != null)
                            {
                                foreach (var item in eles_tin_tuyen_dung)
                                {
                                    var ten_job = item.FindElement(By.XPath("//parent::div/parent::div/parent::div/parent::div/following-sibling::div/ul[@class='fa-ul']//li[1]")).Text;
                                    var url_tin = browser.GetAttribute(item, "href");
                                    if (!string.IsNullOrEmpty(url_tin))
                                    {
                                        JobLink job = new JobLink(LoaiLink.JOB_LINK);
                                        job.app_id = app_id;
                                        job.link = url_tin;
                                        job.trang_thai = TrangThai.DANG_SU_DUNG;
                                        job.trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY;
                                        job.nguoi_tao = XMedia.XUtil.DecodeToken(ch.username);
                                        job.ten_job = ten_job;
                                        lst.Add(job);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            lst = lst.GroupBy(x => x.link).Select(s => s.First()).ToList();
            return lst;
        }

        public List<UngVien> Run(CauHinh ch, List<JobLink> lst_job_link)
        {
            is_authenticated = false;
            List<UngVien> lst_ung_vien = new List<UngVien>();

            using (var browser = new XBrowser(user_profile_path, cv_save_path, string.Empty, false, show_browser))
            {
                foreach (var job in lst_job_link)
                {
                    string url = browser.GoTo(job.link);
                    is_authenticated = url.Contains(ch.url_login) ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg) : browser.GetUrl().Contains(url);
                    if (is_authenticated)
                    {
                        string r_url = url;
                        if (url != job.link)
                        {
                            r_url = browser.GoTo(job.link);
                        }
                        if (r_url.Contains(job.link))
                        {
                            var lst_cv_by_job = XuLyCV(browser, job);
                            //Phân trang CV
                            ////ul[@class='pagination']//a
                            var cv_paging_eles = browser.Find("//ul[@class='pagination']//a");
                            if (cv_paging_eles.Count() > 0)
                            {
                                var lst_link_paging = new List<string>();
                                foreach (var pg_ele in cv_paging_eles)
                                {
                                    lst_link_paging.Add(browser.GetAttribute(pg_ele, "href"));
                                }
                                lst_link_paging = lst_link_paging.Distinct().ToList();
                                foreach (var item in lst_link_paging)
                                {
                                    browser.GoTo(item);
                                    lst_cv_by_job.AddRange(XuLyCV(browser, job));
                                    ES.JobLinkRepository.Instance.UpdateTrangThaiXuLy(job);
                                }
                            }

                            job.tong_so_cv = lst_cv_by_job.Count;
                            lst_ung_vien.AddRange(lst_cv_by_job);
                        }
                    }
                }
            }

            return new List<UngVien>(lst_ung_vien);
        }

        public List<UngVien> Run(CauHinh ch, List<JobLink> lst_job_link, ToolStripStatusLabel label)
        {
            is_authenticated = false;
            List<UngVien> lst_ung_vien = new List<UngVien>();

            using (var browser = new XBrowser(user_profile_path, cv_save_path, string.Empty, false, show_browser))
            {
                foreach (var job in lst_job_link)
                {
                    string url = browser.GoTo(job.link);
                    is_authenticated = url.Contains(ch.url_login) ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg) : browser.GetUrl().Contains(url);
                    if (is_authenticated)
                    {
                        string r_url = url;
                        if (url != job.link)
                        {
                            r_url = browser.GoTo(job.link);
                        }
                        if (r_url.Contains(job.link))
                        {
                            label.Text = $"Đang xử lý link: {job.link}";
                            var lst_cv_by_job = XuLyCV(browser, job);
                            //Phân trang CV
                            ////ul[@class='pagination']//a
                            var cv_paging_eles = browser.Find("//ul[@class='pagination']//a");
                            if (cv_paging_eles.Count() > 0)
                            {
                                var lst_link_paging = new List<string>();
                                foreach (var pg_ele in cv_paging_eles)
                                {
                                    lst_link_paging.Add(browser.GetAttribute(pg_ele, "href"));
                                }
                                lst_link_paging = lst_link_paging.Distinct().ToList();
                                foreach (var item in lst_link_paging)
                                {
                                    browser.GoTo(item);
                                    lst_cv_by_job.AddRange(XuLyCV(browser, job));
                                    ES.JobLinkRepository.Instance.UpdateTrangThaiXuLy(job);
                                }
                            }

                            job.tong_so_cv = lst_cv_by_job.Count;
                            lst_ung_vien.AddRange(lst_cv_by_job);
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
                var eles_ung_vien = browser.Find("//div[@class='col-sm-9']//form//div[@class='list-group-item-heading']");
                if (eles_ung_vien.Count > 0)
                {
                    Dictionary<string, string> dic_cv_ung_vien = new Dictionary<string, string>();
                    foreach (var item_ung_vien in eles_ung_vien)
                    {
                        try
                        {
                            var ho_ten_ele = browser.FindChildElement(item_ung_vien, ".//span[@class='text-accent']");
                            var link_cv_ele = browser.FindChildElement(item_ung_vien, ".//a");

                            if (ho_ten_ele != null && link_cv_ele != null)
                            {
                                string href = browser.GetAttribute(link_cv_ele, "href").Trim();
                                string ho_ten = browser.GetAttribute(ho_ten_ele, "innerText").Trim();
                                if (!dic_cv_ung_vien.ContainsKey(href))
                                {
                                    dic_cv_ung_vien.Add(href, ho_ten);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    foreach (var item in dic_cv_ung_vien)
                    {
                        browser.GoTo(item.Key);
                        UngVien ung_vien = new UngVien();
                        ung_vien.ngay_tao = ung_vien.ngay_sua = XMedia.XUtil.TimeInEpoch();
                        ung_vien.app_id = job.app_id;
                        ung_vien.job_link = job.link;
                        ung_vien.ho_ten = item.Value;
                        ung_vien.full_text = browser.GetPageSource();
                        ung_vien.vi_tri = job.ten_job;
                        ThongTinChungUngVien ttuv = new ThongTinChungUngVien();
                        ttuv.domain = UrlToDomain(item.Key);
                        ttuv.full_text = browser.GetInnerHtml("//div[@class='resume-show']//div[@class='media']", 200);
                        ttuv.full_text += browser.GetInnerHtml("//div[@id='divResumeContactInformation']", 200);
                        ung_vien.thong_tin_chung = ttuv;
                        //Bóc tách XPATH để lấy được thông tin này nếu có
                        ung_vien.kinh_nghiem = browser.GetInnerHtml("//h4[contains(text(),'Kinh nghiệm')]/following-sibling::dl", 200);
                        ung_vien.ky_nang = "";
                        ung_vien.hoc_van = browser.GetInnerHtml("//h4[contains(text(),'Học vấn')]/following-sibling::dl", 200);
                        ung_vien.email = browser.GetInnerHtml("//a[contains(@href,'mailto')]", 200);
                        ung_vien.so_dien_thoai = browser.GetInnerHtml("//div[@id='divResumeContactInformation']//dt[contains(text(),'Điện')]/following-sibling::dd[1]", 200);
                        string ngay_sinh = browser.GetInnerText("//span[@class='confidential-birth-day']", 500);
                        ung_vien.domain = UrlToDomain(item.Key);
                        ung_vien.custom_id = item.Key.Substring(item.Key.LastIndexOf("/") + 1);
                        ung_vien.nguoi_tao = job.nguoi_tao;
                        if (!string.IsNullOrEmpty(ngay_sinh))
                        {
                            //Convert ngay sinh
                            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();
                            dtfi.ShortDatePattern = "dd/MM/yyyy";
                            dtfi.DateSeparator = "/";
                            try
                            {
                                var bird_day = Convert.ToDateTime(ngay_sinh, dtfi);

                                ung_vien.ngay_sinh = XMedia.XUtil.TimeInEpoch(bird_day);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        var ifr = browser.FindFirst("//iframe");
                        if (ifr != null)
                        {
                            ung_vien.link_cv_offline = browser.DownloadByBrowserInIFrame("//iframe", "//button[@id='download']");
                            if (!string.IsNullOrEmpty(ung_vien.link_cv_offline))
                            {
                                ung_vien.cv_byte = File.ReadAllBytes($"{cv_save_path}\\{ung_vien.link_cv_offline}");
                            }
                        }

                        lst_ung_vien.Add(ung_vien);
                    }
                }
                else
                {
                    job.thong_tin_xu_ly = Common.KHONG_TIM_THAY_UNG_VIEN;
                }
            }
            catch (Exception ex)
            {
                job.trang_thai_xu_ly = TrangThaiXuLy.LOI;
                job.thong_tin_xu_ly = ex.Message;
            }
            job.ngay_xu_ly = XMedia.XUtil.TimeInEpoch();
            return lst_ung_vien;
        }

        private string UrlToDomain(string url)
        {
            string domain = url;
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri re))
                {
                    domain = re.Host;
                }
            }
            catch (Exception)
            {
            }
            return domain;
        }

        private void WriteFile(string full_file_name, byte[] by)
        {
            try
            {
                System.IO.File.WriteAllBytes(full_file_name, by);
            }
            catch (Exception)
            {
            }
        }
    }
}