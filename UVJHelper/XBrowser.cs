using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UngVienJobModel;

namespace UVJHelper
{
    public class XBrowser : IDisposable
    {
        private ChromeDriverService config_options = ChromeDriverService.CreateDefaultService();
        private IWebDriver browser = null;
        private WebDriverWait wait = null;
        private bool disposed = false;
        private string default_download_folder = "";

        #region Init Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if (browser != null)
                {
                    browser.Quit();
                }
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~XBrowser()
        {
            Dispose(false);
        }

        #endregion Init Dispose

        public XBrowser()
        {
            config_options.HideCommandPromptWindow = true;

            browser = new ChromeDriver(config_options);
            wait = new WebDriverWait(browser, TimeSpan.FromSeconds(5));
            wait.IgnoreExceptionTypes(typeof(OpenQA.Selenium.NoSuchElementException));
        }

        public XBrowser(string user_profile_path, string user_agent = "", bool disable_load_imgage = true, bool show_browser = false, int width = 0, int height = 0)
        {
            config_options.HideCommandPromptWindow = true;
            browser = new ChromeDriver(config_options, SetupChormeOption(user_profile_path, "", user_agent, disable_load_imgage, show_browser, width, height));
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(browser, TimeSpan.FromSeconds(10));
            wait.IgnoreExceptionTypes(typeof(OpenQA.Selenium.NoSuchElementException));
        }

        public XBrowser(string user_profile_path, string _default_download_folder = "", string user_agent = "", bool disable_load_imgage = true, bool show_browser = false, int width = 0, int height = 0)
        {
            config_options.HideCommandPromptWindow = true;
            default_download_folder = _default_download_folder;
            browser = new ChromeDriver(config_options, SetupChormeOption(user_profile_path, _default_download_folder, user_agent, disable_load_imgage, show_browser, width, height));
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(browser, TimeSpan.FromSeconds(10));
            wait.IgnoreExceptionTypes(typeof(OpenQA.Selenium.NoSuchElementException));
        }

        public void ScrollIntoView(IWebElement ele)
        {
            try
            {
                ((IJavaScriptExecutor)browser).ExecuteScript("arguments[0].scrollIntoView(true);", ele);
            }
            catch (Exception)
            {
            }
        }

        public string GoTo(string url)
        {
            browser.Navigate().GoToUrl(url);
            return browser.Url;
        }

        public string GetUrl()
        {
            return browser.Url;
        }

        public void Quit()
        {
            if (browser != null)
            {
                browser.Quit();
            }
        }

        public ChromeOptions SetupChormeOption(int user_profile_path = 0, bool show_browser = false, string user_agent = "", int width = 0, int height = 0)
        {
            ChromeOptions option = new ChromeOptions();
            if (!String.IsNullOrEmpty(user_agent))
            {
                //V3.14
                ChromeMobileEmulationDeviceSettings CMEDS = new ChromeMobileEmulationDeviceSettings();
                //V4.0
                //OpenQA.Selenium.Chromium.ChromiumMobileEmulationDeviceSettings CMEDS = new OpenQA.Selenium.Chromium.ChromiumMobileEmulationDeviceSettings();

                CMEDS.Width = width > 0 ? width : 300;
                CMEDS.Height = height > 0 ? height : 500;
                CMEDS.PixelRatio = 1.0;

                if (!String.IsNullOrEmpty(user_agent))
                {
                    CMEDS.UserAgent = user_agent;
                }
                else
                {
                    CMEDS.UserAgent = string.Empty;
                }
                option.EnableMobileEmulation(CMEDS);
            }

            option.AddArgument($@"user-data-dir={user_profile_path}");

            option.AddArguments("--no-sandbox");
            option.AddArguments("--start-maximized");
            option.AddArguments("--disable-notifications");
            option.AddArguments("--disable-gpu");
            option.AddArguments("--disable-software-rasterizer");
            option.AddArguments("--mute-audio");
            option.AddArguments("--hide-scrollbars");
            option.AddArgument("--disable-images");
            option.AddArgument("--blink-settings=imagesEnabled=false");
            option.AcceptInsecureCertificates = true;
            option.Proxy = null;
#if !DEBUG
            if (!show_browser)
            {
                    option.AddArgument("headless");
            }
#endif
            return option;
        }

