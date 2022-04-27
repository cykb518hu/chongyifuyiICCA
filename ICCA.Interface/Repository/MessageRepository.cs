using ICCA.Interface.Log;
using ICCA.Interface.Model;
using ICCA.Interface.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ICCA.Interface.Repository
{
    public class MessageRepository
    {
        private ConfigInfo config;
        public MessageRepository(ConfigInfo configInfo)
        {
            config = configInfo;
        }
        public int InsertPatient(string msg, int autoId)
        {
            var result = 1;
            // msg = Common.GetText("PS10003住院登记推送V4.0.txt");
            BrokerRepository sqlHelper = new BrokerRepository();
            try
            {
                HisPatient patient = new HisPatient();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                patient.PatientName = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATNAME").InnerText;
                patient.PatientNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATPATIENTID").InnerText;
                patient.Mrn = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATDOCUMENTNO").InnerText;
                patient.VisitNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMVISITNUMBER").InnerText;
                try
                {
                    patient.Birthday = Convert.ToDateTime(doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATDOB").InnerText);
                }
                catch (Exception ex)
                {
                    patient.Birthday = Convert.ToDateTime("1900-01-01");
                    LogUtil.ErrorLog("生日转化报错:" + ex.ToString());
                }
                patient.Sex = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATSEXDESC").InnerText;
                //身高
                //体重
                //血型
                patient.Telephone = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATTELEPHONE").InnerText;
                //地址
                try
                {
                    patient.AdmissionDatetime = Convert.ToDateTime(doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMSTARTDATE").InnerText + " " + doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMSTARTTIME").InnerText);
                }
                catch (Exception ex)
                {
                    LogUtil.ErrorLog("入院时间报错:" + ex.ToString());
                }

                patient.FeeType = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/FEETYPEDESC").InnerText;

                patient.IdType = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATIDTYPEDESC").InnerText;
                patient.IdNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATIDENTITYNUM").InnerText;
                patient.Marriage = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATMARITALSTATUSDESC").InnerText;
                patient.Nation = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/PATPATIENTINFO/PATNATIONDESC").InnerText;
                //籍贯
                patient.AdmitDoctor = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMADMITDOCDESC").InnerText;
                patient.AttendingDoctorCode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMADMITDOCCODE").InnerText;
                patient.AttendingDoctorName = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMADMITDOCDESC").InnerText;

                //既往史
                //家族史
                //嗜好
                //主诉
                var DeptCode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMADMDEPTCODE").InnerText;
                patient.DeptName = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMADMDEPTDESC").InnerText;
                patient.WardName = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMADMWARDDESC").InnerText;
                patient.BedNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMCURBEDNO").InnerText;

                XmlNode diagnoseNode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTENCOUNTERSTARTEDRT/PAADMDIAGNOSELIST");
                if (diagnoseNode != null)
                {
                    XmlNodeList diagnoseList = diagnoseNode.ChildNodes;
                    if (diagnoseList != null)
                    {
                        foreach (XmlNode diagnose in diagnoseList)
                        {
                            if (diagnose.SelectSingleNode("PADDIAGCATEGORY").InnerText == "主要诊断")
                            {
                                patient.InSubDiagnosis = diagnose.SelectSingleNode("PADDIAGDESC").InnerText;
                                patient.AdmissionDiagnosis = diagnose.SelectSingleNode("PADDIAGDESC").InnerText;

                                patient.AttendingDoctorCode = diagnose.SelectSingleNode("PADDIAGDOCCODE").InnerText;
                                patient.AttendingDoctorName = "";// diagnose.SelectSingleNode("PADDIAGDOCDESC").InnerText;
                            }
                            if (diagnose.SelectSingleNode("PADDIAGTYPEDESC").InnerText == "入院诊断")
                            {
                                patient.AdmissionDiagnosis = diagnose.SelectSingleNode("PADDIAGDESC").InnerText;
                            }
                        }
                    }
                }
                if (patient.InSubDatetime == null)
                {
                    patient.InSubDatetime = patient.AdmissionDatetime;
                }
                sqlHelper.InsertHisPatient(patient);
            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析院登记推送报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;

        }


        public int TransferPatient(string msg, int autoId)
        {
            var result = 1;
            // msg = Common.GetText("PS10005转科转床信息推送V4.0.txt");
            BrokerRepository sqlHelper;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                var deptCode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGDEPTCODE").InnerText; //科室名称没有
                var OldDeptCode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTORIGDEPTCODE").InnerText;
                var visitNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMVISITNUMBER").InnerText;
                var oldBedNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTORIGBEDCODE").InnerText;
                var bedNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGBEDCODE").InnerText;
                var status = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTSTATE").InnerText;
                //患者入院登记没有床位，进来后再分床  01 转出 02 转入 03 换床 04 换医生
                if (deptCode == config.DeptCode && (status == "02" || status == "03"))
                {
                    if (string.IsNullOrEmpty(bedNo))
                    {
                        LogUtil.WarningLog("TransferPatient -入科或者转床 床位为空:" + visitNo);
                        return -2;
                    }
                    if (OldDeptCode == config.DeptCode && !string.IsNullOrEmpty(oldBedNo))
                    {
                        LogUtil.WarningLog("TransferPatient -科室内转床，暂时不处理" + visitNo + "old bed:" + oldBedNo + "; new bed：" + bedNo);
                        return 1;
                    }

                    sqlHelper = new BrokerRepository();
                    var hisPatient = sqlHelper.GetHisPatient(visitNo);
                    if (hisPatient != null)
                    {
                        var patientAdt = new PatientADT();
                        Common.AutoMapping(hisPatient, patientAdt);
                        try
                        {
                            patientAdt.InSubDatetime = Convert.ToDateTime(doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTENDDATE").InnerText + " " + doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTENDTIME").InnerText);
                        }
                        catch (Exception ex)
                        {
                            LogUtil.ErrorLog("入科时间转化报错:" + ex.ToString());
                        }
                        patientAdt.AdmitDoctor = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGDOCCODE").InnerText;
                        patientAdt.AttendingDoctorCode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGDOCCODE").InnerText;
                        patientAdt.AttendingDoctorName = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGDOCCODE").InnerText; //缺失医生名称

                        patientAdt.WardName = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGWARDDESC").InnerText;
                        patientAdt.BedNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTTARGBEDCODE").InnerText;
                        patientAdt.DeptName = Common.GetIccaDeptName(patientAdt.BedNo);
                        patientAdt.PatientSource = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTORIGDEPTCODE").InnerText; //转入科室
                        patientAdt.ActionType = 0;
                        sqlHelper.InsertPatientAdt(patientAdt);
                    }
                }
                //出科
                if (deptCode != config.DeptCode && OldDeptCode == config.DeptCode && status == "01")
                {
                    sqlHelper = new BrokerRepository();
                    var patientAdt = sqlHelper.GetPatientAdt(visitNo);
                    if (patientAdt == null)
                    {
                        LogUtil.WarningLog("TransferPatient-患者不再科室");
                        return result;
                    }
                    patientAdt.ActionType = 2;
                    patientAdt.IsSend = 0;
                    patientAdt.OutSubDatetime = Convert.ToDateTime(doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTSTARTDATE").InnerText + " " + doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADMTRANSACTIONRT/PAADMTSTARTTIME").InnerText);
                    sqlHelper.InsertPatientAdt(patientAdt);
                }

            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析转科转床报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;
        }

        public int DischargePatient(string msg, int autoId)
        {
            //msg = Common.GetText("PS10004出院信息推送 V4.0.txt");
            var result = 1;
            BrokerRepository sqlHelper = new BrokerRepository();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                var deptCode = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTDISCHARGERT/PAADMDISDEPTCODE").InnerText;
                var visitNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTDISCHARGERT/PAADMVISITNUMBER").InnerText;

                //科室内出院
                if (deptCode == config.DeptCode)
                {
                    var patientAdt = sqlHelper.GetPatientAdt(visitNo);
                    if (patientAdt == null)
                    {
                        LogUtil.ErrorLog("DischargeHisPatient-患者不再科室");
                        return 1;
                    }
                    patientAdt.OutSubDatetime = Convert.ToDateTime(doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTDISCHARGERT/PAADMENDDATE").InnerText + " " + doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/INPATIENTDISCHARGERT/PAADMENDTIME").InnerText);
                    patientAdt.ActionType = 2;
                    patientAdt.IsSend = 0;
                    sqlHelper.InsertPatientAdt(patientAdt);
                }
                //  sqlHelper.DeleteHisPatient(visitNo);
            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析出院推送报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;
        }

        public int InsertDrugOrder(string msg, int autoId)
        {
            var result = 1;
            //  msg = Common.GetText("PS35002医嘱信息推送V4.0.txt");
            BrokerRepository sqlHelper;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                var visitNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADDORDERSRT/PAADMVISITNUMBER").InnerText;
                var patientType = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADDORDERSRT/PAADMTYPECODE").InnerText;
                if (!config.InPatientType.Contains(patientType))
                {
                    //如果不是住院患者，返回，减少数据库查询
                    return 1;
                }

                sqlHelper = new BrokerRepository();
                var patient = sqlHelper.GetPatientAdt(visitNo);// sqlHelper.GetPatientAdt(visitNo);
                //只记录重症患者的数据
                if (patient == null)
                {
                    return 1;
                }
                XmlNodeList orderNodes = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/ADDORDERSRT/OEORIINFOLIST").ChildNodes;
                var orders = new List<OrderAdt>();
                foreach (XmlNode orderNode in orderNodes)
                {
                    var orderType = orderNode.SelectSingleNode("OEORICLASS").InnerText;
                    if (!config.DrugOrderType.Contains(orderType))
                    {
                        //如果不是指定医嘱类型，返回，不插入
                        continue;
                    }
                    OrderAdt order = new OrderAdt();
                    order.PatientNo = patient.PatientNo;
                    order.VisitNo = visitNo;
                    order.PatientName = patient.PatientName;
                    order.Mrn = patient.Mrn;
                    order.DeptName = patient.DeptName;
                    order.WardName = patient.WardName;
                    order.BedNo = patient.BedNo;
                    order.CombNo = orderNode.SelectSingleNode("OEORIPARENTORDERID").InnerText;
                    order.OrderId = orderNode.SelectSingleNode("OEORIORDERITEMID").InnerText;
                    if (string.IsNullOrEmpty(order.CombNo))
                    {
                        order.CombNo = order.OrderId;
                    }

                    order.DrugCode = orderNode.SelectSingleNode("OEORIARCITMMASTCODE").InnerText;
                    order.DrugName = orderNode.SelectSingleNode("OEORIARCITMMASTDESC").InnerText;
                    order.Spec = orderNode.SelectSingleNode("OEORISPECIFICATION").InnerText;
                    order.Dose = orderNode.SelectSingleNode("OEORIDOSEQTY").InnerText;
                    order.DoseUnitCode = orderNode.SelectSingleNode("OEORIDOSEUNITCODE").InnerText;
                    order.DoseUnitName = orderNode.SelectSingleNode("OEORIDOSEUNITDESC").InnerText;
                    order.FrequencyCode = orderNode.SelectSingleNode("OEORIFREQCODE").InnerText;
                    order.FrequencyName = orderNode.SelectSingleNode("OEORIFREQDESC").InnerText;
                    order.UsageCode = orderNode.SelectSingleNode("OEORIINSTRCODE").InnerText;
                    order.UsageName = orderNode.SelectSingleNode("OEORIINSTRDESC").InnerText;
                    order.DrugIssueTypeCode = orderNode.SelectSingleNode("OEORIDOSEFORMSCODE").InnerText;
                    order.DrugIssueTypeName = orderNode.SelectSingleNode("OEORIDOSEFORMSDESC").InnerText;
                    order.OrderDoctorCode = orderNode.SelectSingleNode("OEORIENTERDOCCODE").InnerText;
                    order.OrderDoctorName = orderNode.SelectSingleNode("OEORIENTERDOCDESC").InnerText;
                    order.OrderTypeName = orderNode.SelectSingleNode("OEORIPRIORITYDESC").InnerText;
                    order.OrderClassName = orderNode.SelectSingleNode("OEORICLASS").InnerText;
                    order.StopDoctorCode = orderNode.SelectSingleNode("OEORISTOPDOCCODE").InnerText;
                    order.StopDoctorName = orderNode.SelectSingleNode("OEORISTOPDOCDESC").InnerText;
                    order.OrderTime = orderNode.SelectSingleNode("OEORIENTERDATE").InnerText + " " + orderNode.SelectSingleNode("OEORIENTERTIME").InnerText;
                    order.PlanStartTime = orderNode.SelectSingleNode("OEORIREQUIREEXECDATE").InnerText + " " + orderNode.SelectSingleNode("OEORIREQUIREEXECTIME").InnerText;
                    order.PlanStopTime = orderNode.SelectSingleNode("OEORISTOPDATE").InnerText + " " + orderNode.SelectSingleNode("OEORISTOPTIME").InnerText;
                    order.OrderStatus = orderNode.SelectSingleNode("OEORISTATUSCODE").InnerText;
                    if (order.OrderStatus == "V")
                    {
                        order.IccaOrderStatus = "NW";
                    }
                    orders.Add(order);
                    sqlHelper.InsertDrugOrder(order);
                }

            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析医嘱推送报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;
        }


        public int ChangeDrugOrderStatus(string msg, int autoId)
        {
            var result = 1;
            //  msg = Common.GetText("PS35002医嘱信息推送V4.0.txt");

            BrokerRepository sqlHelper;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msg);
                var visitNo = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/UPDATEORDERSRT/PAADMVISITNUMBER").InnerText;
                var stopTime = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/UPDATEORDERSRT/UPDATEDATE").InnerText + " " + doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/UPDATEORDERSRT/UPDATETIME").InnerText;
                sqlHelper = new BrokerRepository();
                var patient = sqlHelper.GetPatientAdt(visitNo);// sqlHelper.GetPatientAdt(visitNo);
                //只记录重症患者的数据
                if (patient == null)
                {
                    return 1;
                }
                XmlNodeList orderNodes = doc.SelectSingleNode("ESBEntry/MsgInfo/Msg/UPDATEORDERSRT/OEORIINFOLIST").ChildNodes;
                foreach (XmlNode orderNode in orderNodes)
                {
                    var orderId = orderNode.SelectSingleNode("OEORIORDERITEMID").InnerText;
                    OrderAdt order = sqlHelper.GetOrderAdt(orderId);
                    if (order != null)
                    {
                        var orderStatus = orderNode.SelectSingleNode("OEORISTATUSCODE").InnerText;
                        if (orderStatus == order.OrderStatus)
                        {
                            //状态没有变化
                            return 1;
                        }
                        order.OrderStatus = orderStatus;
                        if (orderStatus == "D")
                        {
                            order.StopTime = stopTime;
                            order.IccaOrderStatus = "DC";
                        }
                        if (orderStatus == "C")
                        {
                            order.StopTime = stopTime;
                            order.IccaOrderStatus = "CA";
                        }

                        sqlHelper.InsertDrugOrder(order);
                    }

                }

            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析医嘱状态变更报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;
        }
        public int ExecuteDrugOrder(string msg, int autoId)
        {
            var result = 1;
            //  msg = Common.GetText("PS35002医嘱信息推送V4.0.txt");
            BrokerRepository sqlHelper;
            try
            {

            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析医嘱执行报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;
        }

        public int InsertLisCriticalSign(string msg, int autoId)
        {
            var result = 1;
            BrokerRepository sqlHelper;
            try
            {

            }
            catch (Exception ex)
            {
                LogUtil.ErrorLog("解析危机值推送报错,MessageId:" + autoId + "; 错误信息:" + ex.ToString());
                result = -1;
            }
            return result;
        }
    }
}
