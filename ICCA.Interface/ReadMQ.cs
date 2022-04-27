using ICCA.Interface.Log;
using ICCA.Interface.Model;
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
using System.Xml;

namespace ICCA.Interface
{
    partial class ReadMQ : ServiceBase
    {
        private ConfigInfo config;
        private Timer timerMainCallback;
        private Object timerMainCallbackLock = new object();
        public ReadMQ()
        {
            InitializeComponent();
            config = Common.LoadServiceConfig();
            timerMainCallback = new Timer();
            timerMainCallback.Interval = config.InInterval;
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
                    LogUtil.DebugLog("RunMessage-开始");
                    timerMainCallback.Enabled = false;
                    BrokerRepository brokerRepository = new BrokerRepository();
                    var message = brokerRepository.GetUnReadTMessage();
                    var result = 1;
                    if (message != null)
                    {
                        MessageRepository messageRepository = new MessageRepository(config);
                        switch (message.MessageType)
                        {
                            case "InPatient":
                                result=messageRepository.InsertPatient(message.MessageContent, message.AutoId);
                                break;
                            case "TransferPatient":
                                result = messageRepository.TransferPatient(message.MessageContent, message.AutoId);
                                break;
                            case "OutPatient":
                                result = messageRepository.DischargePatient(message.MessageContent, message.AutoId);
                                break;
                            case "DrugOrder":
                                result = messageRepository.InsertDrugOrder(message.MessageContent, message.AutoId);
                                break;
                            case "ChangeDrugOrderStatus":
                                result = messageRepository.ChangeDrugOrderStatus(message.MessageContent, message.AutoId);
                                break;
                            case "ExecuteDrugOrder":
                                result = messageRepository.ExecuteDrugOrder(message.MessageContent, message.AutoId);
                                break;
                            case "LisCriticalSign":
                                result = messageRepository.InsertLisCriticalSign(message.MessageContent, message.AutoId);
                                break;
                                
                        }
                        brokerRepository.UpdateTMessage(message.AutoId, result);
                    }
                }
                catch(Exception ex)
                {
                    LogUtil.ErrorLog("ReadMQ-OnTimerMainCallback 报错:" + ex.ToString());
                }
                finally
                {
                    timerMainCallback.Enabled = true;
                }
            }
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                LogUtil.DebugLog("Enter ReadMQ OnStart().");
                timerMainCallback.Enabled = true;
                return;

                //住院登记推送
                ListenInPatientMq();
                //出入科登记推送
                ListenTransferPatientMq();
                //出院登记推送
                ListenDischargePatientMq();
                //医嘱推送
                ListenDrugOrderMq();
                //医嘱状态变更
                ListenDrugOrderStatusMq();
                //医嘱执行推送
                ListenDrugOrderExecuteMq();

                //危机值监听
                ListenLisCriticalSignMq();
            }
            catch(Exception ex)
            {
                LogUtil.ErrorLog("Error ReadMQ on OnStart():" + ex.ToString());
            }
        }


        protected override void OnStop()
        {
            LogUtil.DebugLog("Exit ReadMQ OnStart().");
        }
        public void Test()
        {
            //BrokerRepository sql = new BrokerRepository();
            //var msg = sql.GetTMessage(140).MessageContent;
            //ChangeDrugOrderStatus(msg);

            PatientQueue pa = new PatientQueue();
            //pa.PatientNo = "BA0000000069";
            //pa.VisitNo = "191";
            pa.PatientNo = "0000000076";
            pa.VisitNo = "575";

         //   GetLisData(pa);
        }
        public void ListenInPatientMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听住院登记推送队列");
            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS10003.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到入院推送信息");
                InsertMessage(msg, "InPatient");
            }, onStoped: (qMgrName, qName) =>
            {

            });
        }

        public void ListenTransferPatientMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听转科转床信息推送队列");

            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS10005.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到转科转床推送信息");
                InsertMessage(msg, "TransferPatient");
            }, onStoped: (qMgrName, qName) => {

            });
        }

        public void ListenDischargePatientMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听出院信息推送队列");

            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS10004.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到出院推送信息");
                InsertMessage(msg, "OutPatient");
            }, onStoped: (qMgrName, qName) => {

            });
        }

        public void ListenDrugOrderMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听医嘱推送队列");

            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS35002.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到医嘱推送信息");
                InsertMessage(msg, "DrugOrder");
            }, onStoped: (qMgrName, qName) =>
            {

            });
        }
        public void ListenDrugOrderStatusMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听医嘱状态推送队列");

            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS35001.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到医嘱变更推送信息");
                InsertMessage(msg, "ChangeDrugOrderStatus");
            }, onStoped: (qMgrName, qName) =>
            {

            });
        }

        public void ListenDrugOrderExecuteMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听医嘱执行推送队列");

            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS35008.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到医嘱执行推送信息");
                InsertMessage(msg, "ExecuteDrugOrder");
            }, onStoped: (qMgrName, qName) =>
            {

            });
        }
        public void ListenLisCriticalSignMq()
        {
            var mqClient = MQSDK.SdkClient.Create();
            LogUtil.DebugLog("开始监听危机值推送队列");

            var response = mqClient.Listen("QMGR.P13", "EQ.S13.PS20003.COLLECT", onReceived: (msgId, msg) =>
            {
                LogUtil.DebugLog("获取到危机值送信息");
                InsertMessage(msg, "LisCriticalSign");
            }, onStoped: (qMgrName, qName) =>
            {

            });
        }

        public void InsertMessage(string msg, string messageType)
        {
            var sqlHelper = new BrokerRepository();
            TMessage message = new TMessage();
            message.MessageContent = msg;
            message.MessageFormat = "XML";
            message.MessageType = messageType;
            sqlHelper.InsertTMessage(message);
        }
    }
}
