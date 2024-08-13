using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
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

            meltTempSeries.PointGeometrySize = 1; // 포인트 크기 설정
            motorSpeedSeries.PointGeometrySize = 1;
            meltWeightSeries.PointGeometrySize = 1;
            inspSeries.PointGeometrySize = 1;

            meltTempSeries.Stroke = new SolidColorBrush(Color.FromRgb(200, 50, 50));
            motorSpeedSeries.Stroke = new SolidColorBrush(Color.FromRgb(200, 110, 220));
            meltWeightSeries.Stroke = new SolidColorBrush(Color.FromArgb(180, 200, 180, 100));
            inspSeries.Stroke = new SolidColorBrush(Color.FromRgb(12, 130, 210));

            meltTempSeries.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            motorSpeedSeries.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            meltWeightSeries.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            inspSeries.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 165, 0));

        }


        public void LoadDataWithInterval(List<DataValues> data, int intervalMinutes = 90)
        {
            var filteredData = FilterDataByInterval(data, intervalMinutes);

            var meltTempData = new ChartValues<double>(filteredData.Select(d => (double)d.melt_temperature));
            var motorSpeedData = new ChartValues<double>(filteredData.Select(d => (double)d.motor_speed));
            var meltWeightData = new ChartValues<double>(filteredData.Select(d => (double)d.melt_weight));
            var inspData = new ChartValues<double>(filteredData.Select(d => d.moisture));

            SetData(meltTempData, motorSpeedData, meltWeightData, inspData);

            xAxis.Labels = filteredData.Select(d => d.date.ToString("yy-MM-dd HH:mm")).ToArray();

            // xAxis 레이블 포맷터 설정
            xAxis.LabelFormatter = value => DateTime.FromOADate(value).ToString("yy-MM-dd HH:mm");
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
