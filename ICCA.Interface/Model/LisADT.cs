using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICCA.Interface.Model
{
    [SugarTable("TLisReport")]
    public class LisADT
    {
        [SugarColumn(ColumnName = "AutoId", IsPrimaryKey = true, IsIdentity = true)]
        public int AutoId { get; set; }

        /// <summary>
        /// 必填
        /// 病人姓名
        /// </summary>
        public string PatientName { get; set; }


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
        /// 病床号
        /// </summary>
        public string BedNo { get; set; }




        /// <summary>
        /// 样本时间
        /// </summary>
        public string SampleDatetime { get; set; }

        /// <summary>
        /// 获取样本时间
        /// </summary>
        public string GetSampleDatetime { get; set; }

        /// <summary>
        /// 报告时间
        /// </summary>
        public string ReportDatetime { get; set; }

        /// <summary>
        /// 样本类型编码
        /// </summary>
        public string SampleTypeCode { get; set; }
        /// <summary>
        /// 样本类型名称
        /// </summary>
        public string SampleTypeName { get; set; }

        /// <summary>
        /// 检验组编码
        /// </summary>
        public string GroupCode { get; set; }
        /// <summary>
        /// 检验组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 检验单号
        /// </summary>
        public string TestNo { get; set; }

        /// <summary>
        /// 检验项目字典编号 或  微生物（细菌）编号 要转化
        /// </summary>
        public string ItemCode { get; set; }


  
        /// <summary>
        /// 检验项目名称 或 微生物（细菌）名称
        /// </summary>
        public string ItemName { get; set; }


        /// <summary>
        /// 检验结果 或  细菌计数（菌落数）
        /// </summary>
        public string LisResult { get; set; }
        /// <summary>
        /// （微生物）药敏结果
        /// </summary>
        public string DrugSensitivity { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 参考范围
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        /// 是否正常
        /// -1	结果低于正常值
        /// 0	结果在正常范围
        /// 1	结果超过正常值
        /// </summary>
        public string IsNormal { get; set; }

        /// <summary>
        /// 是否微生物 0 是 1 否
        /// </summary>
        public int IsMicrobiology { get; set; }



        public DateTime CreateTime { get { return DateTime.Now; } }

        /// <summary>
        /// -5 表示没找到匹配内容
        /// </summary>
        public int IsSend { get; set; }

    }
    [SugarTable("tLISItemDict")]
    public class LisDictionary
    {

        public string SampleTypeId { get; set; }
        public string TestItemCode { get; set; }
        public string HL7ItemCode { get; set; }
        [SugarColumn(ColumnName = "TestItemName")]
        public string ItemICCAName { get; set; }
    }
}