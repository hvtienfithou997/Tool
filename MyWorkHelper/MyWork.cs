using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UngVienJobModel;
using UVJHelper;
using XMedia;

namespace MyWorkHelper
{
    public class MyWork
    {
        private string cv_save_path;
        private string user_profile_path;
        private bool is_authenticated = false;
        private bool show_browser = true;

        public MyWork(string _user_profile_path, string _cv_save_path)
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
                string url = browser.GoTo("https://mywork.com.vn/nha-tuyen-dung/dang-nhap");
                string wait_xpath = ".//div[@class='icon_menu user-info text-center']";
                System.Threading.Thread.Sleep(2000);
                var home = browser.GetUrl();
                is_authenticated = home == "https://mywork.com.vn/" || (link != url
                    ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username,
                        ch.xpath_password, out msg, wait_xpath, false)
                    : browser.GetUrl().Contains(url));

                if (is_authenticated)
                {
                    browser.GoTo(link);
                    System.Threading.Thread.Sleep(5000);
                    List<string> lst_links = new List<string>();
                    var cv_paging_eles = browser.Find("//ul[@class='page-01-lst']//a").Where(x => x.Text != "Trang trước" && x.Text != "Trang sau");

                    if (cv_paging_eles.Count() > 0)
                    {
                        var lst_link_paging = new List<string>();
                        foreach (var pg_ele in cv_paging_eles)
                        {
                            browser.Click(pg_ele);
                            System.Threading.Thread.Sleep(3000);
                            var e = browser.Find(".//ul[@class='jobslist-01-ul']/li/div/div/ul/li[1]/a");

                            try
                            {
                                foreach (var item_href in e)
                                {
                                    var ten_job = item_href.Text;
                                    var url_tin = browser.GetAttribute(item_href, "href");
                                    lst_links.Add(url_tin);
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
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
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

            using (var browser = new XBrowser(user_profile_path, cv_save_path, string.Empty, false, show_browser))
            {
                foreach (var job in lst_job_link)
                {
                    string url = browser.GoTo(job.link);
                    string wait_xpath = ".//div[@class='icon_menu user-info text-center']";
                    if (url.Contains(ch.url_login) || browser.GetUrl() == "https://mywork.com.vn/")
                    {
                        is_authenticated = browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg, wait_xpath, true);
                    }
                    else
                    {
                        is_authenticated = browser.GetUrl().Contains(url);
                    }
                    if (is_authenticated)
                    {
                        string r_url = browser.GoTo(job.link);
                        System.Threading.Thread.Sleep(3000);

                        if (r_url.Contains(job.link))
                        {
                            int sleep = 30000;
                            var find_ung_vien = browser.Find(".//div/span[text()='Không có dữ liệu']");
                            if (find_ung_vien.Count < 1)
                            {
                                while (sleep > 0)
                                {
                                    sleep -= 500;
                                    System.Threading.Thread.Sleep(500);
                                    List<string> lst_cv_ung_vien = new List<string>();
                                    var lst_cv_by_job = new List<UngVien>();
                                    var lst_new = browser.Find("//table[@class='el-table__body']//tbody//div/div/div[1]/a");
                                    if (lst_new.Count < 1)
                                    {
                                        job.thong_tin_xu_ly = Common.KHONG_TIM_THAY_UNG_VIEN;
                                    }
                                    var element = browser.Find("//ul[@class='pagination']//a");

                                    if (element.Count > 0)
                                    {
                                        var cv_paging_eles = element.Where(x => x.Text != null && x.Text != "Trang trước" && x.Text != "Trang sau");

                                        if (cv_paging_eles.Count() > 0)
                                        {
                                            var all_links_in_page = new List<string>();
                                            var links_process = new List<string>();
                                            foreach (var pg_ele in cv_paging_eles)
                                            {
                                                browser.Click(pg_ele);
                                                System.Threading.Thread.Sleep(2000);
                                                lst_new = browser.Find("//table[@class='el-table__body']//tbody//div/div/div[1]/a");

                                                foreach (var lst_ung_vien_new in lst_new)
                                                {
                                                    var url_ung_vien = browser.GetAttribute(lst_ung_vien_new, "href");
                                                    links_process.Add(url_ung_vien);
                                                }
                                            }
                                            foreach (var item in links_process)
                                            {
                                                browser.GoTo(item);
                                                System.Threading.Thread.Sleep(3000);
                                                lst_cv_by_job.AddRange(XuLyCV(browser, job, item));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var current_href = new List<string>();
                                        foreach (var item in lst_new)
                                        {
                                            var url_ung_vien = browser.GetAttribute(item, "href");
                                            current_href.Add(url_ung_vien);
                                        }
                                        foreach (var items in current_href)
                                        {
                                            browser.GoTo(items);
                                            lst_cv_by_job.AddRange(XuLyCV(browser, job, items));

                                            job.tong_so_cv = lst_cv_by_job.Count;
                                        }
                                    }

                                    if (lst_new != null && lst_new.Count > 0)
                                    {
                                        sleep = 0;
                                    }
                                    job.tong_so_cv = lst_cv_by_job.Count;
                                    job.ngay_xu_ly = XMedia.XUtil.TimeInEpoch();
                                    job.trang_thai_xu_ly = TrangThaiXuLy.DA_XU_LY;
                                    lst_ung_vien.AddRange(lst_cv_by_job);
                                    ES.JobLinkRepository.Instance.UpdateTrangThaiXuLy(job);
                                }
                            }
                        }
                    }
                }
            }

            return new List<UngVien>(lst_ung_vien);
        }

        public List<UngVien> XuLyCV(XBrowser browser, JobLink job, string url_ung_vien)
        {
            job.trang_thai_xu_ly = TrangThaiXuLy.DA_XU_LY;

            List<UngVien> lst_ung_vien = new List<UngVien>();
            try
            {
                //var eles_ung_vien = browser.Find("//table[@class='el-table__body']//tbody//div/div/div[1]/a");
                //if (eles_ung_vien.Count <= 0)
                //{
                //    job.thong_tin_xu_ly = Common.KHONG_TIM_THAY_UNG_VIEN;
                //}

                try
                {
                    UngVien ung_vien = new UngVien();
                    ung_vien.vi_tri = job.ten_job;
                    ung_vien.ngay_tao = ung_vien.ngay_sua = XMedia.XUtil.TimeInEpoch();
                    ung_vien.app_id = job.app_id;
                    ung_vien.job_link = job.link;
                    ung_vien.ho_ten = browser.GetInnerText("//div[@class='panel-body']//h5/span", 2000);
                    ung_vien.link_cv_online = browser.GetAttribute("//div[@class='panel-footer']//div[@class='pull-right']//a", "href");
                    ThongTinChungUngVien ttuv = new ThongTinChungUngVien();
                    ttuv.domain = "mywork.com";
                    ung_vien.thong_tin_chung = ttuv;
                    //Bóc tách XPATH để lấy được thông tin này nếu có
                    ung_vien.kinh_nghiem = browser.GetInnerText("//div[@class='list-item']");
                    ung_vien.ky_nang = browser.GetInnerText("//div[@class='panel-body']/div[7]");
                    ung_vien.so_dien_thoai = browser.GetInnerText("//div[@class='info']//div/div[2]/p[2]/span");
                    ung_vien.email = browser.GetInnerText("//div[@class='info']//div[2]/p[1]/a");
                    Uri uri = new Uri(url_ung_vien);
                    ung_vien.domain = uri.Host;
                    ung_vien.custom_id = url_ung_vien.Split('/').Last();
                    ung_vien.hoc_van = "";
                    ung_vien.ngay_sinh = 0;
                    //Tai file
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
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