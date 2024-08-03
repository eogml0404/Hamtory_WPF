using LiveCharts;
using LiveCharts.Wpf;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hamtory_WPF
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(DataTable statisticsTable)
        {
            InitializeComponent();
            PlotStatisticsGraph(statisticsTable);
        }

        private void PlotStatisticsGraph(DataTable table)
        {
            PlotSingleGraph(table, "Melt Temp", "MELT_TEMP", meltTempChart, Colors.Blue);
            PlotSingleGraph(table, "Motor Speed", "MOTORSPEED", motorSpeedChart, Colors.Red);
            PlotSingleGraph(table, "Melt Weight", "MELT_WEIGHT", meltWeightChart, Colors.Green);
            PlotSingleGraph(table, "INSP", "INSP", inspChart, Colors.Purple);
        }

        private void PlotSingleGraph(DataTable table, string title, string columnName, CartesianChart chart, Color color)
        {
            var lineSeries = new LineSeries
            {
                Title = title,
                Values = new ChartValues<double>(table.Rows.Cast<DataRow>().Select(row => Convert.ToDouble(row[columnName]))),
                Stroke = new SolidColorBrush(color),
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 10,
                DataLabels = true,
                LabelPoint = point => columnName == "INSP" ? point.Y.ToString("F2") : point.Y.ToString("F0") // 소수점 조건부 표시
            };

            chart.Series.Clear();
            chart.Series.Add(lineSeries);

            chart.AxisX.Clear();
            chart.AxisX.Add(new Axis
            {
                Title = title,
                Labels = table.Rows.Cast<DataRow>().Select(row => row["Statistic"].ToString()).ToArray(),
                FontSize = 16 // X축 폰트 크기 설정
            });

            chart.AxisY.Clear();
            chart.AxisY.Add(new Axis
            {
                Title = "Values",
                LabelFormatter = value => columnName == "INSP" ? value.ToString("F2") : value.ToString("F0"), // 소수점 조건부 표시
                FontSize = 16 // Y축 폰트 크기 설정
            });
        }
    }
}
