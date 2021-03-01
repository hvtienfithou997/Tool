using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UngVienJobModel;
using UVJHelper;
using XMedia;

namespace TopCVHelper
{
    public class TopCV
    {
        private string cv_save_path;
        private string user_profile_path;
        private bool is_authenticated = false;
        private bool show_browser = true;
        public TopCV(string _user_profile_path, string _cv_save_path)
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
                string wait_xpath = ".//span[@id='currentCredit']";
                is_authenticated = url.Contains(ch.url_login) ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out msg, wait_xpath, false) : browser.GetUrl().Contains(url);
                if (is_authenticated)
                {
                    browser.GoTo(link);
                    var eles = browser.Find("//tbody[@class='tbody-job']//td[3]/a");

                    if (eles.Count > 0)
                    {
                        foreach (var item in eles)
                        {
                            var url_tin = browser.GetAttribute(item, "href");
                            if (!string.IsNullOrEmpty(url_tin))
                            {
                                JobLink job = new JobLink(LoaiLink.JOB_LINK);
                                job.app_id = app_id;
                                job.link = url_tin;
                                job.trang_thai = TrangThai.DANG_SU_DUNG;
                                job.trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY;
                                job.nguoi_tao = ch.username;
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
                    if (url.Contains(ch.url_login))
                    {
                        is_authenticated = browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out string msg);
                    }
                    else
                    {
                        is_authenticated = browser.GetUrl().Contains(url);
                    }
                    if (is_authenticated)
                    {
                        string r_url = browser.GoTo(job.link);

                        if (r_url.Contains(job.link))
                        {
                            var lst_cv_by_job = XuLyCV(browser, job);
                            //Phân trang CV
                            ////ul[@class='pagination']//a
                            var cv_paging_eles = browser.Find("//ul[@class='pagination']//a");
                            if (cv_paging_eles.Count > 0)
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
                var eles_ung_vien = browser.Find("//div[@class='table-responsive']/table//td[2]/a");
                if (eles_ung_vien.Count > 0)
                {
                    foreach (var item_ung_vien in eles_ung_vien)
                    {
                        try
                        {
                            browser.Click(item_ung_vien);
                            UngVien ung_vien = new UngVien();
                            ung_vien.vi_tri = job.ten_job;
                            ung_vien.ngay_tao = ung_vien.ngay_sua = XMedia.XUtil.TimeInEpoch();
                            ung_vien.app_id = job.app_id;
                            ung_vien.job_link = job.link;
                            ung_vien.ho_ten = browser.GetInnerText("//div[@id='action-box']//tr[1]//td[1]", 2000);
                            ung_vien.link_cv_online = browser.GetAttribute("//a[@id='btn-download-candidate']", "href");
                            ThongTinChungUngVien ttuv = new ThongTinChungUngVien();
                            ttuv.domain = "topcv.vn";
                            ttuv.full_text = browser.GetInnerHtml("//div[@id='action-box']/table");
                            ung_vien.thong_tin_chung = ttuv;
                            //Bóc tách XPATH để lấy được thông tin này nếu có
                            ung_vien.kinh_nghiem = browser.GetInnerText("//div[@class='info']//div/div[1]/p[3]/strong");
                            ung_vien.ky_nang = "";
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
                            browser.FindAndClick("//div[@title='Đóng lại']");
                        }
                        catch (Exception)
                        {
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