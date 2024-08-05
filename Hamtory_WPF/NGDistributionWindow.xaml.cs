using LiveCharts;
using LiveCharts.Wpf;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hamtory_WPF
{
    public partial class NGDistributionWindow : Window
    {
        public NGDistributionWindow(DataTable table)
        {
            InitializeComponent();
            PlotNGDistribution(table);
        }

        private void PlotNGDistribution(DataTable table)
        {
            int okCount = table.AsEnumerable().Count(row => row["TAG"].ToString() == "OK");
            int ngCount = table.AsEnumerable().Count(row => row["TAG"].ToString() == "NG");
            int totalCount = okCount + ngCount;

            double okPercentage = (double)okCount / totalCount * 100;
            double ngPercentage = (double)ngCount / totalCount * 100;

            ngPieChart.Series.Clear();
            ngPieChart.Series.Add(new PieSeries
            {
                Title = "OK",
                Values = new ChartValues<int> { okCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Blue),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });
            ngPieChart.Series.Add(new PieSeries
            {
                Title = "NG",
                Values = new ChartValues<int> { ngCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Red),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });

            txtOKCount.Text = $"OK: {okCount} ({okPercentage:F1}%)";
            txtNGCount.Text = $"NG: {ngCount} ({ngPercentage:F1}%)";
        }
    }
}
