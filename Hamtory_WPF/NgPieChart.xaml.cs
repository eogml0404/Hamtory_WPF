using LiveCharts;
using LiveCharts.Wpf;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hamtory_WPF
{
    public partial class NgPieChart : UserControl
    {
        public NgPieChart()
        {
            InitializeComponent();
        }

        public void SetData(DataTable dataTable)
        {
            int okCount = dataTable.AsEnumerable().Count(row => row["TAG"].ToString() == "OK");
            int ngCount = dataTable.AsEnumerable().Count(row => row["TAG"].ToString() == "NG");

            if (okCount + ngCount == 0)
            {
                MessageBox.Show("NG/OK 데이터가 없습니다.");
                return;
            }

            ngPieChart.Series.Clear();
            ngPieChart.Series.Add(new PieSeries
            {
                Title = "OK",
                Values = new ChartValues<int> { okCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Blue),
                FontSize = 20,
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });
            ngPieChart.Series.Add(new PieSeries
            {
                Title = "NG",
                Values = new ChartValues<int> { ngCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Red),
                FontSize = 20,  
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });
        }

    }
}
