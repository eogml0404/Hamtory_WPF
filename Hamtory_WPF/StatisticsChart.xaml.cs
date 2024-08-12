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

        public void DisplayExampleChart()
        {
            // 예시 차트 데이터 설정
            var exampleSeries = new ColumnSeries
            {
                Title = "Example",
                Values = new ChartValues<double> { 250, 300, 150 }
            };

            StatisticsChartControl.Series.Clear();  // 기존 시리즈 지우기
            StatisticsChartControl.Series.Add(exampleSeries);

            // X축이 초기화되지 않았을 경우 새로 추가
            if (StatisticsChartControl.AxisX.Count == 0)
            {
                StatisticsChartControl.AxisX.Add(new Axis());
            }

            // 예시 카테고리 설정
            StatisticsChartControl.AxisX[0].Labels = new[] { "Category1", "Category2", "Category3" }.ToList();
            StatisticsChartControl.AxisX[0].Title = "Categories";
        }


        public void DisplayStatisticsAndChart(string title, DataTable statisticsTable, string[] categories)
        {
            var seriesCollection = new SeriesCollection();

            var meanSeries = new ColumnSeries
            {
                Title = "Mean",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 0, true))
            };

            var medianSeries = new ColumnSeries
            {
                Title = "Median",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 1, false))
            };

            var stdDevSeries = new ColumnSeries
            {
                Title = "Std Dev",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 2, true))
            };

            var maxSeries = new ColumnSeries
            {
                Title = "Max",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 3, false))
            };

            var minSeries = new ColumnSeries
            {
                Title = "Min",
                Values = new ChartValues<double>(GetStatisticsValues(statisticsTable, 4, false))
            };

            seriesCollection.Add(meanSeries);
            seriesCollection.Add(medianSeries);
            seriesCollection.Add(stdDevSeries);
            seriesCollection.Add(maxSeries);
            seriesCollection.Add(minSeries);

            StatisticsChartControl.Series.Clear();  // 기존 시리즈 지우기
            StatisticsChartControl.Series = seriesCollection;
            StatisticsChartControl.AxisX[0].Labels = categories.ToList();
            StatisticsChartControl.AxisX[0].Title = "";  // "Categories" 제거
        }

        private List<double> GetStatisticsValues(DataTable statisticsTable, int rowIndex, bool roundToTwoDecimalPlaces)
        {
            return statisticsTable.Rows[rowIndex].ItemArray.Skip(1).Select(v =>
            {
                if (double.TryParse(v?.ToString(), out double result))
                {
                    return roundToTwoDecimalPlaces ? Math.Round(result, 2) : result;
                }
                else
                {
                    return 0.0;
                }
            }).ToList();
        }
    }
}
