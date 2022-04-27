using ICCA.Interface.Log;
using ICCA.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCA.Interface.Repository
{
    
    public class CalculateRepository
    {
        public void RunCalculate()
        {
            LogUtil.DebugLog("RunCalculate-开始");
            BrokerRepository sqlHelper = new BrokerRepository();
            var patientList = sqlHelper.GetPatientQueue();
            if (patientList.Any())
            {
                foreach (var patient in patientList)
                {
                    CaculateDrug(patient.PatientNo);
                }
            }
            else
            {
                LogUtil.DebugLog("RunCalculate-没有在科患者");
            }
            LogUtil.DebugLog("RunCalculate-结束");
        }

        public void CaculateDrug(string patientNo)
        {
            try
            {
                BrokerRepository broker = new BrokerRepository();
                var drugList = broker.GetPatientDrugList(patientNo);
                var kgryw = GetDrugStr("抗感染药物", drugList);
                var xghxyw = GetDrugStr("心血管类药物", drugList);
                var ztzjyw = GetDrugStr("镇静镇痛药物", drugList);
                var qttsyw = GetDrugStr("其他药物", drugList);
                var bqgcjl = GetSpecialRecordStr(drugList);
                if (!string.IsNullOrEmpty(kgryw) || !string.IsNullOrEmpty(xghxyw) || !string.IsNullOrEmpty(ztzjyw) || !string.IsNullOrEmpty(qttsyw) || !string.IsNullOrEmpty(bqgcjl))
                {

                    bool flag = false;
                    var data = broker.GetPatientOrderCalculate(patientNo);
                    if (data == null)
                    {
                        data = new TPatientOrderCalculate { PatientNo = patientNo, Kgryw = kgryw, Xghxyw = xghxyw, Ztzjyw = ztzjyw, Qttsyw = qttsyw, Bqgcjl = bqgcjl };
                        broker.InsertPatientOrderCalculate(data);
                    }
                    else
                    {
                        if (kgryw != data.Kgryw)
                        {
                            data.Kgryw = kgryw;
                            flag = true;
                        }
                        if (xghxyw != data.Xghxyw)
                        {
                            data.Xghxyw = xghxyw;
                            flag = true;
                        }
                        if (ztzjyw != data.Ztzjyw)
                        {
                            data.Ztzjyw = ztzjyw;
                            flag = true;
                        }
                        if (qttsyw != data.Qttsyw)
                        {
                            data.Qttsyw = qttsyw;
                            flag = true;
                        }
                        if (bqgcjl != data.Bqgcjl)
                        {
                            data.Bqgcjl = bqgcjl;
                            flag = true;
                        }
                        if (flag)
                        {
                            broker.InsertPatientOrderCalculate(data);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogUtil.DebugLog("CaculateDrug-报错,"+ex.ToString());
            }



        }

        public string GetDrugStr(string type, List<PatientDrugModel> drugList)
        {
            BrokerRepository broker = new BrokerRepository();
            var result = "";
            var list = broker.GetDictList(type);
            if (drugList.Any())
            {
                foreach (var drug in drugList)
                {
                    foreach (var r in list)
                    {
                        if (drug.Desc_V.Contains(r.DictDesc))
                        {
                            var value = r.DictDesc + drug.Value;
                            if (!result.Contains(value))
                            {
                                result += value + ";";
                            }
                        }
                    }
                }
                if (result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }
         
            return result;
        }

        public string GetSpecialRecordStr(List<PatientDrugModel> drugList)
        {
            var result = "";
            drugList = drugList.OrderByDescending(x => x.ChartTime).ToList();
            foreach(var group in drugList.GroupBy(x => x.ptDescriptorId))
            {
                var subList = drugList.Where(x => x.ptDescriptorId == group.Key).ToList();
                if (subList.Any() && subList.Count > 1)
                {
                    if (subList[0].Value != subList[1].Value)
                    {
                        result += "遵医嘱调整("+subList[0].Desc_V + ")速率由" + subList[1].Value_V + "变为" + subList[0].Value_V;
                    }
                }
            }
            return result;
        }
    }
}
