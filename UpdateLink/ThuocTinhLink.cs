using log4net;
using System;
using System.ServiceProcess;
using UngVienJobModel;
using UngVienJobUI.Site;
using UngVienJobUI.Utils;

namespace UpdateLink
{
    public partial class ThuocTinhLink : ServiceBase
    {
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private System.Timers.Timer _timerAutoSend = new System.Timers.Timer();
        private static readonly ILog _logger = LogManager.GetLogger(nameof(ThuocTinhLink));

        public ThuocTinhLink()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _logger.Error("START UPDATE");
                //_timer.Elapsed += _timer_Elapsed;
                _timer.AutoReset = false;
                _timer.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        //object sender, System.Timers.ElapsedEventArgs ee
        public void _timer_Elapsed()
        {
            try
            {
                var jsp = new JobStreetPost();
                jsp.AutoUpdate();
                var ch = new CauHinh();
                CauHinhAccount.CauHinhTopCv(ch);
                var mwb = new MyWorkPost();
                mwb.AutoUpdate();
                var tcv = new TopCvPost();
                tcv.AutoUpdate(ch);
                var crl = new CareerLinkPost();
                crl.AutoUpdate();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                var set_time = System.Configuration.ConfigurationManager.AppSettings["TimeIntervalMinutes"];
                _timer.Interval = TimeSpan.FromMinutes(int.Parse(set_time)).TotalMilliseconds;
            }
        }

        protected override void OnStop()
        {
        }
    }
}