using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UngVienJobModel;
using UVJHelper;
using XMedia;

namespace JobsGoHelper
{
    public class JobsGo
    {
        private string cv_save_path;
        private string user_profile_path;
        private bool is_authenticated = false;
        private bool show_browser = true;

        public JobsGo(string _user_profile_path, string _cv_save_path)
        {   
            user_profile_path = _user_profile_path;
            cv_save_path = _cv_save_path;
            if (!Directory.Exists(cv_save_path))
            {
                Directory.CreateDirectory(cv_save_path);
            }
        }

        public List<JobLink> ExtractJobLink(CauHinh ch, List<string> lst_link, string app_id, out string msg)
        {
            is_authenticated = false;
            msg = "";
            List<JobLink> lst = new List<JobLink>();
            string wait_xpath = ".//li[@class='dropdown dropdown-user']";
            //ch.username = "tuyendung@xmedia.vn";
            //ch.password = "@xmedia123";
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, show_browser))
            {
                foreach (var link in lst_link)
                {
                    string url = browser.GoTo(link);
                    is_authenticated = link != url ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out msg, wait_xpath, false) : browser.GetUrl().Contains(url);
                    browser.GoTo(link);
                    try
                    {
                        var find_modal = browser.Find("//div[@id='modal']");
                        if (find_modal.Count > 0)
                        {
                            foreach (var modal in find_modal)
                            {
                                modal.FindElement(By.ClassName("close"))?.Click();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    if (is_authenticated)
                    {
                        var check_paging = browser.Find(".//ul[@class='pagination']/li/a");
                        if (check_paging.Count() > 0)
                        {
                            var list_url_page = new List<string>();
                            foreach (var loop in check_paging)
                            {
                                var url_page = browser.GetAttribute(loop, "href");
                                list_url_page.Add(url_page);
                            }

                            list_url_page = list_url_page.Distinct().ToList();
                            foreach (var go_url in list_url_page)
                            {
                                browser.GoTo(go_url);
                                System.Threading.Thread.Sleep(1500);
                                var eles_paging = browser.Find(".//table[@class='kv-grid-table table table-hover table-bordered table-striped']//tbody/tr/td[2]/strong/a[@title='Xem chi tiết việc làm và các ứng viên']");
                                if (eles_paging.Count > 0)
                                {
                                    foreach (var item in eles_paging)
                                    {
                                        var url_tin = browser.GetAttribute(item, "href");

                                        var ten_job = item.Text;

                                        if (!string.IsNullOrEmpty(url_tin))
                                        {
                                            var job = new JobLink(LoaiLink.JOB_LINK)
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
                            }
                        }
                        else
                        {
                            var eles = browser.Find(".//table[@class='kv-grid-table table table-hover table-bordered table-striped']//tbody/tr/td[2]/strong/a[@title='Xem chi tiết việc làm và các ứng viên']");
                            if (eles.Count > 0)
                            {
                                foreach (var item in eles)
                                {
                                    var url_tin = browser.GetAttribute(item, "href");

                                    var ten_job = item.Text;

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
                        }
                    }
                }
            }
            return lst;
        }

        public List<UngVien> Run(CauHinh ch, List<JobLink> lst_job_link)
        {
            is_authenticated = false;
            List<UngVien> lst_ung_vien = new List<UngVien>();
            string wait_xpath = ".//li[@class='dropdown dropdown-user']";

            using (var browser = new XBrowser(user_profile_path, cv_save_path, string.Empty, false, show_browser))
            {
                foreach (var job in lst_job_link)
                {
                    string url = browser.GoTo(job.link);
                    is_authenticated = url.Contains(ch.url_login) ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg, wait_xpath, false) : browser.GetUrl().Contains(url);
                    if (is_authenticated)
                    {
                        //string r_url = browser.GoTo(job.link);
                        System.Threading.Thread.Sleep(1000);
                        browser.FindAndClick(".//a[@href='#tab2']");

                        var ung_vien = browser.Find(".//tbody[@id='applied']//strong/a");
                        var url_detail_uv = new List<string>();
                        foreach (var uv in ung_vien)
                        {
                            var link_uv = browser.GetAttribute(uv, "href");
                            url_detail_uv.Add(link_uv);
                        }

                        if (url_detail_uv.Count > 0)
                        {
                            foreach (var u_v in url_detail_uv)
                            {
                                browser.GoTo(u_v);
                                var lst_cv_by_job = XuLyCV(browser, job);
                                lst_ung_vien.AddRange(lst_cv_by_job);
                            }
                        }
                        else
                        {
                            job.trang_thai_xu_ly = TrangThaiXuLy.DA_XU_LY;
                            job.ngay_xu_ly = XUtil.TimeInEpoch();
                        }
                        job.tong_so_cv = url_detail_uv.Count;
                        ES.JobLinkRepository.Instance.UpdateTrangThaiXuLy(job);
                    }
                }
            }

            return new List<UngVien>(lst_ung_vien);
        }

        public List<UngVien> Run(CauHinh ch, List<JobLink> lst_job_link, ToolStripStatusLabel toolStripStatusLabel1)
        {
            is_authenticated = false;
            List<UngVien> lst_ung_vien = new List<UngVien>();
            string wait_xpath = ".//li[@class='dropdown dropdown-user']";

            using (var browser = new XBrowser(user_profile_path, cv_save_path, string.Empty, false, show_browser))
            {
                foreach (var job in lst_job_link)
                {
                    string url = browser.GoTo(job.link);
                    is_authenticated = url.Contains(ch.url_login) ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg, wait_xpath, false) : browser.GetUrl().Contains(url);
                    if (is_authenticated)
                    {
                        //string r_url = browser.GoTo(job.link);
                        System.Threading.Thread.Sleep(1000);
                        browser.FindAndClick(".//a[@href='#tab2']");

                        var ung_vien = browser.Find(".//tbody[@id='applied']//strong/a");
                        var url_detail_uv = new List<string>();
                        foreach (var uv in ung_vien)
                        {
                            var link_uv = browser.GetAttribute(uv, "href");
                            url_detail_uv.Add(link_uv);
                        }

                        if (url_detail_uv.Count > 0)
                        {
                            foreach (var u_v in url_detail_uv)
                            {
                                browser.GoTo(u_v);
                                toolStripStatusLabel1.Text = $"Đang xử lý link:{job.link}";
                                var lst_cv_by_job = XuLyCV(browser, job);
                                lst_ung_vien.AddRange(lst_cv_by_job);
                            }
                        }
                        else
                        {
                            job.trang_thai_xu_ly = TrangThaiXuLy.DA_XU_LY;
                            job.ngay_xu_ly = XUtil.TimeInEpoch();
                            toolStripStatusLabel1.Text = $"Không tìm thấy ứng viên link:{job.link}";
                        }
                        job.tong_so_cv = url_detail_uv.Count;
                        ES.JobLinkRepository.Instance.UpdateTrangThaiXuLy(job);
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
                ttuv.domain = "jobsgo.vn";
                //ttuv.full_text = block_ung_vien.FindElement(By.XPath("//div[@class='Card__root ApplicationCard__root']")).;
                ung_vien.thong_tin_chung = ttuv;
                ung_vien.ho_ten = browser.GetInnerText(".//div[@title='Họ tên ứng viên']", 200);
                ung_vien.link_cv_online = browser.GetAttribute("//div[@class='lnks']/a[@class='btn-download-cv lnk cv']", "href");
                ung_vien.ky_nang = browser.GetInnerText("//div[@class='skills-list']");
                ung_vien.hoc_van = browser.GetInnerText("//div[@class='resume-items']//div[@class='name']");

                //div[@class='info-list']/ul/li[1]
                ung_vien.ngay_sinh = 0;
                ung_vien.so_dien_thoai = browser.GetInnerText("//div[@class='info-list']/ul/li[3]/span[@class='tel']");
                ung_vien.email = browser.GetInnerText("//div[@class='info-list']/ul/li[4]/span[@class='email']");
                ung_vien.dia_chi = browser.GetInnerText("//div[@class='info-list']/ul/li[5]/span[@class='address']");

                Uri uri = new Uri(job.link);
                ung_vien.domain = uri.Host;
                var id = string.Join("/", job.link.Split('/').Skip(5));
                //id = id.Substring(0, id.LastIndexOf('/'));
                ung_vien.custom_id = id;
                //Tai file
                //javascript:void(0)

                var link_cv = browser.GetAttribute("//div[@class='lnks']/a[@class='btn-download-cv lnk cv']", "href");

                if (link_cv.Contains("javascript:void(0)"))
                {
                    System.Threading.Thread.Sleep(2500);
                    link_cv = browser.GetAttribute("//div[@class='lnks']/a[@class='btn-download-cv lnk cv']", "href");
                    ung_vien.link_cv_online = link_cv;
                }
                else
                {
                    System.Threading.Thread.Sleep(2500);
                    ung_vien.link_cv_online = link_cv;
                }

                var cv_byte = browser.DownloadFile(ung_vien.link_cv_online, out string cv_file_name);
                if (cv_byte != null)
                {
                    ung_vien.cv_byte = cv_byte;
                    WriteFile($"{cv_save_path}\\{cv_file_name}", cv_byte);
                    ung_vien.link_cv_offline = cv_file_name;
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