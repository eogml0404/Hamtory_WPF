using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace Hamtory_WPF
{
    public partial class StatisticsChart : UserControl
    {
        public StatisticsChart()
        {
            InitializeComponent();
        }

        public void DisplayStatisticsAndChart(string title, DataTable statisticsTable, string[] categories)
        {
            // 카테고리 순서 설정
            categories = new[] { "Temperature", "MotorSpeed", "Weight" };

            var seriesCollection = new SeriesCollection();

            var meanSeries = new ColumnSeries
            {
                Title = "Mean",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, "Mean"))
            };

            var medianSeries = new ColumnSeries
            {
                Title = "Median",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, "Median"))
            };

            var stdDevSeries = new ColumnSeries
            {
                Title = "Std Dev",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, "Std Dev"))
            };

            var maxSeries = new ColumnSeries
            {
                Title = "Max",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, "Max"))
            };

            var minSeries = new ColumnSeries
            {
                Title = "Min",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, "Min"))
            };

            seriesCollection.Add(meanSeries);
            seriesCollection.Add(medianSeries);
            seriesCollection.Add(stdDevSeries);
            seriesCollection.Add(maxSeries);
            seriesCollection.Add(minSeries);

            StatisticsChartControl.Series = seriesCollection;

            // AxisX 설정
            if (StatisticsChartControl.AxisX.Count == 0)
            {
                StatisticsChartControl.AxisX.Add(new Axis());
            }

            // 카테고리 레이블 설정
            StatisticsChartControl.AxisX[0].Labels = categories.ToList();
            StatisticsChartControl.AxisX[0].Title = "";  // "

            // AxisY 설정
            StatisticsChartControl.AxisY[0].Title = "Value";
        }

        private List<double> GetStatisticsValues(DataTable statisticsTable, string metric)
        {
            var row = statisticsTable.Rows.Cast<DataRow>().FirstOrDefault(r => r["Metric"].ToString() == metric);
            if (row != null)
            {
                // 각 열에 대해 값을 가져오며, 소수점 두 자리로 반올림
                return new List<double>
                {
                    Math.Round(Convert.ToDouble(row["MeltTemperature"]), 2),
                    Math.Round(Convert.ToDouble(row["MotorSpeed"]), 2),
                    Math.Round(Convert.ToDouble(row["MeltWeight"]), 2)
                };
            }
            return new List<double> { 0, 0, 0 };
        }
    }
}
