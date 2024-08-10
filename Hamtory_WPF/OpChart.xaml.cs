using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace Hamtory_WPF
{
    public partial class OpChart : UserControl
    {
        public OpChart()
        {
            InitializeComponent();

            chart.Zoom = ZoomingOptions.X; // X축만 확대/축소
            chart.Pan = PanningOptions.X;  // X축만 팬
        }

        public void SetData(List<DateTimePoint> meltTempData,
                            List<DateTimePoint> motorSpeedData,
                            List<DateTimePoint> meltWeightData,
                            List<DateTimePoint> inspData)
        {
            // 데이터 설정
            meltTempSeries.Values = new ChartValues<DateTimePoint>(meltTempData);
            motorSpeedSeries.Values = new ChartValues<DateTimePoint>(motorSpeedData);
            meltWeightSeries.Values = new ChartValues<DateTimePoint>(meltWeightData);
            inspSeries.Values = new ChartValues<DateTimePoint>(inspData);

            // 소수점 설정: INSP만 소수점 둘째 자리까지, 나머지는 정수로 표시
            meltTempSeries.LabelPoint = point => point.Y.ToString("F0");
            motorSpeedSeries.LabelPoint = point => point.Y.ToString("F0");
            meltWeightSeries.LabelPoint = point => point.Y.ToString("F0");
            inspSeries.LabelPoint = point => point.Y.ToString("F2");

            xAxis.LabelFormatter = value => new DateTime((long)value).ToString("HH:mm");
        }

        public void LoadSampleData()
        {
            var now = DateTime.Now;

            var meltTempData = new List<DateTimePoint>
            {
                new DateTimePoint(now, 1),
                new DateTimePoint(now.AddMinutes(30), 2),
                new DateTimePoint(now.AddMinutes(60), 3),
                new DateTimePoint(now.AddMinutes(90), 4),
                new DateTimePoint(now.AddMinutes(120), 5)
            };

            var motorSpeedData = new List<DateTimePoint>
            {
                new DateTimePoint(now, 5),
                new DateTimePoint(now.AddMinutes(30), 4),
                new DateTimePoint(now.AddMinutes(60), 3),
                new DateTimePoint(now.AddMinutes(90), 2),
                new DateTimePoint(now.AddMinutes(120), 1)
            };

            var meltWeightData = new List<DateTimePoint>
            {
                new DateTimePoint(now, 2),
                new DateTimePoint(now.AddMinutes(30), 3),
                new DateTimePoint(now.AddMinutes(60), 4),
                new DateTimePoint(now.AddMinutes(90), 5),
                new DateTimePoint(now.AddMinutes(120), 6)
            };

            var inspData = new List<DateTimePoint>
            {
                new DateTimePoint(now, 6),
                new DateTimePoint(now.AddMinutes(30), 5),
                new DateTimePoint(now.AddMinutes(60), 4),
                new DateTimePoint(now.AddMinutes(90), 3),
                new DateTimePoint(now.AddMinutes(120), 2)
            };

            // 데이터 설정
            SetData(meltTempData, motorSpeedData, meltWeightData, inspData);
        }
    }
}
