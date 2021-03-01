using log4net;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UngVienJobModel;
using UngVienJobUI.Utils;
using UVJHelper;
using XMedia;

namespace UngVienJobUI.Site
{public class TopCvPost
    {
        private string user_profile_path = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\\profiles";
        private bool is_authenticated = false;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TopCvPost));

        public bool ExtractThongTin(CauHinh ch, string link, string app_id, out string msg, UngVienJobModel.ChiTietTinModel ct, bool is_debug = false)
        {
            _logger.Info("START POST TOPCV");
            is_authenticated = false;
            msg = "";
            //string username = XUtil.ConfigurationManager.AppSetting["TopCv:username"];
            //string password = XUtil.ConfigurationManager.AppSetting["TopCv:password"];
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
            {
                browser.GoTo(ch.url_login);
                var login_form = browser.Find(".//div[@id='page-login']");
                if (login_form.Count >= 1)
                {
                    foreach (var element in login_form)
                    {
                        try
                        {
                            element.FindElement(By.XPath(ch.xpath_username)).SendKeys(ch.username);
                            element.FindElement(By.XPath(ch.xpath_password)).SendKeys(XUtil.DecodeToken(ch.password));
                            element.FindElement(By.XPath(ch.xpath_password)).SendKeys(Keys.Enter);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    _logger.Error("LOGIN TOPCV FAILED OR WAS LOGIN!");
                }

                browser.GoTo(link);

                if (browser.GetUrl() == link)
                {
                    _logger.Info("LOGIN TOPCV SUCCESS");
                    is_authenticated = true;
                }

                //is_authenticated = link != url ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out msg) : browser.GetUrl().Contains(url);

                if (is_authenticated)
                {
                    
                    var parent = browser.Find(".//div[@class='panel form-job panel-light']");
                    if (parent != null)
                    {
                        try
                        {
                            foreach (var element in parent)
                            {
                                // chức danh
                                var send_nganh_nghe = element.FindElement(By.Id("jobTitle"));
                                if (send_nganh_nghe != null)
                                {
                                    send_nganh_nghe.SendKeys(ct.chuc_danh);
                                    _logger.Info($"da send nganh nghe {ct.chuc_danh}");
                                }
                                else
                                {
                                    _logger.Error($"khong tim thay xpath {ct.chuc_danh}");
                                }
                                System.Threading.Thread.Sleep(1000);
                                var choose_nganh_nghe =
                                    element.FindElement(By.XPath("//input[@id='jobTitle']/preceding-sibling::div"));
                                if (choose_nganh_nghe != null)
                                {
                                    choose_nganh_nghe.Click();
                                }
                                var nganh_nghe_topcv = browser.Find(".//input[@placeholder='Chọn ngành nghề']")?.First();
                                if (nganh_nghe_topcv != null)
                                {
                                    nganh_nghe_topcv.Click();
                                    _logger.Info("chon nganh nghe topcv");
                                    foreach (var work in ct.nganh_nghe)
                                    {
                                        System.Threading.Thread.Sleep(2000);
                                        var nganh_nghe = browser.Find($".//ul[@id='select2-categoryIds-results']/li[text()='{work.Trim()}']")?.First();
                                        if (nganh_nghe == null)
                                        {
                                            browser.FindAndClick(".//input[@placeholder='Chọn ngành nghề']");
                                            System.Threading.Thread.Sleep(2000);
                                            browser.Find($".//ul[@id='select2-categoryIds-results']/li[text()='{work}']")
                                                ?.First().Click();
                                            _logger.Info($"da chon nganh nghe {work}");
                                        }
                                        else
                                        {
                                            browser.FindAndClick(
                                                $".//ul[@id='select2-categoryIds-results']/li[text()='{work}']");
                                            _logger.Info($"da chon nganh nghe {work}");
                                        }
                                    }
                                }

                                // địa điểm
                                var dia_diem = element.FindElement(By.XPath(".//input[@placeholder='Chọn địa điểm']"));
                                if (dia_diem != null)
                                {
                                    dia_diem.Click();
                                    _logger.Info("chon dia diem topcv");
                                    System.Threading.Thread.Sleep(2000);
                                    var choose_dia_chi = browser.Find($".//ul[@id='select2-cityIds-results']/li[text()='{ct.dia_chi}']")?.First();
                                    if (choose_dia_chi != null)
                                    {
                                        choose_dia_chi.Click();
                                        _logger.Info($"da chon dia chi top cv {ct.dia_chi}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath dia chi");
                                    }
                                }

                                // số lượng
                                var so_luong = element.FindElement(By.Id("quantity"));
                                if (so_luong != null)
                                {
                                    so_luong.SendKeys(ct.so_luong_tuyen.ToString());
                                    _logger.Info($"da send so luong top cv {ct.so_luong_tuyen}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath so luong");
                                }
                                // cấp bậc
                                var cap_bac = element.FindElement(By.XPath("//select[@name='position_id']/following::span[@class='select2 select2-container select2-container--bootstrap'][1]"));
                                if (cap_bac != null)
                                {
                                    cap_bac.Click();
                                    System.Threading.Thread.Sleep(2000);
                                    var choose_cap_bac = browser.Find($".//span[@class='select2-container select2-container--bootstrap select2-container--open']//li[text()='{ct.cap_bac}']")?.First();
                                    if (choose_cap_bac != null)
                                    {
                                        choose_cap_bac.Click();
                                        _logger.Info($"da chon cap bac {ct.cap_bac}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath cap bac");
                                    }
                                }

                                // find textbox
                                var textbox = browser.Find(".//div[@class='note-editable panel-body']");
                                if (textbox.Count > 0)
                                {
                                    // mô tả công việc
                                    var mo_ta_cong_viec = textbox[0];
                                    if (mo_ta_cong_viec != null)
                                    {
                                        mo_ta_cong_viec.SendKeys(ct.mo_ta_cong_viec);
                                        _logger.Info($"da send mo ta cong viec {ct.mo_ta_cong_viec}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath mo ta cong viec");
                                    }
                                    // yêu cầu ứng viên
                                    var yeu_cau_ung_vien = textbox[1];
                                    if (yeu_cau_ung_vien != null)
                                    {
                                        yeu_cau_ung_vien.SendKeys(ct.yeu_cau_cong_viec);
                                        _logger.Info($"da send yeu_cau_cong_viec {ct.yeu_cau_cong_viec}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath yeu_cau_cong_viec");
                                    }
                                    var quyen_loi_ung_vien = textbox[2];
                                    if (quyen_loi_ung_vien != null)
                                    {
                                        quyen_loi_ung_vien.SendKeys(ct.quyen_loi_ung_vien);
                                        _logger.Info($"da send quyen_loi_ung_vien {ct.quyen_loi_ung_vien}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath quyen_loi_ung_vien");
                                    }
                                }

                                element.FindElement(By.Id("btn-submit")).Click();
                                System.Threading.Thread.Sleep(1500);
                                var yc_publish = browser.Find(".//div[@id='suggestPublishRequestModal']");
                                if (yc_publish.Count > 0)
                                {
                                    foreach (var item in yc_publish)
                                    {
                                        item.FindElement(By.XPath(".//button[@class='btn btn-primary']")).Click();
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                                System.Threading.Thread.Sleep(1000);
                                string link_post = "";

                                var find_title_new = browser.Find(".//tbody[@class='tbody-job']/tr[1]");
                                if (find_title_new.Count > 0)
                                {
                                    foreach (var title in find_title_new)
                                    {
                                        title.FindElement(By.XPath(".//button")).Click();
                                        var els = browser.Find(".//ul/li/a[contains(text(),'Xem CV ứng tuyển')]");
                                        foreach (var e in els)
                                        {
                                            link_post = browser.GetAttribute(e, "href");
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(link_post))
                                {
                                    List<JobLink> lst_saved = new List<JobLink>();
                                    JobLink saved = new JobLink(LoaiLink.JOB_LINK)
                                    {
                                        ten_job = ct.chuc_danh,
                                        app_id = app_id,
                                        link = link_post,
                                        trang_thai = TrangThai.DANG_SU_DUNG,
                                        trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                        nguoi_tao = ch.username
                                    };
                                    lst_saved.Add(saved);
                                    _logger.Info("POST TOPCV SUCCESS");
                                    return ES.JobLinkRepository.Instance.IndexMany(lst_saved) > 0;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                    else
                    {
                        _logger.Error("CAN'T FOUND FORM POST TOPCV");
                    }
                }
            }
            _logger.Error("POST TOPCV FAILED!");
            return false;
        }

        public void AutoUpdate(CauHinh ch)
        {
            var all_saved = ES.LinkSavedRepository.Instance.GetLinksByGiaTri(1, "https://topcv.vn/");
            if (all_saved.Count > 0)
            {
                using (var browser = new XBrowser())
                {
                    string url = browser.GoTo(ch.url_login);
                    var login_form = browser.Find(".//div[@class='panel-body']");
                    if (login_form.Count > 0)
                    {
                        try
                        {
                            foreach (var element in login_form)
                            {
                                element.FindElement(By.XPath(ch.xpath_username)).SendKeys(ch.username);
                                element.FindElement(By.XPath(ch.xpath_password)).SendKeys(ch.password);
                                element.FindElement(By.XPath(".//button")).Click();
                            }
                        }
                        catch
                        {
                        }
                    }
                    // top cv status
                    string showing = "https://tuyendung.topcv.vn/tin-tuyen-dung?status=showing";
                    string waitting = "https://tuyendung.topcv.vn/tin-tuyen-dung?status=waiting";
                    string not_publish = "https://tuyendung.topcv.vn/tin-tuyen-dung?status=not_publish";
                    string rejected = "https://tuyendung.topcv.vn/tin-tuyen-dung?status=rejected";

                    var list_link_status = new List<string> { showing, waitting, not_publish, rejected };

                    foreach (string link in list_link_status)
                    {
                        browser.GoTo(link);
                        foreach (var tin in all_saved)
                        {
                            var job = tin.ten_job.Remove(tin.ten_job.LastIndexOf(" ", StringComparison.Ordinal));
                            var find_new = browser.Find($".//a[@class='job-title']/strong[contains(text(),'{job}')]");
                            if (find_new.Count > 0)
                            {
                                string current_url = browser.GetUrl();
                                tin.thuoc_tinh = current_url != showing ? current_url != waitting ? current_url == not_publish ? 1 : 4 : 2 : 3;
                                ES.LinkSavedRepository.Instance.UpdateStatus(tin.id, tin.thuoc_tinh);
                            }
                        }
                    }
                }
            }
        }
    }
}