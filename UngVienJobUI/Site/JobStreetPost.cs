using log4net;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Reflection;
using UngVienJobModel;
using UngVienJobUI.Utils;
using UVJHelper;
using XMedia;

namespace UngVienJobUI.Site
{
    public class JobStreetPost
    {
        private string user_profile_path = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\profiles";
        private bool is_authenticated;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(JobStreetPost));

        public bool ExtractThongTin(CauHinh ch, string link, string app_id, out string msg, UngVienJobModel.ChiTietTinModel ct, bool is_debug = false)
        {
            _logger.Info("START POST JOBSTREET");
            is_authenticated = false;
            msg = "";
            //string username = XUtil.ConfigurationManager.AppSetting["JobStreet:username"];
            //string password = XUtil.ConfigurationManager.AppSetting["JobStreet:password"];
            using (var browser = new XBrowser(user_profile_path, string.Empty, false, is_debug))
            {
                browser.GoTo(ch.url_login);
                var formLogin = browser.Find(".//div[@class='Card__root']");
                if (formLogin.Count > 0)
                {
                    foreach (var form in formLogin)
                    {
                        form.FindElement(By.XPath(ch.xpath_username)).SendKeys(ch.username);
                        form.FindElement(By.XPath(ch.xpath_password)).SendKeys(XMedia.XUtil.DecodeToken(ch.password));
                        form.FindElement(By.XPath(".//button[@class='Button__primary Button__insideCard Button__extraTopMargin']")).Click();
                        System.Threading.Thread.Sleep(3000);
                        _logger.Info("LOGIN JOBSTREET SUCCESS");
                    }
                }
                System.Threading.Thread.Sleep(2000);
                browser.GoTo(link);

                var check_login = browser.Find("//*[@id='app']/div/nav/div/div/a[4]");
                if (check_login.Count >= 1)
                {
                    is_authenticated = true;
                    _logger.Info("CHECK LOGIN JOBSTREET SUCCESS");
                }
                System.Threading.Thread.Sleep(2000);
                if (is_authenticated)
                {
                    _logger.Info("LOGIN JOBSTREET SUCCESS");
                    var parent = browser.Find(".//div[@class='Card__root']");
                    if (parent.Count >= 1)
                    {
                        try
                        {
                            _logger.Info("START SENDKEYS");
                            foreach (var element in parent)
                            {
                                var chuc_danh = element.FindElement(By.Id("jobTitle"));
                                if (chuc_danh != null)
                                {
                                    chuc_danh.SendKeys(ct.chuc_danh);
                                    _logger.Info($"da send chuc danh {ct.chuc_danh}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath chuc danh");
                                }

                                var loai_hinh_cv = element.FindElement(By.Id("jobType"));
                                if (loai_hinh_cv != null)
                                {
                                    loai_hinh_cv.SendKeys(ct.loai_hinh_cong_viec);
                                    _logger.Info($"da send loai hinh cong viec {ct.loai_hinh_cong_viec}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath loai hinh cong viec");
                                }

                                var mo_ta = element.FindElement(By.XPath(".//div[@class='fr-element fr-view']"));
                                if (mo_ta != null)
                                {
                                    mo_ta.SendKeys(ct.mo_ta_cong_viec);
                                    _logger.Info($"da send mo ta cong viec {ct.mo_ta_cong_viec}");
                                }
                                else
                                {
                                    _logger.Error("khong tim thay xpath mo ta cong viec");
                                }

                                var save_button = element.FindElement(By.XPath(
                                    ".//button[@class='Button__primary Button__insideCard Button__extraTopMargin']"));
                                if (save_button != null)
                                {
                                    save_button.Click();
                                    _logger.Info($"luu lai job thanh cong");
                                    _logger.Info("POST JOBSTREET SUCCESS");
                                }
                                else
                                {
                                    _logger.Error("khong the luu lai cong viec");
                                }
                                System.Threading.Thread.Sleep(3000);
                                string cur_url = browser.GetUrl();
                                string del_str = cur_url.Substring(cur_url.LastIndexOf("/", StringComparison.Ordinal));
                                string link_post = cur_url.Replace(del_str, "/details");

                                List<JobLink> lst_saved = new List<JobLink>();
                                JobLink job = new JobLink(LoaiLink.JOB_LINK)
                                {
                                    ten_job = ct.chuc_danh,
                                    app_id = app_id,
                                    link = link_post,
                                    trang_thai = TrangThai.DANG_SU_DUNG,
                                    trang_thai_xu_ly = TrangThaiXuLy.CHUA_XU_LY,
                                    nguoi_tao = ch.username
                                };
                                lst_saved.Add(job);

                                browser.GoTo("https://employer.jobstreet.vn/vn/home");
                                _logger.Info("POST JOBSTREET SUCCESS!");
                                return ES.JobLinkRepository.Instance.IndexMany(lst_saved) > 0;
                            }

                            #region Step2JobStreet

                            //var parent_next = browser.Find(".//div[@class='Card__root']");
                            //if (parent_next.Count > 0)
                            //{
                            //    try
                            //    {
                            //        foreach (var element in parent_next)
                            //        {
                            //            element.FindElement(By.Id("name")).SendKeys(ct.ten_cong_ty);
                            //            element.FindElement(By.Id("country")).SendKeys("Việt Nam");
                            //            IWebElement ele = element.FindElement(By.Id("businessAddress"));
                            //            ele.SendKeys(ct.dia_chi_chi_tiet);
                            //            System.Threading.Thread.Sleep(1000);
                            //            IList<IWebElement> elements = (element.FindElements(By.XPath("//ul[@role='listbox']/child::li")));
                            //            foreach (var ele1 in elements)
                            //            {
                            //                ele1.Click();
                            //                break;
                            //            }
                            //            Console.WriteLine("2131");
                            //            foreach (var nganh in ct.nganh_nghe)
                            //            {
                            //                browser.FindAndClick($".//select[@id='industryId']/option[contains(text(),'{nganh}')]");
                            //            }

                            //            element.FindElement(By.Id("businessSize")).SendKeys(ct.quy_mo_doanh_nghiep);
                            //            string recruiter = "recruiter";
                            //            var select_pos = new SelectElement(element.FindElement(By.Id("role")));
                            //            select_pos.SelectByValue(recruiter);
                            //            element.FindElement(By.Id("recruitmentCompanyName")).SendKeys(ct.ten_cong_ty);
                            //            List<LinkSaved> lst_saved = new List<LinkSaved>();
                            //            LinkSaved saved = new LinkSaved();
                            //            saved.website = "jobstreet.vn";
                            //            saved.ten_job = ct.chuc_danh;
                            //            lst_saved.Add(saved);
                            //            Es.IndexLinkPosted(lst_saved);

                            //            return true;
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Console.WriteLine(ex);
                            //    }
                            //}

                            #endregion Step2JobStreet
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                    else
                    {
                        _logger.Error("CAN'T FOUND FORM POST JOBSTREET");
                    }
                }
                else
                {
                    _logger.Error("LOGIN JOBSTREET FAILED!");
                }
            }
            _logger.Error("POST JOBSTREET FAILED!");
            return false;
        }

        public void AutoUpdate()
        {
            var all_saved = ES.LinkSavedRepository.Instance.GetLinksByGiaTri(1, "https://jobstreet.vn/");

            if (all_saved.Count > 0)
            {
                using (var browser = new XBrowser())
                {
                    foreach (var item in all_saved)
                    {
                        browser.GoTo(item.domain);
                        System.Threading.Thread.Sleep(500);
                        var find = browser.Find(".//div[contains(text(),'Đang được đăng trực tuyến')]");
                        item.thuoc_tinh = find.Count > 0 ? 3 : 5;
                        ES.LinkSavedRepository.Instance.UpdateStatus(item.id, item.thuoc_tinh);
                    }
                }
            }
        }
    }
}