using log4net;
using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace CheckStatusJob
{
    public partial class UpdateStatus : ServiceBase
    {
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private System.Timers.Timer _timerAutoSend = new System.Timers.Timer();
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UpdateStatus).Name);

        public UpdateStatus()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _logger.Error("START Check_ProKafka2FB");
                _timer.Elapsed += _timer_Elapsed;
                _timer.AutoReset = false;
                _timer.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            _logger.Error("Stop here");
        }

        public void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs ee)
        {
            try
            {
               
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                _timer.Interval = TimeSpan.FromMinutes(60).TotalMilliseconds;
            }
        }
    }
}