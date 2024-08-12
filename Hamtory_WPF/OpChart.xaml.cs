using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace Hamtory_WPF
{
    public partial class OpChart : UserControl
    {
        public OpChart()
        {
            InitializeComponent();
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

            // Enable smooth curves for a trend line effect
            meltTempSeries.LineSmoothness = 0.9;
            motorSpeedSeries.LineSmoothness = 0.9;
            meltWeightSeries.LineSmoothness = 0.9;
            inspSeries.LineSmoothness = 0.9;
        }

        public void LoadDataWithInterval(List<DataValues> data, int intervalMinutes = 90)
        {
            var filteredData = FilterDataByInterval(data, intervalMinutes);

            var meltTempData = new ChartValues<double>(filteredData.Select(d => (double)d.melt_temperature));
            var motorSpeedData = new ChartValues<double>(filteredData.Select(d => (double)d.motor_speed));
            var meltWeightData = new ChartValues<double>(filteredData.Select(d => (double)d.melt_weight));
            var inspData = new ChartValues<double>(filteredData.Select(d => d.moisture));

            SetData(meltTempData, motorSpeedData, meltWeightData, inspData);

            xAxis.Labels = filteredData.Select(d => d.date.ToString("HH:mm")).ToArray();
        }

        private List<DataValues> FilterDataByInterval(List<DataValues> data, int intervalMinutes)
        {
            var result = new List<DataValues>();
            DateTime lastAddedTime = DateTime.MinValue;

            foreach (var item in data)
            {
                if (lastAddedTime == DateTime.MinValue || (item.date - lastAddedTime).TotalMinutes >= intervalMinutes)
                {
                    result.Add(item);
                    lastAddedTime = item.date;
                }
            }

            return result;
        }
    }
}
