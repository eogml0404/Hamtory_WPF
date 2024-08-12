using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CsvHelper;
using LiveCharts;
using LiveCharts.Wpf;

namespace Hamtory_WPF
{
    public partial class Page2UserControl : UserControl
    {
        private DataTable dataTable;
        private DataTable filteredDataTable;

        public Page2UserControl()
        {
            InitializeComponent();
            LoadData();
            opChartControl.LoadSampleData();  // 초기 샘플 데이터 로드
            DisplayExampleNgPieChart();
            statisticsChartControl.DisplayExampleChart();  // 예시 차트 표시
        }

        public void LoadData()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDirectory, "melting_tank.csv");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("melting_tank.csv 파일이 존재하지 않습니다.");
                return;
            }

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                using (var dr = new CsvDataReader(csv))
                {
                    var dt = new DataTable();
                    dt.Load(dr);
                    dataTable = dt;
                }

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("CSV 파일에서 데이터를 로드하지 못했습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류 발생: {ex.Message}");
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = StartDatePicker.SelectedDate.Value;
               
            }
            else
            {
                txtStartTime.Text = "00:00";
                txtEndTime.Text = "23:59";
            }
        }

        private void RawDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("데이터가 로드되지 않았습니다.");
                return;
            }

            DateTime? selectedDate = StartDatePicker.SelectedDate;
            string startTime = txtStartTime.Text;
            string endTime = txtEndTime.Text;

            if (selectedDate.HasValue)
            {
                try
                {
                    DateTime startDateTime = DateTime.ParseExact($"{selectedDate.Value:yyyy-MM-dd} {startTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    DateTime endDateTime = DateTime.ParseExact($"{selectedDate.Value:yyyy-MM-dd} {endTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddSeconds(59);

                    FilterData(startDateTime, endDateTime);

                    if (filteredDataTable != null && filteredDataTable.Rows.Count > 0)
                    {
                        var rawDataWindow = new RawData();
                        rawDataWindow.dataGrid.ItemsSource = filteredDataTable.DefaultView;
                        rawDataWindow.Show();

                        var statisticsTable = CalculateStatistics(filteredDataTable);

                        string[] categories = new[] { "MELT_TEMP", "MOTORSPEED", "MELT_WEIGHT" };
                        statisticsChartControl.DisplayStatisticsAndChart("Statistics", statisticsTable, categories);

                        PlotNGDistribution(filteredDataTable);

                        var times = AggregateTimes(filteredDataTable);
                        var meltTempData = AggregateData(filteredDataTable, "MELT_TEMP");
                        var motorSpeedData = AggregateData(filteredDataTable, "MOTORSPEED");
                        var meltWeightData = AggregateData(filteredDataTable, "MELT_WEIGHT");
                        var inspData = AggregateData(filteredDataTable, "INSP");

                        opChartControl.SetData(times, meltTempData, motorSpeedData, meltWeightData, inspData);
                    }
                    else
                    {
                        MessageBox.Show("필터링된 데이터가 없습니다.");
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"날짜 및 시간 형식 오류: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("유효한 날짜를 선택하세요.");
            }
        }

        private void DisplayExampleNgPieChart()
        {
            ngPieChartControl.ngPieChart.Series.Clear();
            ngPieChartControl.ngPieChart.Series.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "OK (예시)",
                Values = new ChartValues<int> { 70 },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Blue),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });
            ngPieChartControl.ngPieChart.Series.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "NG (예시)",
                Values = new ChartValues<int> { 30 },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Red),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });

            ngPieChartControl.txtOKCount.Text = $"OK: 70 (70.0%)";
            ngPieChartControl.txtNGCount.Text = $"NG: 30 (30.0%)";

            ngPieChartControl.txtOKCount.Visibility = Visibility.Collapsed;
            ngPieChartControl.txtNGCount.Visibility = Visibility.Collapsed;
        }

        private void FilterData(DateTime startDateTime, DateTime endDateTime)
        {
            filteredDataTable = GetFilteredData(startDateTime, endDateTime);

            if (filteredDataTable == null || filteredDataTable.Rows.Count == 0)
            {
                MessageBox.Show("필터링된 데이터가 없습니다.");
            }
        }

        private DataTable GetFilteredData(DateTime startDateTime, DateTime endDateTime)
        {
            var filteredDataTable = dataTable.Clone();

            foreach (DataRow row in dataTable.Rows)
            {
                if (DateTime.TryParse(row["STD_DT"].ToString(), out DateTime rowDateTime))
                {
                    if (rowDateTime >= startDateTime && rowDateTime <= endDateTime)
                    {
                        filteredDataTable.ImportRow(row);
                    }
                }
            }

            return filteredDataTable;
        }

        private List<DateTime> AggregateTimes(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(row =>
                        {
                            DateTime dateTime;
                            if (DateTime.TryParse(row.Field<string>("STD_DT"), out dateTime))
                            {
                                return new DateTime(dateTime.Ticks / TimeSpan.TicksPerMinute / 120 * 120 * TimeSpan.TicksPerMinute);
                            }
                            else
                            {
                                return DateTime.MinValue;
                            }
                        })
                        .Distinct()
                        .ToList();
        }

        private List<double> AggregateData(DataTable table, string columnName)
        {
            return table.AsEnumerable()
                        .GroupBy(row =>
                        {
                            DateTime dateTime;
                            if (DateTime.TryParse(row.Field<string>("STD_DT"), out dateTime))
                            {
                                return new DateTime(dateTime.Ticks / TimeSpan.TicksPerMinute / 120 * 120 * TimeSpan.TicksPerMinute);
                            }
                            else
                            {
                                return DateTime.MinValue;
                            }
                        })
                        .Select(g => g.Average(r =>
                        {
                            double value;
                            if (double.TryParse(r.Field<string>(columnName), out value))
                            {
                                return value;
                            }
                            else
                            {
                                return 0.0;
                            }
                        }))
                        .ToList();
        }

        private DataTable CalculateStatistics(DataTable table)
        {
            var statisticsTable = new DataTable();
            statisticsTable.Columns.Add("Metric");
            statisticsTable.Columns.Add("MELT_TEMP");
            statisticsTable.Columns.Add("MOTORSPEED");
            statisticsTable.Columns.Add("MELT_WEIGHT");

            var meanRow = statisticsTable.NewRow();
            meanRow["Metric"] = "Mean";
            var medianRow = statisticsTable.NewRow();
            medianRow["Metric"] = "Median";
            var stdDevRow = statisticsTable.NewRow();
            stdDevRow["Metric"] = "Std Dev";
            var maxRow = statisticsTable.NewRow();
            maxRow["Metric"] = "Max";
            var minRow = statisticsTable.NewRow();
            minRow["Metric"] = "Min";

            foreach (var column in new[] { "MELT_TEMP", "MOTORSPEED", "MELT_WEIGHT" })
            {
                var data = table.AsEnumerable().Select(row => Convert.ToDouble(row[column])).ToList();
                meanRow[column] = data.Average().ToString("0.00");
                medianRow[column] = data.Median();
                stdDevRow[column] = data.StandardDeviation().ToString("0.00");
                maxRow[column] = data.Max();
                minRow[column] = data.Min();
            }

            statisticsTable.Rows.Add(meanRow);
            statisticsTable.Rows.Add(medianRow);
            statisticsTable.Rows.Add(stdDevRow);
            statisticsTable.Rows.Add(maxRow);
            statisticsTable.Rows.Add(minRow);

            return statisticsTable;
        }

        private void PlotNGDistribution(DataTable table)
        {
            int okCount = table.AsEnumerable().Count(row => row["TAG"].ToString() == "OK");
            int ngCount = table.AsEnumerable().Count(row => row["TAG"].ToString() == "NG");
            int totalCount = okCount + ngCount;

            double okPercentage = (double)okCount / totalCount * 100;
            double ngPercentage = (double)ngCount / totalCount * 100;

            ngPieChartControl.ngPieChart.Series.Clear();
            ngPieChartControl.ngPieChart.Series.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "OK",
                Values = new ChartValues<int> { okCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Blue),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });
            ngPieChartControl.ngPieChart.Series.Add(new LiveCharts.Wpf.PieSeries
            {
                Title = "NG",
                Values = new ChartValues<int> { ngCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Red),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });

            ngPieChartControl.txtOKCount.Visibility = Visibility.Collapsed;
            ngPieChartControl.txtNGCount.Visibility = Visibility.Collapsed;
        }
    }

    public static class DataTableExtensions
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            return Math.Sqrt(sumOfSquaresOfDifferences / values.Count());
        }

        public static double Median(this IEnumerable<double> values)
        {
            var sortedValues = values.OrderBy(v => v).ToArray();
            int count = sortedValues.Length;
            if (count == 0)
                throw new InvalidOperationException("Empty collection");

            double median = count % 2 == 0
                ? (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2
                : sortedValues[count / 2];

            return median;
        }

        public static double LowerQuartile(this IEnumerable<double> values)
        {
            var sortedValues = values.OrderBy(v => v).ToArray();
            int count = sortedValues.Length;
            if (count == 0)
                throw new InvalidOperationException("Empty collection");

            double lowerQuartile = sortedValues[(int)(count * 0.25)];
            return lowerQuartile;
        }

        public static double UpperQuartile(this IEnumerable<double> values)
        {
            var sortedValues = values.OrderBy(v => v).ToArray();
            int count = sortedValues.Length;
            if (count == 0)
                throw new InvalidOperationException("Empty collection");

            double upperQuartile = sortedValues[(int)(count * 0.75)];
            return upperQuartile;
        }
    }
}
