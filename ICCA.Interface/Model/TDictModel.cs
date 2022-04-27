using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICCA.Interface.Model
{
    [SugarTable("TDict")]
    public  class TDictModel
    {
        public string DictType { get; set; }
        public string DictKey { get; set; }
        public string DictDesc { get; set; }
    }
}
