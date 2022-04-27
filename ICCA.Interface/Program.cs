using ICCA.Interface.Repository;
using ICCA.Interface.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ICCA.Interface
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //var config = Common.LoadServiceConfig();
            //CalculateRepository calculateRepository = new CalculateRepository();
            //calculateRepository.RunCalculate();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                 new ReadMQ(),
                 new SearchService(),
                 new DrugService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