        public ChromeOptions SetupChormeOption(string user_profile_path = "", string default_download_folder = "", string user_agent = "",
            bool disable_img = true, bool show_browser = false, int width = 0, int height = 0)
        {
            ChromeOptions option = new ChromeOptions();
            if (!String.IsNullOrEmpty(user_agent) && width < 800)
            {
                ChromeMobileEmulationDeviceSettings CMEDS = new ChromeMobileEmulationDeviceSettings();
                //OpenQA.Selenium.Chromium.ChromiumMobileEmulationDeviceSettings CMEDS = new OpenQA.Selenium.Chromium.ChromiumMobileEmulationDeviceSettings();
                CMEDS.Width = width > 0 ? width : 300;
                CMEDS.Height = height > 0 ? height : 500;
                CMEDS.PixelRatio = 1.0;

                if (!String.IsNullOrEmpty(user_agent))
                {
                    CMEDS.UserAgent = user_agent;
                }
                else
                {
                    CMEDS.UserAgent = string.Empty;
                }

                option.EnableMobileEmulation(CMEDS);
            }
            else
            {
                if (width > 0)
                {
                    option.AddArguments($"--width={width}");
                }
                if (height > 0)
                {
                    option.AddArguments($"--height={height}");
                }
            }
            if (!String.IsNullOrEmpty(user_profile_path))
            {
                option.AddArguments($@"user-data-dir={user_profile_path}");
            }
            option.AddExcludedArgument("enable-automation");
            //option.AddAdditionalCapability("excludeSwitches", "enable-automation");
            option.AddArgument("-disable-notifications");
            option.AddArguments("--no-sandbox");
            option.AddArguments("--disable-infobars"); //https://stackoverflow.com/a/43840128/1689770
            option.AddArguments("--disable-dev-shm-usage"); //https://stackoverflow.com/a/50725918/1689770
            option.AddArguments("--disable-browser-side-navigation"); //https://stackoverflow.com/a/49123152/1689770
            option.AddArguments("--disable-gpu");
            option.AddArguments("start-maximized");
            option.AddArguments("--disable-notifications");
            option.AddArguments("--disable-software-rasterizer");
            option.AddArguments("--mute-audio");
            option.AddArguments("enable-features=NetworkServiceInProcess");
            option.PageLoadStrategy = PageLoadStrategy.Eager;

            //option.AddArguments("disable-features=NetworkService");

            if (disable_img)
            {
                option.AddArgument("--disable-images");
                option.AddArgument("--blink-settings=imagesEnabled=false");
            }
            option.AcceptInsecureCertificates = true;
            option.Proxy = null;
            option.AddArguments("chrome.switches", "-disable-extensions");
            option.AddUserProfilePreference("exit_type", "none");
            option.AddUserProfilePreference("exited_cleanly", "true");
            option.AddArgument("--disable-features=InfiniteSessionRestore");
            if (!string.IsNullOrEmpty(default_download_folder))
            {
                option.AddUserProfilePreference("download.default_directory", $"{default_download_folder}");
            }
            option.AddUserProfilePreference("download.prompt_for_download", false);
            option.AddUserProfilePreference("disable-popup-blocking", "true");

            if (!show_browser)
            {
                option.AddArgument("headless");
            }

            return option;
        }

