using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CsvHelper;
using LiveCharts;
using LiveCharts.Wpf;

namespace Hamtory_WPF
{
    public partial class OpChart : UserControl
    {
        public OpChart()
        {
            InitializeComponent();
            // Initialize the chart with default settings if needed
        }

        public void SetData(ChartValues<double> meltTempData,
                            ChartValues<double> motorSpeedData,
                            ChartValues<double> meltWeightData,
                            ChartValues<double> inspData)
        {
            meltTempSeries.Values = meltTempData;
            motorSpeedSeries.Values = motorSpeedData;
            meltWeightSeries.Values = meltWeightData;
            inspSeries.Values = inspData;
        }

        public void LoadSampleData()
        {
            // Example of sample data
            var meltTempData = new ChartValues<double> { 1, 2, 3, 4, 5 };
            var motorSpeedData = new ChartValues<double> { 5, 4, 3, 2, 1 };
            var meltWeightData = new ChartValues<double> { 2, 3, 4, 5, 6 };
            var inspData = new ChartValues<double> { 6, 5, 4, 3, 2 };

            SetData(meltTempData, motorSpeedData, meltWeightData, inspData);
        }
    }
}