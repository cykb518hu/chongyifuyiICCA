using ICCA.Interface.Log;
using ICCA.Interface.Repository;
using ICCA.Interface.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ICCA.Interface
{
    partial class SearchService : ServiceBase
    {
        private ConfigInfo config;
        private Timer timerMainCallback;
        private Object timerMainCallbackLock = new object();
        public SearchService()
        {
            InitializeComponent();
            config = Common.LoadServiceConfig();
            timerMainCallback = new Timer();
            timerMainCallback.Interval = config.InSearchInterval;
            timerMainCallback.Enabled = true;
            timerMainCallback.Elapsed += new ElapsedEventHandler(OnTimerMainCallback);
        }

        /// <summary>
        /// Timer response function.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnTimerMainCallback(object sender, EventArgs e)
        {
            // return;
            lock (timerMainCallbackLock)
            {
                try
                {
                    timerMainCallback.Enabled = false;
                    JobRepository jobRepository = new JobRepository(config);
                    jobRepository.RunJob();
                }
                catch (Exception ex)
                {
                    LogUtil.ErrorLog("SearchService-OnTimerMainCallback 报错:" + ex.ToString());
                }
                finally
                {
                    timerMainCallback.Enabled = true;
                }
            }
        }


        protected override void OnStart(string[] args)
        {
            LogUtil.DebugLog("Enter SearchService OnStart().");
            // TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            LogUtil.DebugLog("Exit SearchService OnStop().");
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
