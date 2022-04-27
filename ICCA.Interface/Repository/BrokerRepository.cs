using ICCA.Interface.Log;
using ICCA.Interface.Model;
using ICCA.Interface.Util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCA.Interface.Repository
{
    public class BrokerRepository
    {
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BrokerCacheDB"].ToString();

        public SqlSugarClient db;//用来处理事务多表查询和复杂的操作 
        public BrokerRepository()
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样
            });

        }
        public void InsertTMessage(TMessage message)
        {
            db.Insertable(message).ExecuteCommand();
        }
        public TMessage GetUnReadTMessage()
        {
            var message = db.Queryable<TMessage>().OrderBy("autoid desc").First(x =>x.IsRead == 0);
            return message;
        }
        public TMessage GetTMessage(int id)
        {
            var message = db.Queryable<TMessage>()
                    .Where(x => x.AutoId == id).First();
            return message;
        }
        public void UpdateTMessage(int id, int read)
        {
            db.Updateable<TMessage>().SetColumns(it => it.IsRead == read).Where(it => it.AutoId == id).ExecuteCommand();
        }

        public void InsertHisPatient(HisPatient patient)
        {
            var existItem = new HisPatient();
            existItem = db.Queryable<HisPatient>()
               .Where(x => x.PatientNo == patient.PatientNo && x.VisitNo == patient.VisitNo)
               .First();
            if (existItem == null)
            {
                db.Insertable(patient).ExecuteCommand();
            }
            else
            {
                LogUtil.DebugLog("患者数据已经存在，不再插入,PatientNo:"+patient.PatientNo+",VisitNo:"+patient.VisitNo);
            }
        }

        public HisPatient GetHisPatient(string visitId)
        {
            var patient = db.Queryable<HisPatient>()
                    .Where(x => x.VisitNo == visitId).OrderBy("CreateTime desc").First();
            return patient;
        }

       
        public void DeleteHisPatient(string visitId)
        {
            var patient = db.Deleteable<HisPatient>()
                    .Where(x => x.VisitNo == visitId).ExecuteCommand();
        }
        public void InsertPatientAdt(PatientADT patient)
        {
            var existItem = new PatientADT();
            existItem = db.Queryable<PatientADT>()
               .Where(x => x.PatientNo == patient.PatientNo && x.VisitNo == patient.VisitNo && x.ActionType == patient.ActionType && patient.ActionType != 3 && patient.ActionType != 1)
               .First();
            if (existItem == null)
            {
                db.Insertable(patient).ExecuteCommand();
            }
            else
            {
                LogUtil.DebugLog("患者数据已经存在，不再插入,PatientNo:"+patient.PatientNo+",VisitNo:"+patient.VisitNo+"ActionType:"+patient.ActionType);

            }
        }
        public PatientADT GetPatientAdt(string visitId)
        {
            var patient = db.Queryable<PatientADT>()
                    .Where(x => x.VisitNo == visitId).OrderBy("CreateTime desc").First();
            return patient;
        }
        public void InsertDrugOrder(OrderAdt order)
        {
            var existItem = new OrderAdt();
            existItem = db.Queryable<OrderAdt>()
               .Where(x => x.OrderId == order.OrderId&&order.OrderStatus==x.OrderStatus)
               .First();
            if (existItem == null)
            {
                db.Insertable(order).ExecuteCommand();
            }
            else
            {
                LogUtil.DebugLog("医嘱数据已经存在，不再插入,Order Id:"+order.OrderId);
            }
        }

        public void InsertLisAdt(LisADT lis)
        {

            var existItem = new LisADT();
            existItem = db.Queryable<LisADT>()
                  .Where(x => x.TestNo == lis.TestNo && x.ItemCode == lis.ItemCode && x.SampleTypeCode == lis.SampleTypeCode)
               .First();
            if (existItem == null)
            {
                db.Insertable(lis).ExecuteCommand();
            }
            else
            {
                LogUtil.DebugLog("检验数据已经存在，不再插入,TestNo:" + lis.TestNo);
            }

        }

        public List<PatientQueue> GetPatientQueue()
        {
            var sql= Common.GetText("GetPatientQueue.txt");
            return db.Ado.SqlQuery<PatientQueue>(sql);
        }

        public OrderAdt GetOrderAdt(string orderId)
        {
            return db.Queryable<OrderAdt>().Where(x => x.OrderId == orderId).OrderBy(x => x.AutoId).First();
        }

        public List<TDictModel> GetDictList(string type)
        {
            return db.Queryable<TDictModel>().Where(x => x.DictType == type).ToList();
        }

        public List<PatientDrugModel> GetPatientDrugList(string patientNo)
        {
            return db.Ado.SqlQuery<PatientDrugModel>("exec dbo.GetPatientDrugList '" + patientNo + "'");
        }
        public TPatientOrderCalculate GetPatientOrderCalculate(string patientId)
        {
            return db.Queryable<TPatientOrderCalculate>().Where(x => x.PatientNo == patientId).OrderBy("createTime desc ").First();
        }
        public void InsertPatientOrderCalculate(TPatientOrderCalculate data)
        {
            db.Insertable(data).ExecuteCommand();
        }

    }

    /// <summary>
    /// 血气
    /// </summary>
    public class LisBloodRepository
    {
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Lis"].ToString();

        public SqlSugarClient db;//用来处理事务多表查询和复杂的操作 
        public LisBloodRepository()
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样
            });
        }

        public List<LisADT> GetLisBloodFromDb(DateTime startTime)
        {
            var sql = "select * from v_sam_icca where ReportDatetime>'" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            var list = db.Ado.SqlQuery<LisADT>(sql);
            return list;
        }


    }
}
