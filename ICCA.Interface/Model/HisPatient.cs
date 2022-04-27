using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICCA.Interface.Model
{
    public class BasicPatient
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
        /// 出身日期 必填
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 性别 必填
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        ///  身高(cm)
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        ///  体重(Kg)(入科)
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        ///  血型
        /// </summary>
        public string BloodType { get; set; }

        /// <summary>
        ///  联系电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        ///  家庭住址
        /// </summary>
        public string Address { get; set; }


        /// <summary>
        /// 入院时间
        /// </summary>
        public DateTime? AdmissionDatetime { get; set; }

        /// <summary>
        /// 入院诊断
        /// </summary>
        public string AdmissionDiagnosis { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string IdType { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string IdNo { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string Marriage { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 收治医生
        /// </summary>
        public string AdmitDoctor { get; set; }

        /// <summary>
        /// 既往史
        /// </summary>
        public string PastHistory { get; set; }

        /// <summary>
        /// 家族史
        /// </summary>
        public string FamilyHistory { get; set; }
        /// <summary>
        /// 嗜好
        /// </summary>
        public string Habit { get; set; }

        /// <summary>
        /// 嗜好
        /// </summary>
        public string ChiefComplaint { get; set; }

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
        /// 入科时间
        /// </summary>
        public DateTime? InSubDatetime { get; set; }

        /// <summary>
        /// 入科诊断
        /// </summary>
        public string InSubDiagnosis { get; set; }

        /// <summary>
        /// 主诊医师工号
        /// </summary>
        public string AttendingDoctorCode { get; set; }

        /// <summary>
        /// 主诊医师
        /// </summary>
        public string AttendingDoctorName { get; set; }

        /// <summary>
        /// 管床医师工号
        /// </summary>
        public string CareproviderCode { get; set; }

        /// <summary>
        /// 管床医师
        /// </summary>
        public string CareproviderName { get; set; }

        /// <summary>
        /// 病人来源
        /// </summary>
        public string PatientSource { get; set; }

        /// <summary>
        /// 过敏信息
        /// </summary>
        public string allergyHistory { get; set; }

        /// <summary>
        /// 手术类型
        /// </summary>
        public string OperationType { get; set; }
        /// <summary>
        /// 手术时间
        /// </summary>
        public DateTime? OperationDatetime { get; set; }
        /// <summary>
        /// 手术名称
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// 计划转入
        /// </summary>
        public string PlanTransfer { get; set; }

        /// <summary>
        /// 出科时间
        /// </summary>
        public DateTime? OutSubDatetime { get; set; }

        /// <summary>
        /// 出科诊断
        /// </summary>
        public string OutSubDiagnosis { get; set; }

        /// <summary>
        /// 出科医嘱
        /// </summary>
        public string OutSubOrder { get; set; }

        /// <summary>
        /// 出院时间
        /// </summary>
        public string DischargeTime { get; set; }

        /// <summary>
        /// 出院转归
        /// </summary>
        public string DischargeReturn { get; set; }

        
    }

    [SugarTable("HisPatient")]
    public class HisPatient:BasicPatient
    {
        
    }
}