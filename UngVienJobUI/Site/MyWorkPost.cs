using log4net;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UngVienJobModel;
using UVJHelper;

namespace UngVienJobUI.Site
{
    public class MyWorkPost
    {
        private string user_profile_path = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\profiles";
        private bool is_authenticated = false;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MyWorkPost));
        //public MyWorkPost()
        //{
        //}

        public bool ExtractThongTin(CauHinh ch, string link, string app_id, out string msg, UngVienJobModel.ChiTietTinModel ct, bool is_debug = false)
        {
            _logger.Info("START POST MYWORK");
            is_authenticated = false;
            msg = "";
            //string username = XUtil.ConfigurationManager.AppSetting["MyWork:username"];
            //string password = XUtil.ConfigurationManager.AppSetting["MyWork:password"];
            
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
            {
                browser.GoTo(ch.url_login);
                var login_form = browser.Find(".//form[@class='form-hook']");
                if (login_form.Count > 0)
                {
                    try
                    {
                        foreach (var element in login_form)
                        {
                            element.FindElement(By.XPath(ch.xpath_username)).SendKeys(ch.username);
                            element.FindElement(By.XPath(ch.xpath_password)).SendKeys(ch.password);
                            //element.FindElement(By.XPath(".//button[@class='el-button full-width el-button--primary']")).Click();
                            element.FindElement(By.XPath(ch.xpath_password)).SendKeys(Keys.Enter);
                            //wait element nao do xuat hien
                            _logger.Info("LOGIN MYWORK SUCCESS");
                            Thread.Sleep(4000);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                browser.GoTo(link);
                if (browser.GetUrl() == link)
                {
                    System.Threading.Thread.Sleep(2000);
                    //is_authenticated = link != url ? browser.Login(ch.url_login, ch.username, ch.password, ch.xpath_username, ch.xpath_password, out msg) : browser.GetUrl().Contains(url);
                    is_authenticated = true;
                    _logger.Info("LOGIN MYWORK SUCCESS");
                }

                if (is_authenticated)
                {
                    //Thông tin công việc
                    try
                    {
                        var thong_tin_cong_viec = browser.Find(".//div[@class='create-cnt-box'][1]");
                        if (thong_tin_cong_viec != null)
                        {
                            foreach (var element in thong_tin_cong_viec)
                            {
                                
                                var chuc_danh = element.FindElement(By.XPath(
                                    ".//input[@name='title']"));
                                if (chuc_danh != null)
                                {
                                    chuc_danh.SendKeys(ct.chuc_danh);
                                    _logger.Info($"da send chuc danh {ct.chuc_danh}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath chuc danh");
                                }

                                // số lượng tuyển
                                var so_luong = element.FindElement(By.XPath(".//input[@name='vacancy_quantity']"));
                                so_luong.SendKeys(Keys.Control + "a");
                                so_luong.SendKeys(Keys.Delete);
                                so_luong?.SendKeys(ct.so_luong_tuyen.ToString());

                                var cap_bac =
                                        element.FindElement(By.XPath("div[2]/div/div[5]/div/div[2]/div/div/span/span"));
                                if (cap_bac != null)
                                {
                                    cap_bac.Click();
                                    _logger.Info($"da send cap bac : {ct.cap_bac}");
                                    System.Threading.Thread.Sleep(2000);
                                    string str_vi_tri = ct.cap_bac;
                                    char[] spearator0 = { ',', '/', '-', ' ' };
                                    foreach (string s in str_vi_tri.Split(spearator0, StringSplitOptions.None))
                                    {
                                        if (!string.IsNullOrWhiteSpace(s))
                                        {
                                            var industry = browser.Find($".//div/div[contains(text(),'{s}')]");
                                            if (industry.Count > 0)
                                            {
                                                foreach (var item in industry)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(item.Text))
                                                    {
                                                        item.Click();
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //

                                // loại hình công việc
                                var loai_hinh =
                                    element.FindElement(By.XPath("div[2]/div/div[6]/div/div[2]/div/div/span/span"));
                                if (loai_hinh != null)
                                {
                                    loai_hinh.Click();
                                    System.Threading.Thread.Sleep(1000);
                                    var choose_loai_hinh = browser.Find($".//div/div[contains(text(),'{ct.loai_hinh_cong_viec}')]");
                                    if (choose_loai_hinh != null)
                                    {
                                        foreach (var choose in choose_loai_hinh)
                                        {
                                            if (!string.IsNullOrWhiteSpace(choose.Text))
                                            {
                                                choose.Click();
                                                break;
                                            }
                                        }
                                        _logger.Info($"da chon loai hinh cong viec: {ct.loai_hinh_cong_viec}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath loai hinh cong viec");
                                    }
                                }

                                // mức lương
                                var muc_luong = element.FindElement(By.XPath("div[2]/div/div[7]/div/div[2]/div/div/span/span"));
                                if (muc_luong != null)
                                {
                                    muc_luong.Click();
                                    _logger.Info($"chon muc luong");
                                    System.Threading.Thread.Sleep(1000);
                                    var choose_muc_luong = browser.Find($".//div/div[text()='{ct.muc_luong}']")?.First();
                                    if (choose_muc_luong != null)
                                    {
                                        choose_muc_luong.Click();
                                        _logger.Info($"da chon muc luong {ct.muc_luong}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath, khong tim thay muc luong phu hop");
                                    }
                                }

                                // địa điểm làm việc
                                var dia_diem = element.FindElement(By.XPath("div[2]/div/div[9]/div/div[2]/div/div/span/span"));

                                if (dia_diem != null)
                                {
                                    dia_diem.Click();
                                    System.Threading.Thread.Sleep(1000);
                                    var choose_dia_diem = browser.Find($".//div/div[contains(text(),'{ct.dia_chi}')]")?.First();
                                    if (choose_dia_diem != null)
                                    {
                                        choose_dia_diem.Click();
                                        _logger.Info($"da chon dia diem {ct.dia_chi}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath dia diem");
                                    }
                                }

                                // ngành nghề
                                var nganh_nghe = element.FindElement(By.XPath("div[2]/div/div[10]/div/div[2]/div/div/span/span"));
                                if (nganh_nghe != null)
                                {
                                    nganh_nghe.Click();
                                    _logger.Info("chon nganh nghe phu hop");
                                    System.Threading.Thread.Sleep(1000);
                                    foreach (string nghe in ct.nganh_nghe)
                                    {
                                        var nn = browser.Find($".//div/div[text()='{nghe}']")?.First();
                                        if (nn != null)
                                        {
                                            nn.Click();
                                            _logger.Info($"da chon nganh nghe {nn}");
                                        }
                                        else
                                        {
                                            _logger.Error($"khong tim thay xpath nganh nghe {nn}]");
                                        }
                                    }
                                }

                                // mô tả công việc
                                var mo_ta = element.FindElement(By.XPath(".//textarea[@name='description']"));
                                if (mo_ta != null)
                                {
                                    mo_ta.SendKeys(ct.mo_ta_cong_viec);
                                    _logger.Info($"da send mo ta cong viec {ct.mo_ta_cong_viec}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath mo ta cong viec");
                                }

                                var quyen_loi = element.FindElement(By.XPath(".//textarea[@name='benefit']"));
                                if (quyen_loi != null)
                                {
                                    quyen_loi.SendKeys(ct.quyen_loi_ung_vien);
                                    _logger.Info($"da send quyen loi ung vien {ct.quyen_loi_ung_vien}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath quyen_loi_ung_vien");
                                }
                            }

                            // Yêu cầu công việc
                            var yeu_cau_cong_viec = browser.Find(".//div[@class='create-cnt-box'][2]");
                            if (yeu_cau_cong_viec != null)
                            {
                                foreach (var element in yeu_cau_cong_viec)
                                {
                                    //kinh nghiem lam viec
                                    var ele_kn = element.FindElement(By.XPath("div[2]/div/div[1]/div/div[2]/div/div/span/span"));
                                    if (ele_kn != null)
                                    {
                                        ele_kn.Click();
                                        if (!string.IsNullOrEmpty(ct.yeu_cau_kinh_nghiem))
                                        {
                                            var choose_kinh_nghiem = browser.Find($".//div/div[contains(text(),'{ct.yeu_cau_kinh_nghiem}')]")?.First();
                                            choose_kinh_nghiem?.Click();
                                            _logger.Info($"da send yeu cau kinh nghiem {ct.yeu_cau_kinh_nghiem}");
                                        }
                                        else
                                        {
                                            browser.ScrollIntoView(ele_kn);
                                            browser.SetAttribute(ele_kn, "readonly", "");
                                            ele_kn.SendKeys("Đã có nhiều kinh nghiệm trong nghề");
                                            _logger.Info($"da send yeu cau kinh nghiem {ct.yeu_cau_kinh_nghiem}");
                                        }
                                    }

                                    // bang cap
                                    var ele_bcap = element.FindElement(By.XPath("div[2]/div/div[2]/div/div[2]/div/div/span/span"));
                                    if (ele_bcap != null)
                                    {
                                        ele_bcap.Click();
                                        
                                        var choose_bang_cap = browser.Find($".//div/div[contains(text(),'{ct.bang_cap}')]")?.First();
                                        choose_bang_cap?.Click();
                                        _logger.Info($"da send bang cap {ct.bang_cap}");
                                    }
                                    // giới tính
                                    var ele_gtinh = element.FindElement(By.XPath("div[2]/div/div[3]/div/div[2]/div/div/span/span"));
                                    if (ele_gtinh != null)
                                    {
                                        ele_gtinh.Click();
                                        
                                        var choose_gender = browser
                                            .Find($".//div/div[contains(text(),'{ct.gioi_tinh}')]")?.First();
                                        choose_gender?.Click();
                                        _logger.Info($"da send gioi tinh {ct.gioi_tinh}");
                                    }

                                    // hạn nộp hồ sơ

                                    var date_now = DateTime.Today.AddMonths(1).ToString("dd/MM/yyyy");

                                    var han_nop_hs =
                                        element.FindElement(By.XPath("div[2]/div/div[4]/div/div[2]/div/div//input"));
                                    if (han_nop_hs != null)
                                    {
                                        han_nop_hs.SendKeys(Keys.Control + "a");
                                        han_nop_hs.SendKeys(Keys.Delete);
                                        han_nop_hs.SendKeys(date_now);
                                        _logger.Info($"da send han nop ho so {date_now}");
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay xpath han nop ho so");
                                    }

                                    // ngon ngu ho so

                                    var ngon_ngu_ho_so = element.FindElement(By.XPath("div[2]/div/div[5]/div/div[2]/div/div/span/span"));
                                    if (ngon_ngu_ho_so != null)
                                    {
                                        ngon_ngu_ho_so.Click();
                                        var choose_ngon_ngu = browser.Find($".//div/div[contains(text(),'Tiếng Việt')]")?.First();
                                        choose_ngon_ngu?.Click();
                                        _logger.Info($"da send gioi tinh {ct.gioi_tinh}");
                                    }

                                    // yeu cau cong viec
                                    var yc = element.FindElement(By.XPath(".//textarea[@name='job_requirement']"));
                                    if (yc != null)
                                    {
                                        yc.SendKeys(ct.yeu_cau_cong_viec);
                                        _logger.Info($"da send yeu cau cong viec {ct.yeu_cau_cong_viec}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath yeu cau cong viec");
                                    }
                                }
                            }

                            //Thông tin liên hệ(Bắt buộc)
                            var thong_tin_lien_he = browser.Find(".//div[@class='create-cnt-box'][3]");
                            if (thong_tin_lien_he != null)
                            {
                                foreach (var element in thong_tin_lien_he)
                                {
                                    // ten nguoi lien he 
                                    var nguoi_lien_he = element.FindElement(By.XPath(".//input[@name='contact_name']"));
                                    if (nguoi_lien_he != null)
                                    {
                                        //IWebElement textBox = nguoi_lien_he;
                                        //textBox.SendKeys(Keys.Control + "a");
                                        nguoi_lien_he.SendKeys(ct.lien_he);
                                        _logger.Info($"da send nguoi lien he {ct.lien_he}");
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay xpath lien he");
                                    }

                                    // dia chi lien he
                                    var dia_chi_lien_he =
                                        element.FindElement(By.XPath(".//input[@name='contact_address']"));

                                    if (dia_chi_lien_he != null)
                                    {
                                        //IWebElement textBox = nguoi_lien_he;
                                        //textBox.SendKeys(Keys.Control + "a");
                                        dia_chi_lien_he.SendKeys(ct.dia_chi_chi_tiet);
                                        _logger.Info($"da send dia_chi lien he {ct.dia_chi_chi_tiet}");
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay xpath lien he");
                                    }

                                    //so dien thoai lien he
                                    var sdt_lien_he = element.FindElement(By.XPath(".//input[@name='contact_phone']"));
                                    if (sdt_lien_he != null)
                                    {
                                        sdt_lien_he.SendKeys(Keys.Control + "a");
                                        sdt_lien_he.SendKeys(ct.so_dien_thoai);
                                        _logger.Info($"da send so dien thoai {ct.so_dien_thoai}");
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay xpath so dien thoai");
                                    }

                                    // email lien he

                                    var email_lien_he =
                                        element.FindElement(By.XPath(".//input[@name='contact_email']"));
                                    if (sdt_lien_he != null)
                                    {
                                       email_lien_he.SendKeys(ct.email);
                                       _logger.Error($"da sendkey email");
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay email");
                                    }
                                }
                            }
                            
                            //var dich_vu =
                            //    element.FindElement(
                            //        By.XPath(".//input[@placeholder='Chọn dịch vụ cho tin đăng']"));
                            //dich_vu?.Click();

                           

                            
                                    
                            browser.FindAndClick(".//button[@class='btn btn-orange-56 ex-upload']");
                            _logger.Info("POST MYWORK SUCCESS");
                            Thread.Sleep(5000);
                            
                            string url_home = browser.GetUrl();

                            if (url_home == "https://mywork.com.vn/nha-tuyen-dung/tao-tin-tuyen-dung-thanh-cong")
                            {

                                browser.FindAndClick(".//a[@class='btn btn-orange-46 w370 w-100-mb font17']");
                                Thread.Sleep(1000);
                                if (browser.GetUrl() == "https://mywork.com.vn/nha-tuyen-dung/quan-ly-tin-dang?")
                                {
                                    var els = browser.Find($".//div[@class='jobslist-01-cont']//a[text()='{ct.chuc_danh}']");

                                    string link_post = "";
                                    foreach (var r_url in els)
                                    {
                                        link_post = browser.GetAttribute(r_url, "href");
                                        break;
                                    }
                                    string id = link_post.Replace("https://mywork.com.vn/nha-tuyen-dung/xem-truoc-tin-dang?id=", "").Split('/').First();

                                    if (!string.IsNullOrEmpty(link_post))
                                    {
                                        List<JobLink> lst_saved = new List<JobLink>();
                                        JobLink saved = new JobLink(LoaiLink.JOB_LINK)
                                        {
                                            ten_job = ct.chuc_danh,
                                            app_id = app_id,
                                            link = $"https://mywork.com.vn/nha-tuyen-dung/ho-so-da-ung-tuyen?job_id={id}",
                                            trang_thai = TrangThai.DANG_SU_DUNG,
                                            trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                            nguoi_tao = ch.username
                                        };
                                        lst_saved.Add(saved);
                                        _logger.Info("POST MYWORK SUCCESS!");
                                        return ES.JobLinkRepository.Instance.IndexMany(lst_saved) > 0;
                                    }
                                }
                               
                            }
                            else
                            {
                                _logger.Error("POST MY WORK FAILED! CAN'T GET LINK POST OR JOB NAME IS EXISTED");
                                return false;
                            }
                        }

                        //else
                        //{
                        //    _logger.Error("CAN'T FOUND FORM POST MYWORK");
                        //}
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }

            _logger.Error("POST MYWORK FAILED!");
            return false;
        }

        public void AutoUpdate()
        {
            var all_saved = ES.LinkSavedRepository.Instance.GetLinksByGiaTri(1, "https://mywork.com.vn/");
            if (all_saved.Count > 0)
            {
                using (var browser = new XBrowser())
                {
                    foreach (var item in all_saved)
                    {
                        browser.GoTo(item.domain);
                        System.Threading.Thread.Sleep(500);
                        var find = browser.Find(".//div[contains(text(),'Tin tuyển dụng này đã được nhà tuyển dụng khóa trước đó.')]");
                        item.thuoc_tinh = find.Count > 0 ? 2 : 5;
                        ES.LinkSavedRepository.Instance.UpdateStatus(item.id, item.thuoc_tinh);
                    }
                }
            }
        }
    }
}