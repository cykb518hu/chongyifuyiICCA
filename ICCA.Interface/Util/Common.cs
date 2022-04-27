using ICCA.Interface.Log;
using ICCA.Interface.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ICCA.Interface.Util
{
    public class Common
    {
        public static ConfigInfo LoadServiceConfig()
        {
            ConfigInfo config = new ConfigInfo();

            config.InInterval = Convert.ToInt32(ConfigurationManager.AppSettings["InInterval"].Trim().ToString());
            config.InDrugInterval = Convert.ToInt32(ConfigurationManager.AppSettings["InDrugInterval"].Trim().ToString());
            config.InSearchInterval = Convert.ToInt32(ConfigurationManager.AppSettings["InSearchInterval"].Trim().ToString());
            config.LogLevel = ConfigurationManager.AppSettings["LogLevel"].Trim().ToString();
            config.LogFilePath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["LogFilePath"].Trim().ToString());
            config.LogFileKeepDay = Convert.ToInt32(ConfigurationManager.AppSettings["LogFileKeepDay"].Trim().ToString());

            config.DeptCode = ConfigurationManager.AppSettings["DeptCode"].Trim().ToString();
            config.InPatientType = ConfigurationManager.AppSettings["InPatientType"].Trim().ToString();
            config.DrugOrderType = ConfigurationManager.AppSettings["DrugOrderType"].Trim().ToString();
            config.LisBloodSharePath = ConfigurationManager.AppSettings["LisBloodSharePath"].Trim().ToString();

            LogUtil.Initialize(config.LogFilePath, config.LogLevel, config.LogFileKeepDay);

            return config;
        }

        public static string GetText(string fileName)
        {
            // var path = Path.Combine(System.Environment.CurrentDirectory, "Data\\" + fileName);
            var path = "C:\\File\\ICCA\\重医附一\\IC\\ICCA.Interface\\ICCA.Interface\\Data\\" + fileName;
            //var path = "D:\\integration files\\ICCA.Interface\\ICCA.Interface\\Data\\" + fileName;
            var str = File.ReadAllText(path, Encoding.UTF8);
            return str;
        }

        public static void AutoMapping(HisPatient s, PatientADT t)
        {
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);

                if (targetPP != null && value != null)
                {
                    targetPP.SetValue(t, value, null);
                }
            }
        }
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static string GetIccaDeptName(string bed)
        {
            var deptName = "ICU-A";
            try
            {
                var bedNo = Convert.ToInt32(bed);
                if (bedNo >15)
                {
                    deptName = "ICU-B";
                }
            }
            catch
            {
            }
            return deptName;
        }

    }

    public class ConfigInfo
    {
        public int InInterval { get; set; }
        public int InDrugInterval { get; set; }
        public int InSearchInterval { get; set; }
        public string LogFilePath { get; set; }
        public string LogLevel { get; set; }
        public int LogFileKeepDay { get; set; }

        public string DeptCode { get; set; }
        public string InPatientType { get; set; }
        public string DrugOrderType { get; set; }
        public string LisBloodSharePath { get; set; }
    }
}
