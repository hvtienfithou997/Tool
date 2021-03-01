using log4net;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UngVienJobModel;
using UngVienJobUI.Utils;
using UVJHelper;

namespace UngVienJobUI.Site
{
    public class CareerBuilderPost
    {
        private bool is_authenticated = false;
        private string user_profile_path = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\profiles";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CareerBuilderPost));

        public bool ExtractThongTin(CauHinh ch, string link, UngVienJobModel.ChiTietTinModel ct, string app_id, out string msg, bool is_debug = false)
        {
            msg = "";
            is_authenticated = false;
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
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

                browser.GoTo(link);
                if (is_authenticated)
                {
                    _logger.Info("LOGIN CAREER BUILDER SUCCESS");
                    try
                    {
                        var form_post = browser.Find("//div[@id='tab-postjob-description']");
                        if (form_post.Count > 0)
                        {
                            foreach (var element in form_post)
                            {
                                var chuc_danh = element.FindElement(By.Id("job_title"));
                                if (chuc_danh != null)
                                {
                                    chuc_danh.SendKeys(ct.chuc_danh);
                                    _logger.Info("da sendkeys chuc danh");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath chuc danh");
                                }

                                var mo_ta_cv =
                                    element.FindElement(By.XPath(".//iframe[@title='Rich Text Editor, job_desc']"));
                                if (mo_ta_cv != null)
                                {
                                    mo_ta_cv.SendKeys(ct.mo_ta_cong_viec);
                                    _logger.Info("da sendkeys mo ta cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath mo ta cong viec");
                                }

                                var yeu_cau_cv =
                                    element.FindElement(By.XPath(".//iframe[@title='Rich Text Editor, job_req']"));

                                if (yeu_cau_cv != null)
                                {
                                    yeu_cau_cv.SendKeys(ct.yeu_cau_cong_viec);
                                    _logger.Info("da sendkeys yeu cau cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath yeu cau cong viec");
                                }

                                var nganh_nghe = element.FindElement(
                                    By.XPath("//div[@class='fl_left width_202 box_multiSelect_industry']/button"));
                                if (nganh_nghe != null)
                                {
                                    nganh_nghe.Click();
                                    foreach (var nn in ct.nganh_nghe)
                                    {
                                        //browser.FindAndClick($"//li/label/span[text()='{nn}']/preceding-sibling::input");
                                        var find_nn = browser
                                            .Find($"//li/label/span[text()='{nn}']/preceding-sibling::input")?.First();
                                        if (find_nn != null)
                                        {
                                            find_nn.Click();
                                            _logger.Info($"da chon nganh nghe {nn}");
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath nganh nghe");
                                }

                                //
                                // địa chỉ

                                var dia_chi = browser
                                    .Find(
                                        $"//select[@name='LOCATION_ID[]']/optgroup[@label='Việt Nam']/option[text()='{ct.dia_chi}']")
                                    ?.First();
                                if (dia_chi != null)
                                {
                                    dia_chi.Click();
                                    _logger.Info($"da chon dia chi {ct.dia_chi}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay dia chi");
                                }

                                var btn_add_work =
                                    browser.Find("//a[@onclick='addWorkLocation();']")?.First();

                                if (btn_add_work != null)
                                {
                                    btn_add_work.Click();

                                    var ins_dia_chi = browser
                                        .Find($"//select[@id='location_id']/option[text()='{ct.dia_chi}']")?.First();
                                    if (ins_dia_chi != null)
                                    {
                                        ins_dia_chi.Click();
                                        _logger.Info($"da chon dia chi moi la {ct.dia_chi}");
                                        var ins_district = browser
                                            .Find($"//select[@id='sldistrict']/option[text()='{ct.district}']")
                                            ?.First();
                                        if (ins_district != null)
                                        {
                                            ins_district.Click();
                                            _logger.Info($"da chon quan {ct.district}");
                                        }
                                        else
                                        {
                                            _logger.Info($"khong tim thay quan {ct.district}");
                                        }
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay dia chi {ct.dia_chi}");
                                    }

                                    foreach (var address in browser.Find("//input[@id='address']"))
                                    {
                                        address.SendKeys(ct.dia_chi_chi_tiet);
                                    }

                                    var save_address = browser.Find("//input[@value='Lưu']")?.First();
                                    if (save_address != null)
                                    {
                                        save_address.Click();
                                        _logger.Info("da luu dia chi moi tao");
                                    }
                                }
                                else
                                {
                                    _logger.Error("khong tim thay button tao dia chi");
                                }

                                System.Threading.Thread.Sleep(3000);
                                var dia_diem_lam_viec = browser.Find("//input[@value='Địa điểm làm việc']")?.First();
                                if (dia_diem_lam_viec != null)
                                {
                                    dia_diem_lam_viec.Click();
                                    System.Threading.Thread.Sleep(3000);
                                    var detail = browser
                                        .Find($".//ul[@class='chosen-results']/li[text()='{ct.dia_chi_chi_tiet}']")
                                        ?.First();
                                    if (detail != null)
                                    {
                                        detail.Click();
                                        _logger.Info($"da chon dia chi lam viec: {ct.dia_chi_chi_tiet}");
                                    }
                                    else
                                    {
                                        browser.FindAndClick("//input[@value='Địa điểm làm việc']");
                                        System.Threading.Thread.Sleep(3000);
                                        detail.Click();
                                        _logger.Info($"da chon lai dia chi lam viec {ct.dia_chi_chi_tiet}");
                                    }

                                    _logger.Info($"da chon dia diem lam viec {ct.dia_chi_chi_tiet}");
                                }

                                char[] cut_luong = { ',', '/', '-', ' ' };
                                var arr_luong = ct.muc_luong.Split(cut_luong, StringSplitOptions.RemoveEmptyEntries);
                                var luong_from = arr_luong[0];
                                var luong_to = arr_luong[1];

                                var luong_f = element.FindElement(By.Id("salary_from"));
                                if (luong_f != null)
                                {
                                    luong_f.SendKeys(luong_from + "000000");
                                    _logger.Info("da sendkeys luong from");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath luong from");
                                }

                                var luong_t = element.FindElement(By.Id("salary_to"));
                                if (luong_t != null)
                                {
                                    luong_t.SendKeys(luong_to + "000000");
                                    _logger.Info("da sendkeys luong to");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath luong from");
                                }

                                // hình thức làm việc
                                var hinh_thuc_lam_viec = browser
                                    .Find(
                                        $"//div/label/span[text()='{ct.loai_hinh_cong_viec}']/preceding-sibling::input")
                                    ?.First();
                                if (hinh_thuc_lam_viec != null)
                                {
                                    hinh_thuc_lam_viec.Click();
                                    _logger.Info($"da sendkeys hinh thuc lam viec {ct.loai_hinh_cong_viec}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath hinh thuc lam viec");
                                }

                                // hạn nhận hồ sơ

                                browser.removeAttrById("JOB_LASTDATE", "readonly");
                                //element.FindElement(By.Id("JOB_LASTDATE"))
                                //    .SendKeys(DateTime.Now.Date.AddMonths(1).ToString("dd/MM/yyyy"));
                                var deadline = element.FindElement(By.Id("JOB_LASTDATE"));
                                if (deadline != null)
                                {
                                    deadline.SendKeys(DateTime.Now.Date.AddMonths(1).ToString("dd/MM/yyyy"));
                                    _logger.Info("da sendkeys han nhan ho so");
                                }
                                else
                                {
                                    _logger.Info("khong tim thay xpath han nhan ho so");
                                }

                                // kinh nghiệm  JOB_ISEXPERIENCE
                                //browser.FindAndClick(
                                //    $"//select[@id='JOB_ISEXPERIENCE']/option[text()='{ct.yeu_cau_kinh_nghiem}']");
                                var kinh_nghiem = browser
                                    .Find($"//select[@id='JOB_ISEXPERIENCE']/option[text()='{ct.yeu_cau_kinh_nghiem}']")
                                    ?.First();
                                if (kinh_nghiem != null)
                                {
                                    kinh_nghiem.Click();
                                    _logger.Info($"da chon kinh nghiem {ct.yeu_cau_kinh_nghiem}");
                                    if (ct.yeu_cau_kinh_nghiem == "Có kinh nghiệm")
                                    {
                                        if (!string.IsNullOrEmpty(ct.kinh_nghiem_from) &&
                                            !string.IsNullOrEmpty(ct.kinh_nghiem_to))
                                        {
                                            var min_year_exp = browser.Find("//input[@id='JOB_FROMEXPERIENCE']");
                                            foreach (var min in min_year_exp)
                                            {
                                                min.SendKeys(Keys.Control + "a");
                                                min.SendKeys(ct.kinh_nghiem_from);
                                            }

                                            var max_year_exp = browser.Find("//input[@id='JOB_TOEXPERIENCE']");
                                            foreach (var max in max_year_exp)
                                            {
                                                max.SendKeys(Keys.Control + "a");
                                                max.SendKeys(ct.kinh_nghiem_to);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath kinh nghiem lam viec");
                                }

                                // cấp bậc làm việc  LEVEL_ID
                                var level = browser.Find($"//select[@id='LEVEL_ID']/option[text()='{ct.cap_bac}']")
                                    ?.First();
                                if (level != null)
                                {
                                    level.Click();
                                    _logger.Info("da chon cap bac lam viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath cap bac lam viec");
                                }

                                //input[@value='Lưu và Tiếp tục']
                                browser.FindAndClick("//input[@value='Lưu và Tiếp tục']");
                                _logger.Info("da luu va tiep tuc buoc 2");

                            }

                            var save = browser.Find(
                                "//div[@id='tab-postjob-contactinformation']//div[@class='btn_submit']//input")?.First();
                            if (save != null)
                            {
                                save.Click();
                                _logger.Info("da click nut submit");
                            }
                            else
                            {
                                _logger.Error("khong tim thay nut submit");
                            }

                            System.Threading.Thread.Sleep(3000);
                            
                            var post_job = browser.Find("//div[@id='tab-postjob-matchingscore']//div[@class='btn_submit']//input")?.First();
                            if (post_job != null)
                            {
                                post_job.Click();
                                _logger.Info("post job thanh cong");
                            }
                            System.Threading.Thread.Sleep(3000);
                            var url_tin = browser.GetUrl();
                            _logger.Info($"lay url job thanh cong: {url_tin}");
                            List<JobLink> lst_saved = new List<JobLink>();
                            JobLink saved = new JobLink(LoaiLink.JOB_LINK)
                            {
                                ten_job = ct.chuc_danh,
                                app_id = app_id,
                                link = url_tin,
                                trang_thai = TrangThai.DANG_SU_DUNG,
                                trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                nguoi_tao = ch.username
                            };

                            lst_saved.Add(saved);
                            _logger.Info("post tin careerbuilder thanh cong");
                            return ES.JobLinkRepository.Instance.IndexMany(lst_saved) > 0;
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }

            return false;
        }
    }
}