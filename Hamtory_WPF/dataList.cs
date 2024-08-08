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
            
            List<DateTime> date_datas = new List<DateTime>();
            List<int> melt_temperature_datas = new List<int>();
            List<int> index = new List<int>();
            List<int> motor_speed = new List<int>();
            List<int> melt_weight = new List<int>();
            List<double> moisture = new List<double>();

            foreach (var data in datas){
                date_datas.Add(data.date);
                melt_temperature_datas.Add(data.melt_temperature);
                index.Add(data.index);
                motor_speed.Add(data.motor_speed);
                melt_weight.Add(data.melt_weight);
                moisture.Add(data.melt_weight);
            }
        }

    }

}
