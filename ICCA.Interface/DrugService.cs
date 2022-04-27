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
    partial class DrugService : ServiceBase
    {
        private ConfigInfo config;
        private Timer timerMainCallback;
        private Object timerMainCallbackLock = new object();
        public DrugService()
        {
            InitializeComponent();
            config = Common.LoadServiceConfig();
            timerMainCallback = new Timer();
            timerMainCallback.Interval = config.InDrugInterval;
            timerMainCallback.Enabled = true;
            timerMainCallback.Elapsed += new ElapsedEventHandler(OnTimerMainCallback);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LogUtil.DebugLog("Enter DrugService OnStart().");
                timerMainCallback.Enabled = true;
            }
            catch (Exception e)
            {
                LogUtil.ErrorLog("Error DrugService on OnStart():"+e.ToString());
            }
        }

        protected override void OnStop()
        {
            LogUtil.DebugLog("Exit DrugService OnStop().");
        }

        /// <summary>
        /// Timer response function.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Event arguments</param>
        private void OnTimerMainCallback(object sender, EventArgs e)
        {
            lock (timerMainCallbackLock)
            {
                try
                {
                    //TO DO
                    timerMainCallback.Enabled = false;
                    CalculateRepository calculateRepository = new CalculateRepository();
                    calculateRepository.RunCalculate();
                }
                catch (Exception ex)
                {
                    LogUtil.ErrorLog("DrugService-OnTimerMainCallback报错:" + ex.ToString());
                }
                finally
                {
                    timerMainCallback.Enabled = true;
                }
            }
        }
    }
}
