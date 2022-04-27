using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ICCA.Interface.Model
{
    [SugarTable("TOrder")]
    public class OrderAdt
    {
        [SugarColumn(ColumnName = "AutoId", IsPrimaryKey = true, IsIdentity = true)]
        public int AutoId { get; set; }
        /// <summary>
        /// 必填
        /// 病人姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 病人唯一号
        /// </summary>
        public string PatientNo { get; set; }

        /// <summary>
        /// 必填
        /// 病历号
        /// </summary>
        public string Mrn { get; set; }

        /// <summary>
        /// 住院流水号
        /// </summary>
        public string VisitNo { get; set; }

        /// <summary>
        /// 目前科室
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 病房
        /// </summary>
        public string WardName { get; set; }

        /// <summary>
        /// 病床号
        /// </summary>
        public string BedNo { get; set; }

        /// <summary>
        /// 医嘱可以多次，这个orderid是每一次id，单次唯一
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 医嘱单组号 --东华的医嘱组号 多次用药的id相同
        /// </summary>
        public string CombNo { get; set; }

        /// <summary>
        /// 药品代码 必填
        /// </summary>
        public string DrugCode { get; set; }

        /// <summary>
        /// 药品名称 必填
        /// </summary>
        public string DrugName { get; set; }


        /// <summary>
        /// 商品名
        /// </summary>
        public string CommodityName { get; set; }

        /// <summary>
        /// 规格 是
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 剂量 必填
        /// </summary>
        public string Dose { get; set; }


        /// <summary>
        /// 剂量单位代码 必填
        /// </summary>
        public string DoseUnitCode { get; set; }

        /// <summary>
        /// 剂量单位名称 必填
        /// </summary>
        public string DoseUnitName { get; set; }

        /// <summary>
        /// 频次代码 必填
        /// </summary>
        public string FrequencyCode { get; set; }

        /// <summary>
        /// 频次名称 必填
        /// </summary>
        public string FrequencyName { get; set; }

        /// <summary>
        /// 用法代码 必填
        /// </summary>
        public string UsageCode { get; set; }


        /// <summary>
        /// 用法名称 必填
        /// </summary>
        public string UsageName { get; set; }


        /// <summary>
        /// 剂型代码
        /// </summary>
        public string DrugIssueTypeCode { get; set; }

        /// <summary>
        /// 剂型名称
        /// </summary>
        public string DrugIssueTypeName { get; set; }

        /// <summary>
        /// 用药部位代码
        /// </summary>
        public string PositionCode { get; set; }

        /// <summary>
        /// 用药部位名称
        /// </summary>
        public string PositionName { get; set; }


        /// <summary>
        /// 开医嘱医生代码
        /// </summary>
        public string OrderDoctorCode { get; set; }

        /// <summary>
        /// 开医嘱医生名称
        /// </summary>
        public string OrderDoctorName { get; set; }

        /// <summary>
        /// 医嘱下达时间 必填
        /// </summary>
        public string OrderTime { get; set; }

        /// <summary>
        /// 医嘱类型：长期医嘱，临时医嘱 必填
        /// </summary>
        public string OrderTypeName { get; set; }
        /// <summary>
        /// 医嘱类别，例如：西药 必填
        /// </summary>
        public string OrderClassName { get; set; }

        /// <summary>
        /// 停医嘱医生代码
        /// </summary>
        public string StopDoctorCode { get; set; }

        /// <summary>
        /// 停医嘱医生名称
        /// </summary>
        public string StopDoctorName { get; set; }

        /// <summary>
        /// 计划开始时间 必填
        /// </summary>
        public string PlanStartTime { get; set; }

        /// <summary>
        /// 计划停止时间
        /// </summary>
        public string PlanStopTime { get; set; }
        /// <summary>
        /// 医嘱停止操作时间
        /// </summary>
        public string StopTime { get; set; }
        /// <summary>
        /// 医嘱执行条形码
        /// </summary>
        public string BarCode { get; set; }


        /// <summary>
        /// 医嘱执行时间
        /// </summary>
        public string ExecuteTime { get; set; }


        /// <summary>
        /// 执行护士代码
        /// </summary>
        public string ExecuteNurseCode { get; set; }


        /// <summary>
        /// 执行护士姓名
        /// </summary>
        public string ExecuteNurseName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark1 { get; set; }

        /// <summary>
        /// 医嘱状态；例如：已确认，已停止 必填
        /// </summary>
        public string OrderStatus { get; set; }

        public string IccaOrderStatus { get; set; }

        public DateTime CreateTime { get { return DateTime.Now; } }

        public int IsSend { get; set; }


    }
    [SugarTable("tOrderDict")]
    public class OrderDictionary
    {
        public string DictType { get; set; }
        public string ICCADictCode { get; set; }
        public string HISDictCode { get; set; }
        public string HISDictDesc { get; set; }
        public string Spec { get; set; }
    }

    public class PatientDrugModel
    {
        public string lifetimeNumber { get; set; }
        public string encounterNumber { get; set; }
        public string PropName { get; set; }
        public DateTime ChartTime { get; set; }
        public string Value
        {
            get
            {
                var result = "";
                if (!string.IsNullOrEmpty(this.Value_V))
                {
                    var startIndex = this.Value_V.IndexOf("(");
                    var endIndex = this.Value_V.IndexOf(")");
                    result = this.Value_V.Substring(startIndex, endIndex - startIndex+1);
                }
                return result;
            }
        }
        public string Value_V { get; set; }
        public string Desc_V { get; set; }
        public string ptDescriptorId { get; set; }
    }
    [SugarTable("TPatientOrderCalculate")]
    public class TPatientOrderCalculate
    {
        public string PatientNo { get; set; }
        /// <summary>
        /// 抗感染药物
        /// </summary>
        public string Kgryw { get; set; }
        /// <summary>
        /// 心血管类药物
        /// </summary>
        public string Xghxyw { get; set; }
        /// <summary>
        /// 镇静镇痛药物
        /// </summary>
        public string Ztzjyw { get; set; }
        /// <summary>
        /// 其他药物
        /// </summary>
        public string Qttsyw { get; set; }

        /// <summary>
        /// 病情观察记录
        /// </summary>
        public string Bqgcjl { get; set; }

    }
}