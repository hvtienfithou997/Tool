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
    public class JobsGoPost
    {
        //private string cv_save_path;

        private bool is_authenticated = false;
        private string user_profile_path = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\profiles";

        //public JobsGoPost(string _user_profile_path)
        //{
        //    user_profile_path = _user_profile_path;
        //}
        private static readonly ILog _logger = LogManager.GetLogger(typeof(JobsGoPost));

        public bool ExtractThongTin(CauHinh ch, string link, UngVienJobModel.ChiTietTinModel ct, string app_id, out string msg, bool is_debug = false)
        {
            _logger.Info("START POST JOBSGO");
            msg = "";
            is_authenticated = false;
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
            {
                string url = browser.GoTo(ch.url_login);
                var login_form = browser.Find(".//div[@class='panel panel-body login-form']");
                if (browser.Find("//div[@id='modal']//button").Count > 0)
                {
                    browser.FindAndClick("//div[@id='modal']//button");
                }
                if (login_form.Count > 0)
                {
                    try
                    {
                        foreach (var element in login_form)
                        {
                            element.FindElement(By.XPath(ch.xpath_username)).SendKeys(ch.username);
                            element.FindElement(By.XPath(ch.xpath_password)).SendKeys(ch.password);
                            element.FindElement(By.XPath(".//button[@class='display-block btn btn-primary btn-ladda btn-ladda-spinner btn-ladda-progress']")).Click();
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                browser.GoTo(link);

                is_authenticated = true;

                if (is_authenticated)
                {
                    _logger.Info("LOGIN JOBSGO SUCCESS");
                    try
                    {
                        var tab1 = browser.Find(".//div[@id='tab1']");
                        if (tab1.Count > 0)
                        {
                            foreach (var tab in tab1)
                            {
                                var chuc_danh = tab.FindElement(By.Id("jobformcreate-job_title"));
                                if (chuc_danh != null)
                                {
                                    chuc_danh.SendKeys(ct.chuc_danh);
                                    _logger.Info("da send chuc danh");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath chuc danh");
                                }

                                var mo_ta = tab.FindElement(By.Id("jobformcreate-job_description_ifr"));
                                if (mo_ta != null)
                                {
                                    mo_ta.SendKeys(ct.mo_ta_cong_viec);
                                    _logger.Info("da send mo ta cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath mo ta cong viec");
                                }

                                var yeu_cau = tab.FindElement(By.Id("jobformcreate-job_requirement_ifr"));
                                if (yeu_cau != null)
                                {
                                    yeu_cau.SendKeys(ct.yeu_cau_cong_viec);
                                    _logger.Info("da send yeu cau cong viec");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath yeu cau cong viec");
                                }

                                var quyen_loi = tab.FindElement(By.Id("jobformcreate-job_benefit_ifr"));
                                if (quyen_loi != null)
                                {
                                    quyen_loi.SendKeys(ct.quyen_loi_ung_vien);
                                    _logger.Info("da send quyen loi ung vien");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath quyen loi ung vien");
                                }

                                var next_tab = tab.FindElement(By.XPath(".//button[@class='btn btn-primary btn-next btn-ladda btn-ladda-spinner btn-ladda-progress']"));
                                if (next_tab != null)
                                {
                                    next_tab.Click();
                                    _logger.Info("luu tab 1 de toi tab 2");
                                }
                                else
                                {
                                    _logger.Error("luu tab 1 de toi tab 2");
                                }

                                System.Threading.Thread.Sleep(2000);
                            }
                        }

                        var tab2 = browser.Find(".//div[@id='tab2']");
                        if (tab2.Count > 0)
                        {
                            foreach (var tab in tab2)
                            {
                                var detail_address =
                                    tab.FindElement(By.XPath(".//input[@placeholder='Nhập địa điểm']"));
                                if (detail_address != null)
                                {
                                    detail_address.SendKeys(ct.dia_chi_chi_tiet);
                                    _logger.Info("da send dia chi chi tiet");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath dia chi chi tiet");
                                }
                                System.Threading.Thread.Sleep(3000);
                                try
                                {
                                    foreach (var ele in browser.Find(".//div[@class='pac-container pac-logo hdpi']/div"))
                                    {
                                        ele.Click();
                                        _logger.Info($"chon dia chi {ele}");
                                        break;
                                    }

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);

                                }
                                var dia_chi = browser.Find("//table[@id='tableAddRow']/tbody/tr/td[3]/span")?.First();
                                if (dia_chi != null)
                                {
                                    dia_chi.Click();
                                    System.Threading.Thread.Sleep(1500);
                                    var ins_dia_chi = browser.Find($"//span[@class='select2-container select2-container--default select2-container--open']//ul/li[text()='{ct.dia_chi}']")?.First();
                                    if (ins_dia_chi != null)
                                    {
                                        ins_dia_chi.Click();
                                        _logger.Info($"da send dia chi {ct.dia_chi}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath dia chi");
                                    }
                                    System.Threading.Thread.Sleep(1000);
                                }

                                //Ngành nghề
                                System.Threading.Thread.Sleep(1000);
                                //browser.FindAndClick("//div[@class='form-group field-jobformnew-job_category required']/div/span");
                                var nganh_nghe = browser.Find("//div[contains(@class,'field-jobformnew-job_category')]")?.First();
                                if (nganh_nghe != null)
                                {
                                    nganh_nghe.Click();
                                    _logger.Info("chon it nhat mot nganh nghe");
                                    foreach (var nn in ct.nganh_nghe)
                                    {
                                        _logger.Info("Tim nganh nghe");
                                        browser.FindAndClick($"//span[@class='select2-container select2-container--default select2-container--open']//ul/li[text()='{nn}']");
                                        _logger.Info($"da chon nganh nghe: {nn}");
                                        System.Threading.Thread.Sleep(1000);
                                        //browser.FindAndClick("//div[@class='form-group field-jobformnew-job_category required has-success']/div/span");
                                    }
                                    System.Threading.Thread.Sleep(1000);
                                }

                                // Năm kinh nghiệm

                                if (!string.IsNullOrEmpty(ct.kinh_nghiem_from) &&
                                    !string.IsNullOrEmpty(ct.kinh_nghiem_to))
                                {
                                    var min_year_exp = browser.Find(".//input[@name='JobFormNew[exp_min_require_year]']")?.First();
                                    if (min_year_exp != null)
                                    {
                                        min_year_exp.SendKeys(Keys.Control + "a");
                                        min_year_exp.SendKeys(ct.kinh_nghiem_from);
                                        _logger.Info("da send nam kinh nghiem from");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath nam kinh nghiem from");
                                    }
                                    var max_year_exp = browser.Find(".//input[@name='JobFormNew[exp_max_require_year]']")?.First();
                                    if (max_year_exp != null)
                                    {
                                        max_year_exp.SendKeys(Keys.Control + "a");
                                        max_year_exp.SendKeys(ct.kinh_nghiem_to);
                                        _logger.Info("da send nam kinh nghiem to");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath nam kinh nghiem to");
                                    }
                                }
                                else
                                {
                                    browser.FindAndClick("//input[@id='checkbox1']");
                                    _logger.Info("cong viec khong yeu cau kinh nghiem");
                                }
                                //
                                // Mức lương
                                char[] cut_luong = { ',', '/', '-', ' ' };
                                var arr_luong = ct.muc_luong.Split(cut_luong, StringSplitOptions.RemoveEmptyEntries);
                                var luong_from = arr_luong[0];
                                var luong_to = arr_luong[1];
                                System.Threading.Thread.Sleep(1000);
                                var min_salary = browser.Find(".//input[@name='JobFormNew[min_expect_salary]']")?.First();
                                if (min_salary != null)
                                {
                                    min_salary.SendKeys(Keys.Control + "a");
                                    min_salary.SendKeys(luong_from);
                                    _logger.Info($"da send luong from {luong_from}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath luong from");
                                }

                                System.Threading.Thread.Sleep(1000);
                                var max_salary = browser.Find(".//input[@name='JobFormNew[max_expect_salary]']")?.First();

                                if (max_salary != null)
                                {
                                    max_salary.SendKeys(Keys.Control + "a");
                                    max_salary.SendKeys(luong_to);
                                    _logger.Info($"da send luong to {luong_to}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath luong to");
                                }
                                // Bằng cấp
                                System.Threading.Thread.Sleep(1000);

                                var bang_cap = browser.Find("//div[contains(@class,'field-jobformnew-degree')]/div/span")?.First();
                                if (bang_cap != null)
                                {
                                    bang_cap.Click();
                                    _logger.Info("da send bang cap");
                                    System.Threading.Thread.Sleep(1000);
                                    var choose_bc = browser.Find($"//span[@class='select2-container select2-container--default select2-container--open']//ul/li[text()='{ct.bang_cap}']")?.First();
                                    if (choose_bc != null)
                                    {
                                        choose_bc.Click();
                                        _logger.Info($"da chon bang cap {ct.bang_cap}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath bang cap");
                                    }
                                }

                                // loại hình công việc / Tính chất công việc
                                System.Threading.Thread.Sleep(1000);
                                var loai_hinh_cv = browser.Find("//div[contains(@class,'field-jobformnew-job_type')]/div/span")?.First();
                                if (loai_hinh_cv != null)
                                {
                                    loai_hinh_cv.Click();
                                    _logger.Info("chon loai hinh cong viec");
                                    var choose_loai_hinh = browser.Find(
                                        $"//span[@class='select2-container select2-container--default select2-container--open']//ul/li[text()='{ct.loai_hinh_cong_viec}']")?.First();
                                    if (choose_loai_hinh != null)
                                    {
                                        choose_loai_hinh.Click();
                                        _logger.Info($"da chon loai hinh cong viec {ct.loai_hinh_cong_viec}");
                                    }
                                    else
                                    {
                                        _logger.Error("khong tim thay xpath loai hinh cong viec");
                                    }
                                }
                                System.Threading.Thread.Sleep(1000);

                                // Vị trí
                                System.Threading.Thread.Sleep(1000);
                                var vi_tri = browser.Find("//div[contains(@class,'field-jobformnew-job_position_id')]/div/span")?.First();
                                if (vi_tri != null)
                                {
                                    vi_tri.Click();
                                    _logger.Info("chon vi tri cong viec");
                                    System.Threading.Thread.Sleep(1000);
                                    var choose_vi_tri = browser.Find($"//span[@class='select2-container select2-container--default select2-container--open']//ul/li[text()='{ct.cap_bac}']")?.First();
                                    if (choose_vi_tri != null)
                                    {
                                        choose_vi_tri.Click();
                                        _logger.Info($"da chon vi tri {ct.cap_bac}");
                                    }
                                    else
                                    {
                                        _logger.Error($"khong tim thay xpath vi tri");
                                    }
                                }

                                var save = browser.Find("//button[@class='btn btn-primary btn-next btn-ladda btn-ladda-spinner btn-ladda-progress']")?.First();
                                if (save != null)
                                {
                                    save.Click();
                                    _logger.Info("luu lai cong viec thanh cong");
                                }
                                else
                                {
                                    _logger.Error("khong the luu lai cong viec");
                                }
                                System.Threading.Thread.Sleep(2000);
                            }
                        }

                        browser.GoTo("https://employer.jobsgo.vn/job/inactive");
                        var url_tin =
                            browser.Find(
                                $".//table[@class='kv-grid-table table table-hover table-bordered table-striped']//tbody/tr/td[2]/strong/a[@title='Xem chi tiết việc làm và các ứng viên'][text()='{ct.chuc_danh} ']")?.First();

                        string href = browser.GetAttribute(url_tin, "href");
                        _logger.Info($"da lay link cua tin tuc : {href}");
                        var lst_saved = new List<JobLink>();
                        if (!string.IsNullOrEmpty(href))
                        {
                            JobLink saved = new JobLink(LoaiLink.JOB_LINK)
                            {
                                ten_job = ct.chuc_danh,
                                app_id = app_id,
                                link = href,
                                trang_thai = TrangThai.DANG_SU_DUNG,
                                trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                nguoi_tao = ch.username
                            };
                            lst_saved.Add(saved);
                            return ES.JobLinkRepository.Instance.IndexMany(lst_saved) > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            return false;
        }
    }
}