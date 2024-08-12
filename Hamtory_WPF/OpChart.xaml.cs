using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Hamtory_WPF
{
    public partial class OperationalChartControl : UserControl
    {
        public OperationalChartControl()
        {
            InitializeComponent();
            InitializeChart();
        }

        private void InitializeChart()
        {
            // X축과 Y축 둘 다 줌과 팬을 가능하도록 설정
            cartesianChart.Zoom = ZoomingOptions.Xy;  // X축과 Y축 모두 줌 가능
            cartesianChart.Pan = PanningOptions.Xy;   // X축과 Y축 모두 팬 가능
        }

        public void SetData(List<DateTime> times,
                            List<double> meltTempData,
                            List<double> motorSpeedData,
                            List<double> meltWeightData,
                            List<double> inspData)
        {
            // Melt Temp Series 설정
            meltTempSeries.Values = new ChartValues<ObservablePoint>(
                meltTempData.Select((value, index) => new ObservablePoint(times[index].Ticks, value))
            );
            meltTempSeries.LabelPoint = point => $"{point.Y:F0}"; // 소수점 없이 정수로 표시

            // Motor Speed Series 설정
            motorSpeedSeries.Values = new ChartValues<ObservablePoint>(
                motorSpeedData.Select((value, index) => new ObservablePoint(times[index].Ticks, value))
            );
            motorSpeedSeries.LabelPoint = point => $"{point.Y:F0}"; // 소수점 없이 정수로 표시

            // Melt Weight Series 설정
            meltWeightSeries.Values = new ChartValues<ObservablePoint>(
                meltWeightData.Select((value, index) => new ObservablePoint(times[index].Ticks, value))
            );
            meltWeightSeries.LabelPoint = point => $"{point.Y:F0}"; // 소수점 없이 정수로 표시

            // INSP Series 설정
            inspSeries.Values = new ChartValues<ObservablePoint>(
                inspData.Select((value, index) => new ObservablePoint(times[index].Ticks, value))
            );
            inspSeries.LabelPoint = point => $"{point.Y:F2}"; // 소수점 둘째 자리까지 표시

            // X축 포맷 설정
            cartesianChart.AxisX[0].LabelFormatter = value => new DateTime((long)value).ToString("HH:mm");
            cartesianChart.AxisY[0].LabelFormatter = value => value.ToString("F0"); // Y축 값 소수점 없이 표시
        }

        public void LoadSampleData()
        {
            var now = DateTime.Now;

            var times = new List<DateTime>
            {
                now,
                now.AddMinutes(30),
                now.AddMinutes(60),
                now.AddMinutes(90),
                now.AddMinutes(120)
            };

            var meltTempData = new List<double> { 1, 2, 3, 4, 5 };
            var motorSpeedData = new List<double> { 5, 4, 3, 2, 1 };
            var meltWeightData = new List<double> { 2, 3, 4, 5, 6 };
            var inspData = new List<double> { 6, 5, 4, 3, 2 };

            SetData(times, meltTempData, motorSpeedData, meltWeightData, inspData);
        }
    }
}
