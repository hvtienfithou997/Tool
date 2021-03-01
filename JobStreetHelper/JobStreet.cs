using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UngVienJobModel;
using UVJHelper;
using XMedia;

namespace JobStreetHelper
{
    public class JobStreet
    {
        private string cv_save_path;
        private string user_profile_path;
        private bool is_authenticated = false;

        private bool show_browser = true;
        //string default_path_download = $"{System.IO.Path.GetDirectoryName(Application.)}\\profiles\\test";
        //private string default_path_download = $"F:\\CUNL Helper\\UngVienJobUI\\bin\\Debug\\profiles\\downloads";

        public JobStreet(string _user_profile_path, string _cv_save_path)
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

                string wait_xpath = ".//a[@data-test-key='accountMenuItem']";
                is_authenticated = link != url ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out msg, wait_xpath, false) : browser.GetUrl().Contains(url);

                if (is_authenticated)
                {
                    browser.GoTo(link);

                    var eles = browser.Find("//div[@class='Dashboard__dashboard']//a[@class='JobCard__clickable']");

                    if (eles != null)
                    {
                        foreach (var item in eles)
                        {
                            var url_tin = browser.GetAttribute(item, "href");
                            var ten_job = item.FindElement(By.XPath(".//strong[@class='BoldedText__regular JobCard__jobTitle']")).Text;

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
                        string r_url = browser.GoTo(job.link);

                        if (r_url.Contains(job.link))
                        {
                            var lst_cv_by_job = XuLyCV(browser, job);

                            lst_ung_vien.AddRange(lst_cv_by_job);
                            job.tong_so_cv = lst_cv_by_job.Count;
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
                var eles_ung_vien = browser.Find("//div[@class='Card__root ApplicationCard__root']");
                List<string> lst_block_ung_vien = new List<string>();
                if (eles_ung_vien.Count > 0)
                {
                    foreach (var block_ung_vien in eles_ung_vien)
                    {
                        try
                        {
                            UngVien ung_vien = new UngVien();

                            ung_vien.ngay_tao = ung_vien.ngay_sua = XUtil.TimeInEpoch(DateTime.Now);
                            ung_vien.app_id = job.app_id;
                            ung_vien.job_link = job.link;
                            ung_vien.vi_tri = job.ten_job;
                            try
                            {
                                ung_vien.ho_ten = block_ung_vien.FindElement(By.ClassName("ApplicationCard__name")).Text;
                            }
                            catch (Exception)
                            {
                                ung_vien.ho_ten = "";
                            }

                            try
                            {
                                browser.FindFirst(".//div[@class='CardCopy__regularSpacing CardCopy__extraTopMargin']/a").Click();
                                System.Threading.Thread.Sleep(1500);
                                ung_vien.link_cv_online = block_ung_vien.FindElement(By.XPath(".//iframe[@class='DownloadResume__downloadFrame']")).GetAttribute("src");
                            }
                            catch (Exception)
                            {
                            }
                            ThongTinChungUngVien ttuv = new ThongTinChungUngVien();
                            ttuv.domain = "jobstreet.vn";
                            //ttuv.full_text = block_ung_vien.FindElement(By.XPath("//div[@class='Card__root ApplicationCard__root']")).;
                            ung_vien.thong_tin_chung = ttuv;

                            //Bóc tách XPATH để lấy được thông tin này nếu có

                            var is_exist_email = browser.FindFirst(".//div[@class='ApplicationCard__revealEmailAddress']/span/a");
                            var is_exist_phone = browser.FindFirst(".//div[@class='ApplicationCard__revealPhoneNumber']/span/a");

                            if ((is_exist_email != null && is_exist_email.Displayed == true) || (is_exist_phone != null && is_exist_phone.Displayed == true))
                            {
                                if (is_exist_email != null)
                                {
                                    is_exist_email.Click();
                                    System.Threading.Thread.Sleep(300);
                                    var element_email = block_ung_vien.FindElement(By.XPath(".//div[@class='ApplicationCard__revealEmailAddress']/a"));
                                    if (element_email != null && element_email.Displayed == true)
                                    {
                                        ung_vien.email = block_ung_vien.FindElement(By.XPath(".//div[@class='ApplicationCard__revealEmailAddress']/a")).Text;
                                    }
                                }
                                if (is_exist_phone != null)
                                {
                                    is_exist_phone.Click();
                                    System.Threading.Thread.Sleep(300);
                                    var element_phone = browser.FindFirst(".//div[@class='ApplicationCard__revealPhoneNumber']/span/a");

                                    if (element_phone != null && element_phone.Displayed == true)
                                    {
                                        ung_vien.so_dien_thoai = block_ung_vien.FindElement(By.XPath("//div[@class='ApplicationCard__revealPhoneNumber']")).Text;
                                    }
                                }
                            }
                            ung_vien.ky_nang = "";
                            ung_vien.hoc_van = "";
                            ung_vien.ngay_sinh = 0;
                            Uri uri = new Uri(job.link);
                            ung_vien.domain = uri.Host;
                            var id = string.Join("/", job.link.Split('/').Skip(5));
                            id = id.Substring(0, id.LastIndexOf('/'));
                            ung_vien.custom_id = id;
                            //Tai file
                            //browser.DownloadByBrowser(block_ung_vien.FindElement(By.XPath(".//div[@class='CardCopy__regularSpacing CardCopy__extraTopMargin']/a")));
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
                            //browser.FindAndClick("//div[@title='Đóng lại']");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
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