        public bool Login(string url_login, string username, string password, string xpath_username, string xpath_password, out string msg, string wait_xpath, bool need_decode = false)
        {
            msg = "";
            if (need_decode)
            {
                username = XMedia.XUtil.DecodeToken(username);
                password = XMedia.XUtil.DecodeToken(password);
            }
            try
            {
                browser.Url = url_login;

                var ele_username = wait.Until<IWebElement>((d) =>
                {
                    try
                    {
                        return d.FindElement(By.XPath(xpath_username));
                    }
                    catch (NoSuchElementException e)
                    {
                        return null;
                    }
                });
                var ele_password = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath_password)); } catch (NoSuchElementException e) { return null; } });
                if (ele_password != null && ele_username != null)
                {
                    ele_username.Clear();
                    ele_username.SendKeys(username);
                    ele_password.Clear();
                    ele_password.SendKeys(password);
                    ele_password.SendKeys(Keys.Enter);
                    try
                    {
                        //var ele_point = wait.Until(w => w.FindElement(By.XPath(wait_xpath)));
                        var ele_point = wait.Until<IWebElement>((d) =>
                        {
                            try
                            {
                                return d.FindElement(By.XPath(wait_xpath));
                            }
                            catch (NoSuchElementException e)
                            {
                                return null;
                            }
                        });
                        if (ele_password != null)
                        {
                            return true;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                            ele_point = wait.Until(w => w.FindElement(By.XPath(wait_xpath)));
                            return ele_point != null;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return browser.Url != url_login;
                }
                else
                {
                    msg = "Không tìm thấy xpath";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return false;
        }

        public bool Login(string url_login, string username, string password, string xpath_username, string xpath_password, out string msg)
        {
            msg = "";
            username = XMedia.XUtil.DecodeToken(username);
            password = XMedia.XUtil.DecodeToken(password);
            try
            {
                browser.Url = url_login;
                var ele_username = wait.Until<IWebElement>((d) =>
                {
                    try
                    {
                        return d.FindElement(By.XPath(xpath_username));
                    }
                    catch (NoSuchElementException e)
                    {
                        return null;
                    }
                });
                var ele_password = wait.Until<IWebElement>((d) =>
                {
                    try
                    {
                        return d.FindElement(By.XPath(xpath_password));
                    }
                    catch (NoSuchElementException e)
                    {
                        return null;
                    }
                });
                if (ele_password != null && ele_username != null)
                {
                    ele_username.Clear();
                    ele_username.SendKeys(username);
                    ele_password.Clear();
                    ele_password.SendKeys(password);
                    ele_password.SendKeys(Keys.Enter);
                    return browser.Url != url_login;
                }
                else
                {
                    msg = "Không tìm thấy xpath";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return false;
        }

        public void FindAndClick(string xpath)
        {
            try
            {
                var ele_find = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele_find != null)
                {
                    if (ele_find.Displayed)
                        ele_find.Click();
                    else
                    {
                        IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                        js.ExecuteScript("arguments[0].click();", ele_find);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public IWebElement FindFirst(string xpath)
        {
            try
            {
                var ele_find = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele_find != null)
                    return ele_find;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public IList<IWebElement> Find(string xpath)
        {
            try
            {
                var ele_find = wait.Until(w =>
                {
                    try
                    {
                        return w.FindElements(By.XPath(xpath));
                    }
                    catch (NoSuchElementException)
                    {
                    }

                    return null;
                });
                if (ele_find != null)
                    return ele_find;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public IWebElement FindChildElement(IWebElement ele, string xpath)
        {
            try
            {
                var child = wait.Until(w => ele.FindElement(By.XPath(xpath)));
                return child;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void CloseAllOtherTab()
        {
            try
            {
                if (browser.WindowHandles.Count > 1)
                {
                    foreach (var tab in browser.WindowHandles.Skip(1))
                    {
                        browser.SwitchTo().Window(tab);
                        browser.Close();
                    }
                    browser.SwitchTo().Window(browser.WindowHandles[0]);
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetAttribute(IWebElement ele, string prop, string val)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                js.ExecuteScript($"arguments[0].setAttribute('{prop}', '{val}')", ele);
            }
            catch (Exception)
            {
            }
        }

        public string GetAttribute(IWebElement ele, string attr)
        {
            try
            {
                return ele.GetAttribute(attr);
            }
            catch (Exception)
            {
            }
            return "";
        }

        public void Click(IWebElement ele)
        {
            try
            {
                if (ele.Displayed)
                    ele.Click();
                else
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                    js.ExecuteScript("arguments[0].click();", ele);
                }
            }
            catch (Exception)
            {
            }
        }

        public string GetAttribute(string xpath, string attr)
        {
            try
            {
                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele != null)
                {
                    string val = ele.GetAttribute(attr);
                    if (string.IsNullOrEmpty(val))
                        val = "";
                    return val.Trim();
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public string GetInnerText(string xpath)
        {
            try
            {
                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele != null)
                {
                    return ele.Text.Replace("\r", "").Replace("\n", "").Trim();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        public string GetInnerText(string xpath, int time_out_ms)
        {
            try
            {
                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele != null)
                {
                    string text = ele.Text;
                    int time_out = time_out_ms / 100;
                    while (time_out-- > 0)
                    {
                        text = ele.Text;
                        if (string.IsNullOrEmpty(text))
                            Thread.Sleep(100);
                        else
                            break;
                    }
                    return ele.Text.Trim();
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public string GetPageSource()
        {
            try
            {
                return browser.PageSource;
            }
            catch (Exception)
            {
            }
            return "";
        }

        public void WaitForLoad(int time_out = 15)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                WebDriverWait wait = new WebDriverWait(browser, new TimeSpan(0, 0, time_out));
                wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
            }
            catch (Exception)
            {
            }
        }

        public string GetIFrameSource(string xpath)
        {
            try
            {
                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele != null)
                {
                    string src = ele.GetAttribute("src");
                    string frame_locator = !string.IsNullOrEmpty(ele.GetAttribute("id")) ? ele.GetAttribute("id") : ele.GetAttribute("name");
                    wait.Until(w => SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(frame_locator));

                    browser.SwitchTo().Frame(ele);

                    string source = browser.PageSource;
                    if (source.Length < 100)
                    {
                        System.Threading.Thread.Sleep(2000);
                        source = browser.PageSource;
                    }
                    browser.SwitchTo().DefaultContent();
                    return source;
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public string GetIFrameText(string xpath)
        {
            try
            {
                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele != null)
                {
                    string src = ele.GetAttribute("src");
                    string frame_locator = !string.IsNullOrEmpty(ele.GetAttribute("id")) ? ele.GetAttribute("id") : ele.GetAttribute("name");
                    wait.Until(w => SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(frame_locator));

                    browser.SwitchTo().Frame(ele);

                    string source = browser.PageSource;
                    if (source.Length < 100)
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                    source = browser.FindElement(By.TagName("body")).GetAttribute("innerText");
                    browser.SwitchTo().DefaultContent();
                    return source;
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public string GetInnerHtml(string xpath, int timeout = 1000)
        {
            try
            {
                wait.Timeout = TimeSpan.FromMilliseconds(timeout);
                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                if (ele != null)
                {
                    return ele.GetAttribute("innerHTML").Trim();
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public byte[] DownloadFile(string url, out string file_name)
        {
            file_name = "";
            try
            {
                using (WebClient wc = new WebClient())
                {
                    StringBuilder all_cookies = new StringBuilder(); ;
                    foreach (var item in browser.Manage().Cookies.AllCookies)
                    {
                        all_cookies.AppendFormat("{0}={1}; ", item.Name, item.Value);
                    }
                    wc.Headers[HttpRequestHeader.Cookie] = all_cookies.ToString().Trim().TrimEnd(';');
                    var res = wc.DownloadData(url);
                    if (!string.IsNullOrEmpty(wc.ResponseHeaders["Content-Disposition"]))
                    {
                        file_name = wc.ResponseHeaders["Content-Disposition"].Substring(wc.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", "");
                    }
                    else
                    {
                        file_name = url.Substring(url.LastIndexOf('/') + 1);
                    }
                    return res;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void DoWorkFlow()
        {
            List<QuyTrinh> lst = new List<QuyTrinh>();
            var qt1 = new QuyTrinh() { thu_tu = 1, ten = "Tìm link", hanh_dong = HanhDong.LIST, xpath = "//tbody[@class='tbody-job']//td[3]/a", time_out = 2000 };
            var qt2 = new QuyTrinh() { thu_tu = 2, ten = "Click link", hanh_dong = HanhDong.GO_TO, xpath = "@href", time_out = 2000 };
            qt2.quy_trinh.Add(new QuyTrinh() { thu_tu = 1, ten = "Đợi", hanh_dong = HanhDong.WAIT, xpath = "", time_out = 2000 });
            var qt3 = new QuyTrinh() { thu_tu = 2, ten = "Tim", hanh_dong = HanhDong.LIST, xpath = "//div[@class='table-responsive']/table//td[2]/a", time_out = 2000 };
            qt3.quy_trinh.Add(new QuyTrinh()
            {
                thu_tu = 1,
                ten = "Xem CV",
                hanh_dong = HanhDong.CLICK,
                xpath = ".",
                time_out = 2000,
                quy_trinh = new List<QuyTrinh>() {
                    new QuyTrinh() {
                        ten="Tải CV",
                        thu_tu=1,
                        hanh_dong=HanhDong.CLICK,
                        xpath="//a[@id='btn-download-candidate']"
                    },
                    new QuyTrinh() {
                        ten="Đóng CV",
                        thu_tu=2,
                        hanh_dong=HanhDong.CLICK,
                        xpath="//div[@title='Đóng lại']"
                    }
                }
            });
            qt2.quy_trinh.Add(qt3);
            qt1.quy_trinh.Add(qt2);
            lst.Add(qt1);

            foreach (var qt in lst.OrderBy(x => x.thu_tu))
            {
                switch (qt.hanh_dong)
                {
                    case HanhDong.LIST:
                        var eles = wait.Until(w => w.FindElements(By.XPath(qt.xpath)));
                        Do(qt, eles);
                        break;

                    case HanhDong.CLICK:
                        var ele = wait.Until(w => w.FindElement(By.XPath(qt.xpath)));
                        if (ele != null)
                            ele.Click();
                        break;

                    case HanhDong.WAIT:
                        break;

                    case HanhDong.GO_TO:
                        browser.Url = qt.xpath;
                        Do(qt, null);

                        break;
                }
            }
        }

        public void Do(QuyTrinh qt, object parent)
        {
            foreach (var next in qt.quy_trinh.OrderBy(x => x.thu_tu))
            {
                switch (next.hanh_dong)
                {
                    case HanhDong.LIST:
                        if (parent == null)
                        {
                            var eles = wait.Until(w => w.FindElements(By.XPath(next.xpath)));
                            Do(next, eles);
                        }
                        break;

                    case HanhDong.CLICK:
                        if (parent == null)
                        {
                            try
                            {
                                var ele = wait.Until(w => w.FindElement(By.XPath(next.xpath)));
                                if (ele != null)
                                    ele.Click();
                            }
                            catch (Exception)
                            {
                            }

                            Do(next, null);
                        }
                        else
                        {
                            if (typeof(System.Collections.ObjectModel.ReadOnlyCollection<IWebElement>) == parent.GetType())
                            {
                                foreach (var item in (IEnumerable<IWebElement>)parent)
                                {
                                    if (browser.WindowHandles.Count > 1)
                                    {
                                        foreach (var tab in browser.WindowHandles.Skip(1))
                                        {
                                            browser.SwitchTo().Window(tab);
                                            browser.Close();
                                        }
                                        browser.SwitchTo().Window(browser.WindowHandles[0]);
                                    }

                                    if (item.Displayed)
                                        item.Click();
                                    else
                                    {
                                        IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                                        js.ExecuteScript("arguments[0].click();", item);
                                    }
                                    Do(next, null);
                                }
                            }
                        }
                        break;

                    case HanhDong.WAIT:
                        System.Threading.Thread.Sleep(next.time_out);
                        break;

                    case HanhDong.GO_TO:
                        if (typeof(System.Collections.ObjectModel.ReadOnlyCollection<IWebElement>) == parent.GetType())
                        {
                            List<string> lst_url = new List<string>();
                            foreach (var item in (IEnumerable<IWebElement>)parent)
                            {
                                lst_url.Add(item.GetAttribute(next.xpath.TrimStart('@')));
                            }

                            foreach (var url in lst_url)
                            {
                                browser.Url = url;
                                Do(next, null);
                            }
                        }

                        break;
                }
            }
        }

        public IWebElement expandRootElement(IWebElement element)
        {
            IWebElement ele = (IWebElement)((IJavaScriptExecutor)browser).ExecuteScript("return arguments[0].shadowRoot", element);
            return ele;
        }

        public string DownloadByBrowser(IWebElement ele, int timeout = 5000)
        {
            string file_name = "";
            IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
            try
            {
                ele.Click();

                while ((timeout = timeout - 500) > 0)
                {
                    browser.Navigate().GoToUrl("chrome://downloads");
                    System.Threading.Thread.Sleep(500);
                    try
                    {
                        var re = js.ExecuteScript("return document.querySelector('downloads-manager').shadowRoot.querySelector('downloads-item').shadowRoot.getElementById('name').innerText");

                        if (re != null)
                        {
                            var file_exist = System.IO.File.Exists($"{default_download_folder}\\{re.ToString()}");
                            if (file_exist)
                            {
                                file_name = re.ToString();
                            }
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                //XÓA TẤT CẢ CÁC FILE ĐÃ TẢI VỀ
                try
                {
                    js.ExecuteScript("document.querySelector('downloads-manager').shadowRoot.getElementById('toolbar').shadowRoot.getElementById('moreActionsMenu').querySelector('.clear-all').click()");
                }
                catch (Exception)
                {
                }
            }
            return file_name;
        }

        public string DownloadByBrowserInIFrame(string xpath_frame, string xpath_button_download)
        {
            string file_name = "";
            try
            {
                var ifr = browser.FindElement(By.XPath(xpath_frame));
                if (ifr != null)
                {
                    browser.SwitchTo().Frame(0);
                    var d = browser.FindElement(By.XPath(xpath_button_download));
                    file_name = DownloadByBrowser(d);
                }
            }
            catch (Exception)
            {
            }
            return file_name;
        }

        public IWebElement WaitElementVisible(string xpath, int timeout_ms, out bool is_element_exist)
        {
            is_element_exist = false;
            try
            {
                browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(timeout_ms);

                var ele = wait.Until<IWebElement>((d) => { try { return d.FindElement(By.XPath(xpath)); } catch (NoSuchElementException e) { return null; } }); ;
                is_element_exist = (ele != null);
                return ele;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void removeAttrById(string id, string attr)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
            js.ExecuteScript($"document.getElementById('{id}').removeAttribute('{attr}')");
        }
      

    }
}