using DocumentFormat.OpenXml.Drawing.Charts;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Hamtory_WPF
{
    static class DataList
    {
        private static ProcessData dataLoader = new ProcessData();
        public static List<DataValues> datas { get; private set; }

        // 비동기 초기화 메서드
        public static async Task InitializeAsync()
        {
            datas = await dataLoader.LoadDataFileAsync("melting_tank.csv");
        }
    }
}
