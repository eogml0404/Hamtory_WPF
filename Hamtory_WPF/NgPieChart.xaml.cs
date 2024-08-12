using System.Windows.Controls;

namespace Hamtory_WPF
{
    public partial class NgPieChart : UserControl
    {
        public NgPieChart()
        {
            InitializeComponent();
        }

        public void UpdateChart(double okPercentage, double ngPercentage, int okCount, int ngCount)
        {
            ngPieChartControl.Series.Clear();

            // 소수점 두 번째 자리까지만 표시
            string okPercentageFormatted = okPercentage.ToString("F2");
            string ngPercentageFormatted = ngPercentage.ToString("F2");

            var okSeries = new LiveCharts.Wpf.PieSeries
            {
                Title = "OK",
                Values = new LiveCharts.ChartValues<double> { okPercentage },
                Fill = System.Windows.Media.Brushes.Blue,
                DataLabels = true, // 데이터 레이블 표시
                FontSize = 20,
                LabelPoint = chartPoint => $"{okCount} ({okPercentageFormatted}%)" // 레이블 텍스트 설정: 개수 (비율)
            };

            var ngSeries = new LiveCharts.Wpf.PieSeries
            {
                Title = "NG",
                Values = new LiveCharts.ChartValues<double> { ngPercentage },
                Fill = System.Windows.Media.Brushes.Red,
                DataLabels = true, // 데이터 레이블 표시
                FontSize = 20,
                LabelPoint = chartPoint => $"{ngCount} ({ngPercentageFormatted}%)" // 레이블 텍스트 설정: 개수 (비율)
            };

            ngPieChartControl.Series.Add(okSeries);
            ngPieChartControl.Series.Add(ngSeries);
        }
    }
}
