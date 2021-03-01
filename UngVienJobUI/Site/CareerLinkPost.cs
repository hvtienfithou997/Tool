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
{
    public class CareerLinkPost
    {
        //private string cv_save_path;
        private string user_profile_path = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\profiles";

        private bool is_authenticated = false;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CareerLinkPost));
        //public CareerLinkPost(string _user_profile_path)
        //{
        //    user_profile_path = _user_profile_path;
        //}

        public bool ExtractThongTin(CauHinh ch, string link, string app_id, UngVienJobModel.ChiTietTinModel ct, out string msg, bool is_debug = false)
        {
            _logger.Info("START POST CAREER LINK");
            msg = "";
            is_authenticated = false;
            //string username = XUtil.ConfigurationManager.AppSetting["CareerLink:username"];
            //string password = XUtil.ConfigurationManager.AppSetting["CareerLink:password"];
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
            {
                string url = browser.GoTo(ch.url_login);
                var login_form = browser.Find(".//form[@id='login-check-form']");
                if (login_form.Count >= 1)
                {
                    try
                    {
                        foreach (var element in login_form)
                        {
                            element.FindElement(By.XPath(ch.xpath_username)).SendKeys(ch.username);
                            element.FindElement(By.XPath(ch.xpath_password)).SendKeys(XUtil.DecodeToken(ch.password));
                            element.FindElement(By.XPath(".//input[@name='btnCompanyLogin']")).Click();
                            _logger.Info("LOGIN CAREER LINK SUCCESS");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
                else
                {
                    _logger.Info("THIS SITE WAS LOGIN OR NOT FOUND");
                }

                browser.GoTo(link);

                is_authenticated = true;

                if (is_authenticated)
                {
                    _logger.Info("LOGIN CAREER LINKS SUCCESS");
                    try
                    {
                        _logger.Info("START POST STEP 1");
                        // step 1
                        var job_step_1 = browser.Find(".//form[@id='job-step1-form']");
                        if (job_step_1.Count > 0)
                        {
                            foreach (var step_1 in job_step_1)
                            {
                                var chuc_danh = step_1.FindElement(By.Id("jobStep1_name"));
                                if (chuc_danh != null)
                                {
                                    chuc_danh.SendKeys(ct.chuc_danh);
                                    _logger.Info("da sendkeys chuc danh");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath chuc danh");
                                }
                                var next_step = step_1.FindElement(By.XPath(".//input[@value='Lưu Bước 1 để tiếp tục Bước 2']"));
                                if (next_step != null)
                                {
                                    next_step.Click();
                                    _logger.Info("luu thanh cong buoc 1 de sang buoc 2");
                                }
                                else
                                {
                                    _logger.Error("khong sang dc buoc 2");
                                }
                            }
                        }
                        _logger.Info("START POST STEP 2");
                        // Step 2
                        var job_step_2 = browser.Find(".//form[@id='job-step2']");
                        if (job_step_2.Count > 0)
                        {
                            foreach (var step_2 in job_step_2)
                            {
                                // mô tả công việc
                                if (ct.nganh_nghe.Count >= 1)
                                {
                                    int count = 1;
                                    foreach (string item in ct.nganh_nghe)
                                    {
                                        var nganh_nghe = browser.Find($".//select[@id='jobStep2_category{count}']/option[contains(text(),'{item}')]")?.First();
                                        if (nganh_nghe != null)
                                        {
                                            nganh_nghe.Click();
                                            _logger.Info($"da chon nganh nghe {item}");
                                        }
                                        count++;
                                    }
                                }

                                var tinh_thanh_exac = browser.Find($".//select[@id='jobStep2_province1']/option[contains(text(),'{ct.dia_chi}')]")?.First();
                                if (tinh_thanh_exac != null)
                                {
                                    tinh_thanh_exac.Click();
                                    _logger.Info($"da chon tinh thanh {ct.dia_chi}");
                                }
                                else
                                {
                                    string str = ct.dia_chi;
                                    char[] spearator = { ',', '/', '-', ' ' };
                                    string[] strlist = str.Split(spearator, StringSplitOptions.None);

                                    foreach (string s in strlist)
                                    {
                                        if (!string.IsNullOrWhiteSpace(s))
                                        {
                                            var industry = browser.Find($".//select[@id='jobStep2_province1']/option[contains(text(),'{s}')]");
                                            if (industry.Count > 0)
                                            {
                                                foreach (var item in industry)
                                                {
                                                    item.Click();
                                                    _logger.Info($"da chon nganh nghe {item}");
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                browser.FindAndClick(".//select[@id='jobStep2_province1']/option[contains(text(),'Khác')]");
                                            }
                                        }
                                    }
                                }

                                var loai_hinh = step_2.FindElement(By.Id("jobStep2_positionType"));
                                if (loai_hinh != null)
                                {
                                    loai_hinh.SendKeys(ct.loai_hinh_cong_viec);
                                    _logger.Info("da send loai hinh cong viec");
                                }

                                //step_2.FindElement(By.Id("jobStep2_desireCareerLevel"));
                                var cap_bac = browser.Find($".//select[@id='jobStep2_desireCareerLevel']/option[text()='{ct.cap_bac}']")?.First();
                                if (cap_bac != null)
                                {
                                    cap_bac.Click();
                                    _logger.Info($"da tim thay cap bac {ct.cap_bac}");
                                }
                                else
                                {
                                    string str_vi_tri = ct.cap_bac;
                                    char[] spearator0 = { ',', '/', '-', ' ' };
                                    string[] strlist1 = str_vi_tri.Split(spearator0, StringSplitOptions.None);
                                    foreach (string s in strlist1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(s))
                                        {
                                            var industry = browser.Find($".//select[@id='jobStep2_desireCareerLevel']/option[contains(text(),'{s}')]");
                                            if (industry.Count > 0)
                                            {
                                                foreach (var item in industry)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(item.Text))
                                                    {
                                                        item.Click();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                browser.FindAndClick(".//ul[@class='el-scrollbar__view el-select-dropdown__list']//span[contains(text(),'Ngành nghề khác')]/ancestor::li");
                                            }
                                        }
                                    }
                                }

                                //step_2.FindElement(By.Id());
                                var luong = ct.muc_luong;
                                char[] cut_luong = { ',', '/', '-', ' ' };
                                var arr_luong = luong.Split(cut_luong, StringSplitOptions.RemoveEmptyEntries);
                                var luong_from = arr_luong[0];
                                var luong_to = arr_luong[1];

                                var luong_f = step_2.FindElement(By.Id("jobStep2_salaryFrom"));
                                if (luong_f != null)
                                {
                                    luong_f.SendKeys($"{luong_from}000000");
                                    _logger.Info("da send luong from");
                                }

                                var luong_t = step_2.FindElement(By.Id("jobStep2_salaryTo"));
                                if (luong_t != null)
                                {
                                    luong_t.SendKeys($"{luong_to}000000");
                                    _logger.Info("da send luong from");
                                }

                                var mo_ta_cong_viec = step_2.FindElement(By.Id("jobStep2_description_ifr"));
                                if (mo_ta_cong_viec != null)
                                {
                                    mo_ta_cong_viec.SendKeys(ct.mo_ta_cong_viec);
                                    _logger.Info("da send mo ta cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath mo ta cong viec");
                                }
                                // Kinh nghiệm / kỹ năng
                                browser.FindAndClick(".//select[@id='jobStep2_educationLevel']/option[contains(text(),'Khác')]");

                                var kinh_nghiem = step_2.FindElement(By.Id("jobStep2_experienceLevel"));
                                if (kinh_nghiem != null)
                                {
                                    kinh_nghiem.SendKeys("1-2 năm kinh nghiệm");
                                    _logger.Info("da send kinh nghiem lam viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath kinh nghiem lam viec");
                                }

                                var yeucau_cv = step_2.FindElement(By.Id("jobStep2_skill_ifr"));
                                if (yeucau_cv != null)
                                {
                                    yeucau_cv.SendKeys(ct.yeu_cau_cong_viec);
                                    _logger.Info("da send yeu cau cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath yeu cau cong viec");
                                }

                                //step_2.FindElement(By.XPath(".//input[@value='Lưu Bước 2 để tiếp tục Bước 3']")).Click();
                                var step2to3 = step_2.FindElement(By.XPath(".//input[@value='Lưu Bước 2 để tiếp tục Bước 3']"));
                                if (step2to3 != null)
                                {
                                    step2to3.Click();
                                    _logger.Info("da click buoc 2 de toi buoc 3");
                                }
                                else
                                {
                                    _logger.Error("khong den duoc step 3");
                                }
                                System.Threading.Thread.Sleep(2000);
                            }
                        }
                        _logger.Info("START POST STEP 3");
                        // Step 3 Thông tin liên hệ
                        var job_step_3 = browser.Find(".//form[@id='job-step3']");
                        if (job_step_3.Count > 0)
                        {
                            foreach (var step3 in job_step_3)
                            {
                                //step3.FindElement(By.Id("jobStep3_applicationPreferredLanguage")).SendKeys("Tiếng Việt");
                                var ngon_ngu = step3.FindElement(By.Id("jobStep3_applicationPreferredLanguage"));
                                if (ngon_ngu != null)
                                {
                                    ngon_ngu.SendKeys("Tiếng Việt");
                                    _logger.Info("da send ngon ngu TiengViet");
                                }

                                var mo_ta = step3.FindElement(By.Id("jobStep3_contactDescription_ifr"));
                                if (mo_ta != null)
                                {
                                    mo_ta.SendKeys(ct.mo_ta_cong_viec);
                                    _logger.Info("da send mo ta cong viec");
                                }
                                else
                                {
                                    _logger.Error("Khong tim thay xpath, ko the sendkeys mo ta cong viec");
                                }
                                //step3.FindElement(By.XPath(".//input[@value='Lưu Bước 3 để xem lại công việc']")).Click();
                                var next_step =
                                    step3.FindElement(By.XPath(".//input[@value='Lưu Bước 3 để xem lại công việc']"));
                                if (next_step != null)
                                {
                                    next_step.Click();
                                    _logger.Info(" da luu buoc 3 va xem lai cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong luu duoc cong viec");
                                }

                                System.Threading.Thread.Sleep(2000);
                            }
                        }
                        _logger.Info("START POST STEP 4");
                        // Step 4 xem lại và đăng lên
                        string cur_url = browser.GetUrl();
                        string del_str = cur_url.Substring(cur_url.LastIndexOf("/"));
                        string link_post = cur_url.Replace(del_str, "/");

                        List<JobLink> lst_saved = new List<JobLink>();
                        JobLink saved = new JobLink(LoaiLink.JOB_LINK)
                        {
                            ten_job = ct.chuc_danh,
                            app_id = app_id,
                            link = link_post + "thu-xin-viec-da-nhan?status=all",
                            trang_thai = TrangThai.DANG_SU_DUNG,
                            trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                            nguoi_tao = ch.username
                        };
                        _logger.Info($"da luu link job: {saved.link}");
                        lst_saved.Add(saved);
                        browser.FindAndClick(".//form/input[@name='btnSubmit']");
                        _logger.Info("SUCCESS POST CAREER LINK");
                        return ES.JobLinkRepository.Instance.IndexMany(lst_saved) > 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
            _logger.Error("POST CAREER LINK FAILED!");
            return false;
        }

        public void AutoUpdate()
        {
            var all_saved = ES.LinkSavedRepository.Instance.GetLinksByGiaTri(1, "https://www.careerlink.vn/");
            if (all_saved.Count > 0)
            {
                using (var browser = new XBrowser())
                {
                    foreach (var item in all_saved)
                    {
                        browser.GoTo(item.domain);
                        System.Threading.Thread.Sleep(2500);
                        var find = browser.Find(".//div[contains(text(),'Đang được đăng trực tuyến')]");
                        if (find.Count > 0)
                        {
                            item.thuoc_tinh = 3;
                            ES.LinkSavedRepository.Instance.UpdateStatus(item.id, item.thuoc_tinh);
                        }
                    }
                }
            }
        }
    }
}