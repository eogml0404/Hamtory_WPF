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
            var seriesCollection = new SeriesCollection();

            var meanSeries = new ColumnSeries
            {
                Title = "Mean",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 0))
            };

            var medianSeries = new ColumnSeries
            {
                Title = "Median",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 1))
            };

            var stdDevSeries = new ColumnSeries
            {
                Title = "Std Dev",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 2))
            };

            var maxSeries = new ColumnSeries
            {
                Title = "Max",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 3))
            };

            var minSeries = new ColumnSeries
            {
                Title = "Min",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 4))
            };

            seriesCollection.Add(meanSeries);
            seriesCollection.Add(medianSeries);
            seriesCollection.Add(stdDevSeries);
            seriesCollection.Add(maxSeries);
            seriesCollection.Add(minSeries);

            StatisticsChartControl.Series = seriesCollection;
            StatisticsChartControl.AxisX[0].Labels = categories.ToList();
            StatisticsChartControl.AxisX[0].Title = "";  // "Categories" 제거
        }

        private List<double> GetStatisticsValues(DataTable statisticsTable, int rowIndex)
        {
            return statisticsTable.Rows[rowIndex].ItemArray.Skip(1).Select(v =>
            {
                if (double.TryParse(v?.ToString(), out double result))
                {
                    return result;
                }
                else
                {
                    return 0.0; // 기본값으로 0.0을 사용하거나, 다른 값을 지정할 수 있습니다.
                }
            }).ToList();
        }
    }
}
