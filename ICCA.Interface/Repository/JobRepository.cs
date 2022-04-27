using ICCA.Interface.Log;
using ICCA.Interface.Model;
using ICCA.Interface.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ICCA.Interface.Repository
{
    public class JobRepository
    {
        private DateTime startTime = DateTime.Now;
        private string shareFoder = "";
        public JobRepository(ConfigInfo config)
        {
            shareFoder = config.LisBloodSharePath;
            startTime = DateTime.Now.AddMilliseconds(-config.InInterval);
        }
        public void RunJob()
        {
            LogUtil.DebugLog("RunJob-开始");
            BrokerRepository sqlHelper = new BrokerRepository();
            var patientList = sqlHelper.GetPatientQueue();
            if (patientList.Any())
            {
                foreach (var patient in patientList)
                {
                   // GetOperationData(patient);
                   // GetLisData(patient);
                }
                GetLisBloodData(patientList);
            }
            else
            {
                LogUtil.DebugLog("RunJob-没有在科患者");
            }
            LogUtil.DebugLog("RunJob-结束");
        }

        public void GetOperationData(PatientQueue patient)
        {
            var resultMsg = "";
            BrokerRepository sqlHelper = new BrokerRepository();
            try
            {
                LogUtil.DebugLog("GetOperationData-查询手术信息");
                var conditonStr = "<query item=\"MR_NO\" compy=\" = \" value=\"'" + patient.PatientNo + "'\" splice=\"AND\"/>";
                if (!string.IsNullOrEmpty(patient.VisitNo))
                {
                    conditonStr += "<query item=\"INHOSP_INDEX_NO\" compy=\"=\" value=\"'" + patient.VisitNo + "'\" splice=\"AND\"/>";
                }

                var queryMsg = Common.GetText("BS30002手术信息入参.txt");
                queryMsg = queryMsg.Replace("{qeury}", conditonStr);

                var mqClient = MQSDK.SdkClient.Create();
                var response = mqClient.ComposePutAndGetMsg("QMGR.S13", "DC10001", 10000, queryMsg);
                if (response.IsSuccessful)
                {
                    resultMsg = response.Result.Message;
                    if (string.IsNullOrEmpty(resultMsg))
                    {
                        LogUtil.DebugLog("GetOperationData-手术信息返回空");
                        return;
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultMsg);
                    var msgInfoNode = doc.SelectSingleNode("ESBEntry/MsgInfo");
                    if (msgInfoNode == null)
                    {
                        LogUtil.DebugLog("GetOperationData-没有手术信息," + doc.SelectSingleNode("ESBEntry/RetInfo/RetCon").InnerText);
                        return;
                    }

                    var resultXml = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg").InnerText;
                    doc.LoadXml(resultXml);

                    var patientAdt = sqlHelper.GetPatientAdt(patient.VisitNo);

                    patientAdt.OperationName = doc.SelectSingleNode("msg/body/row/SURGERY_OPER_NAME").InnerText;
                    patientAdt.OperationDatetime = Convert.ToDateTime(doc.SelectSingleNode("msg/body/row/SURGERY_END_DATE").InnerText);
                    patientAdt.OperationType = doc.SelectSingleNode("msg/body/row/ANES_METHOD_NAME").InnerText; //麻醉方式

                    patientAdt.ActionType = 3;
                    sqlHelper.InsertPatientAdt(patientAdt);
                }
                else
                {
                    LogUtil.ErrorLog("GetOperationData-手术信息查询失败: response" + response.Msg);
                }
            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("GetOperationData-PatientNo:" + patient.PatientNo + ",获取手术数据报错:" + ex.ToString());
            }
            finally
            {
                if (!string.IsNullOrEmpty(resultMsg))
                {
                    TMessage message = new TMessage();
                    message.MessageContent = resultMsg;
                    message.MessageFormat = "XML";
                    message.MessageType = "OperationData";
                    message.IsRead = 1;
                    sqlHelper.InsertTMessage(message);
                }

            }
        }

        public void GetLisData(PatientQueue patient)
        {
            BrokerRepository sqlHelper = new BrokerRepository();
            var resultMsg = "";
            try
            {
                LogUtil.DebugLog("GetLisData-查询检验信息");
                var conditonStr = "<query item=\"PAT_INDEX_NO\" compy=\" = \" value=\"'" + patient.PatientNo + "'\" splice=\"AND\"/>";
                if (!string.IsNullOrEmpty(patient.VisitNo))
                {
                    conditonStr += "<query item=\"INHOSP_INDEX_NO\" compy=\"=\" value=\"'" + patient.VisitNo + "'\" splice=\"AND\"/>";
                }
                conditonStr += "<query item=\"RECORD_DATE\" compy=\" &gt;= \" value=\"'" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "'\" splice=\"AND\"/>"; 
                var queryMsg = Common.GetText("BS20012检验项目明细入参.txt");
                queryMsg = queryMsg.Replace("{qeury}", conditonStr);

                var mqClient = MQSDK.SdkClient.Create();
                var response = mqClient.ComposePutAndGetMsg("QMGR.S13", "DC10001", 100000, queryMsg);
                if (response.IsSuccessful)
                {
                    resultMsg = response.Result.Message;
                    if (string.IsNullOrEmpty(resultMsg))
                    {
                        LogUtil.DebugLog("GetLisData-检验信息返回为空");
                        return;
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultMsg);

                    var msgInfoNode = doc.SelectSingleNode("ESBEntry/MsgInfo");
                    if (msgInfoNode == null)
                    {
                        LogUtil.DebugLog("GetLisData-没有检验信息," + doc.SelectSingleNode("ESBEntry/RetInfo/RetCon").InnerText);
                        return;
                    }
                    XmlNodeList lisNodes = doc.SelectSingleNode("ESBEntry/MsgInfo").ChildNodes;
                    var patientAdt = sqlHelper.GetPatientAdt(patient.VisitNo);
                    foreach (XmlNode lisNode in lisNodes)
                    {
                        LisADT lis = new LisADT();
                        lis.PatientName = patientAdt.PatientName;
                        lis.VisitNo = patientAdt.VisitNo;
                        lis.Mrn = patientAdt.Mrn;
                        lis.DeptName = patientAdt.DeptName;
                        lis.BedNo = patientAdt.BedNo;

                        doc.LoadXml(lisNode.InnerText);

                        lis.TestNo = doc.SelectSingleNode("msg/body/row/REPORT_NO").InnerText;
                        lis.SampleDatetime = doc.SelectSingleNode("msg/body/row/RECORD_DATE").InnerText;
                        lis.ReportDatetime = doc.SelectSingleNode("msg/body/row/RECORD_DATE").InnerText;
                        lis.SampleTypeCode = doc.SelectSingleNode("msg/body/row/SAMPLE_TYPE_CODE").InnerText;
                        lis.SampleTypeName = doc.SelectSingleNode("msg/body/row/SAMPLE_TYPE_NAME").InnerText;
                        lis.ItemCode = doc.SelectSingleNode("msg/body/row/TEST_ITEM_CODE").InnerText;
                        lis.ItemName = doc.SelectSingleNode("msg/body/row/TEST_ITEM_NAME").InnerText;
                        lis.LisResult = doc.SelectSingleNode("msg/body/row/TEST_RESULT_VALUE").InnerText;
                        lis.UnitName = doc.SelectSingleNode("msg/body/row/TEST_RESULT_VALUE_UNIT").InnerText;
                        lis.Range = doc.SelectSingleNode("msg/body/row/REFERENCE_RANGES").InnerText;
                        lis.IsNormal = doc.SelectSingleNode("msg/body/row/NORMAL_FLAG").InnerText;
                        var valid = doc.SelectSingleNode("msg/body/row/INVALID_FLAG").InnerText;
                        if (valid == "0")
                        {
                            //无效
                            return;
                        }
                        var microbiology = doc.SelectSingleNode("msg/body/row/MICROBE_NAME").InnerText;
                        if (!string.IsNullOrEmpty(microbiology))
                        {
                            lis.ItemName = microbiology;
                            lis.LisResult = doc.SelectSingleNode("msg/body/row/BACTERIAL_COLONY_COUNT").InnerText;
                            lis.DrugSensitivity = doc.SelectSingleNode("msg/body/row/YAOMIN_RESULT").InnerText;
                            lis.IsMicrobiology = 0;
                        }
                        else
                        {
                            lis.IsMicrobiology = 1;
                        }
                        sqlHelper.InsertLisAdt(lis);
                    }
                }
                else
                {
                    LogUtil.ErrorLog("GetLisData-检验信息查询失败: response" + response.Msg);
                }
            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("GetLisData- PatientNo:" + patient.PatientNo + ",获取检验数据报错:" + ex.ToString());
            }
            finally
            {
                if (!string.IsNullOrEmpty(resultMsg))
                {
                    TMessage message = new TMessage();
                    message.MessageContent = resultMsg;
                    message.MessageFormat = "XML";
                    message.MessageType = "LisData";
                    message.IsRead = 1;
                    sqlHelper.InsertTMessage(message);
                }

            }
        }

        public void GetLisBloodData(List<PatientQueue> patients)
        {
            var sqlHelper = new BrokerRepository();
            var lisResults = new List<LisADT>();
            lisResults.AddRange(GetLisBloodDataFromDb(startTime));
            lisResults.AddRange(GetLisBloodDataFromFile(startTime));
            foreach (var lis in lisResults)
            {
                var patient = patients.FirstOrDefault(x => x.BedNo == Convert.ToInt32(lis.BedNo));
                if (patient != null)
                {
                    lis.PatientName = patient.PatientName;
                    lis.BedNo = patient.Bed;
                    lis.VisitNo = patient.VisitNo;
                    lis.Mrn = patient.PatientNo;
                    lis.IsMicrobiology = -1;
                    sqlHelper.InsertLisAdt(lis);
                }
                else
                {
                    LogUtil.DebugLog("GetLisBloodData-没有匹配到患者信息," + lis.BedNo + "; " + lis.PatientName);
                }
            }

        }

        //从血气拿检验信息
        //可以直接从数据库获取
        public List<LisADT> GetLisBloodDataFromDb(DateTime startTime)
        {
            var result = new List<LisADT>();
            try
            {
                var lisHelper = new LisBloodRepository();
                result = lisHelper.GetLisBloodFromDb(startTime);
            }
            catch(Exception ex)
            {
                LogUtil.ErrorLog("GetLisBloodDataFromDb-报错:" + ex.ToString());
            }
            return result;
        }

        //从血气拿检验信息
        //数据库无法读取，从文本里面获取
        public List<LisADT> GetLisBloodDataFromFile(DateTime startTime)
        {
            var result = new List<LisADT>();
            try
            {
                var folder = shareFoder;
                var files = new DirectoryInfo(folder).GetFiles();
                DateTime lastModify = DateTime.MinValue;
                var lastFile = "";
                foreach (FileInfo file in files)
                {
                    if (file.LastWriteTime > lastModify)
                    {
                        lastModify = file.LastWriteTime;
                        lastFile = file.Name;
                    }
                }
                var str = File.ReadAllText(folder + lastFile, Encoding.GetEncoding("gb2312"));
                var array = str.Split(new string[] { "L|1|N" }, StringSplitOptions.None);
                foreach (var lisItem in array)
                {
                    var bed = "";
                    var sampleTypeCode = "";
                    var item = lisItem.Split(new string[] { "\r" }, StringSplitOptions.None);
                    foreach (var subLis in item)
                    {
                        if (subLis.Contains("AQT90"))
                        {
                            var time = DateTime.ParseExact(subLis.Split('|').Last(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                            if (time < startTime)
                            {
                                break;
                            }
                        }
                        if (subLis.StartsWith("P|"))
                        {
                            bed = subLis.Split('|')[3];
                        }
                        if (subLis.StartsWith("O|"))
                        {
                            sampleTypeCode = subLis.Split('|')[3].Split('^')[1];
                        }
                        if (subLis.StartsWith("R|"))
                        {
                            var lisAdt = new LisADT();
                            lisAdt.BedNo = bed;
                            lisAdt.SampleTypeCode = sampleTypeCode;
                            // lisAdt.SampleTypeName= to do
                            lisAdt.TestNo = subLis.Split('|')[11];
                            lisAdt.SampleDatetime = DateTime.ParseExact(subLis.Split('|')[11], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss");
                            lisAdt.ReportDatetime = DateTime.ParseExact(subLis.Split('|')[12], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss");
                            lisAdt.ItemCode = subLis.Split('|')[2].Split('^')[3];
                            // lisAdt.ItemName= to do
                            lisAdt.UnitName = subLis.Split('|')[4];
                            lisAdt.Range = subLis.Split('|')[5];
                            while (lisAdt.Range.EndsWith("^"))
                            {
                                lisAdt.Range = lisAdt.Range.Substring(0, lisAdt.Range.Length - 1);
                            }
                            lisAdt.Range = lisAdt.Range.Replace("^", "-");
                            lisAdt.LisResult = subLis.Split('|')[3];
                            lisAdt.IsNormal = "0";
                            var isNormal = subLis.Split('|')[6];
                            if (isNormal == "H" || isNormal == ">")
                            {
                                lisAdt.IsNormal = "1";
                            }
                            if (isNormal == "L" || isNormal == "<")
                            {
                                lisAdt.IsNormal = "-11";
                            }
                            result.Add(lisAdt);
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                LogUtil.ErrorLog("GetLisBloodDataFromFile-报错:" + ex.ToString());
            }
            if (result.Any())
            {
                var broker = new BrokerRepository();
                var list = broker.GetDictList("血气名称对照");
                foreach(var r in result)
                {
                    var item = list.FirstOrDefault(x => x.DictKey == r.SampleTypeCode);
                    if (item != null)
                    {
                        r.SampleTypeName = item.DictDesc;
                    }
                    item = list.FirstOrDefault(x => x.DictKey == r.ItemCode);
                    if (item != null)
                    {
                        r.ItemName = item.DictDesc;
                    }
                }
            }
            return result;
        }
    }
}
