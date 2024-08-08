using DocumentFormat.OpenXml.Drawing.Charts;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hamtory_WPF
{
    static class DataList
    {
        private static ProcessData dataLoader = new ProcessData();
        public static List<DataValues> datas { get; private set; }
        static DataList()
        {
            datas = dataLoader.LoadDataFile("melting_tank.csv");
            
        }

    }

}
