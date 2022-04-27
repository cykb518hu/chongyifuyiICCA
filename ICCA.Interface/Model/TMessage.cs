using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICCA.Interface.Model
{
    [SugarTable("TMessage")]
    public class TMessage
    {
        [SugarColumn(ColumnName = "AutoId", IsPrimaryKey = true, IsIdentity = true)]
        public int AutoId { get; set; }

        public string MessageContent { get; set; }

        public string MessageType { get; set; }

        public string MessageFormat { get; set; }

        public DateTime CreateTime { get { return DateTime.Now; } }

        public int IsRead { get; set; }
    }
}