using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICCA.Interface.Model
{

    public class PatientQueue
    {
        public string PatientNo { get; set; }
        public string VisitNo { get; set; }
        public string PatientName { get; set; }
        public string Bed { get; set; }
        public int BedNo
        {
            get
            {
                int result = 0;
                try
                {
                    result = Convert.ToInt32(Bed.Replace("ICU", ""));
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
    }


    [SugarTable("TPatient")]
    public class PatientADT: BasicPatient
    {
        /// <summary>
        /// 操作类型
        /// 01   0	入科	A01
        /// 03   1	转床 A02
        /// 02   2	出科 A03
        /// 04   3	更新 A08
        /// </summary>
        public int ActionType { get; set; }

        public DateTime LastUpdateDatetime { get { return DateTime.Now;  } }

        public DateTime CreateTime { get { return DateTime.Now; } }
        public int IsSend { get; set; }

        public int RetryNum { get { return 0; } }
    }
}