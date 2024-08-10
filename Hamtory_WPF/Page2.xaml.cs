using CsvHelper;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ClosedXML.Excel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using LiveCharts.Defaults;

namespace Hamtory_WPF
{
    public partial class Page2 : Page
    {
        private DataTable dataTable;
        private DataTable filteredDataTable;

        public Page2()
        {
            InitializeComponent();
            LoadData();  // 페이지 초기화 시 자동으로 데이터를 로드
            DateRangePickerControl.DateRangeChanged += DateRangePickerControl_DateRangeChanged;
            opChart.LoadSampleData();
        }

        public void LoadData()
        {
            // Debug 폴더에 있는 CSV 파일 경로 지정
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDirectory, "melting_tank.csv");

            // 파일이 존재하는지 확인
            if (!File.Exists(filePath))
            {
                MessageBox.Show("melting_tank.csv 파일이 존재하지 않습니다.");
                return;
            }

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    using (var dr = new CsvDataReader(csv))
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        dataTable = dt;
                    }
                }

                // 데이터가 로드된 후 DataGrid에 표시
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    dataGrid.ItemsSource = dataTable.DefaultView;

                }
                else
                {
                    MessageBox.Show("CSV 파일에서 데이터를 로드하지 못했습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류 발생: {ex.Message}");
            }
        }

        private void DateRangePickerControl_DateRangeChanged(object sender, DateRangeChangedEventArgs e)
        {
            StartDateText.Text = e.StartDate.HasValue ? e.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            EndDateText.Text = e.EndDate.HasValue ? e.EndDate.Value.ToString("yyyy-MM-dd") : "End Date";
        }

        private void RawDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("데이터가 로드되지 않았습니다.");
                return;
            }

            DateTime? startDate = DateRangePickerControl.StartDate;
            DateTime? endDate = DateRangePickerControl.EndDate;
            string startTime = txtStartTime.Text;
            string endTime = txtEndTime.Text;

            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime startDateTime = DateTime.ParseExact($"{startDate.Value:yyyy-MM-dd} {startTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                DateTime endDateTime = DateTime.ParseExact($"{endDate.Value:yyyy-MM-dd} {endTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture).AddSeconds(59);

                FilterData(startDateTime, endDateTime);

                if (filteredDataTable != null && filteredDataTable.Rows.Count > 0)
                {
                    var meltTempData = filteredDataTable.AsEnumerable()
                        .Select(row =>
                        {
                            DateTime dateTime;
                            bool isParsed = DateTime.TryParse(row.Field<string>("STD_DT"), out dateTime);
                            return new { Row = row, DateTime = isParsed ? dateTime : (DateTime?)null };
                        })
                        .Where(item => item.DateTime.HasValue && item.DateTime.Value >= startDateTime && item.DateTime.Value <= endDateTime)
                        .GroupBy(item => new DateTime(item.DateTime.Value.Ticks - (item.DateTime.Value.Ticks % TimeSpan.FromMinutes(30).Ticks)))
                        .Select(g => new DateTimePoint(g.Key, g.Average(item => Convert.ToDouble(item.Row["MELT_TEMP"]))))
                        .ToList();

                    var motorSpeedData = filteredDataTable.AsEnumerable()
                        .Select(row =>
                        {
                            DateTime dateTime;
                            bool isParsed = DateTime.TryParse(row.Field<string>("STD_DT"), out dateTime);
                            return new { Row = row, DateTime = isParsed ? dateTime : (DateTime?)null };
                        })
                        .Where(item => item.DateTime.HasValue && item.DateTime.Value >= startDateTime && item.DateTime.Value <= endDateTime)
                        .GroupBy(item => new DateTime(item.DateTime.Value.Ticks - (item.DateTime.Value.Ticks % TimeSpan.FromMinutes(30).Ticks)))
                        .Select(g => new DateTimePoint(g.Key, g.Average(item => Convert.ToDouble(item.Row["MOTORSPEED"]))))
                        .ToList();

                    var meltWeightData = filteredDataTable.AsEnumerable()
                        .Select(row =>
                        {
                            DateTime dateTime;
                            bool isParsed = DateTime.TryParse(row.Field<string>("STD_DT"), out dateTime);
                            return new { Row = row, DateTime = isParsed ? dateTime : (DateTime?)null };
                        })
                        .Where(item => item.DateTime.HasValue && item.DateTime.Value >= startDateTime && item.DateTime.Value <= endDateTime)
                        .GroupBy(item => new DateTime(item.DateTime.Value.Ticks - (item.DateTime.Value.Ticks % TimeSpan.FromMinutes(30).Ticks)))
                        .Select(g => new DateTimePoint(g.Key, g.Average(item => Convert.ToDouble(item.Row["MELT_WEIGHT"]))))
                        .ToList();

                    var inspData = filteredDataTable.AsEnumerable()
                        .Select(row =>
                        {
                            DateTime dateTime;
                            bool isParsed = DateTime.TryParse(row.Field<string>("STD_DT"), out dateTime);
                            return new { Row = row, DateTime = isParsed ? dateTime : (DateTime?)null };
                        })
                        .Where(item => item.DateTime.HasValue && item.DateTime.Value >= startDateTime && item.DateTime.Value <= endDateTime)
                        .GroupBy(item => new DateTime(item.DateTime.Value.Ticks - (item.DateTime.Value.Ticks % TimeSpan.FromMinutes(30).Ticks)))
                        .Select(g => new DateTimePoint(g.Key, g.Average(item => Convert.ToDouble(item.Row["INSP"]))))
                        .ToList();

                    // 차트에 데이터 설정
                    opChart.SetData(meltTempData, motorSpeedData, meltWeightData, inspData);

                    // 원형 차트에 데이터 설정
                    PlotNGDistribution(filteredDataTable);

                    // RawData 윈도우에 데이터 표시
                    RawData rawDataWindow = new RawData();
                    rawDataWindow.dataGrid.ItemsSource = filteredDataTable.DefaultView;
                    rawDataWindow.Show();
                }
                else
                {
                    MessageBox.Show("필터링된 데이터가 없습니다.");
                }
            }
            else
            {
                MessageBox.Show("유효한 날짜를 선택하세요.");
            }
        }

        private void FilterData(DateTime startDateTime, DateTime endDateTime)
        {
            if (dataGrid == null)
            {
                MessageBox.Show("데이터 그리드가 초기화되지 않았습니다.");
                return;
            }

            filteredDataTable = Task.Run(() => GetFilteredData(startDateTime, endDateTime)).Result;

            if (filteredDataTable != null && filteredDataTable.Rows.Count > 0)
            {
                dataGrid.ItemsSource = filteredDataTable.DefaultView;
            }
            else
            {
                dataGrid.ItemsSource = null;
                MessageBox.Show("필터링된 데이터가 없습니다.");
            }
        }

        private DataTable GetFilteredData(DateTime startDateTime, DateTime endDateTime)
        {
            if (dataTable == null)
            {
                MessageBox.Show("데이터가 로드되지 않았습니다.");
                return null;
            }

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

        private void ShowStatistics(DataTable filteredRows)
        {
            var statisticsTable = CalculateStatistics(filteredRows);
            PlotStatisticsGraph(statisticsTable);
        }

        private void ShowNGDistribution(DataTable filteredRows)
        {
            PlotNGDistribution(filteredRows);
        }

        private DataTable CalculateStatistics(DataTable table)
        {
            var statisticsTable = new DataTable();
            statisticsTable.Columns.Add("Statistic");
            statisticsTable.Columns.Add("MELT_TEMP");
            statisticsTable.Columns.Add("MOTORSPEED");
            statisticsTable.Columns.Add("MELT_WEIGHT");
            statisticsTable.Columns.Add("INSP");

            var columns = new[] { "MELT_TEMP", "MOTORSPEED", "MELT_WEIGHT", "INSP" };

            var averages = columns.Select(column => table.AsEnumerable().Average(row => Convert.ToDouble(row[column]))).ToArray();
            var medians = columns.Select(column => table.AsEnumerable().Median(row => Convert.ToDouble(row[column]))).ToArray();
            var stdDevs = columns.Select(column => table.AsEnumerable().StandardDeviation(row => Convert.ToDouble(row[column]))).ToArray();
            var maxValues = columns.Select(column => table.AsEnumerable().Max(row => Convert.ToDouble(row[column]))).ToArray();
            var minValues = columns.Select(column => table.AsEnumerable().Min(row => Convert.ToDouble(row[column]))).ToArray();

            statisticsTable.Rows.Add("평균", FormatNumber(averages[0]), FormatNumber(averages[1]), FormatNumber(averages[2]), FormatNumber(averages[3]));
            statisticsTable.Rows.Add("중앙값", FormatNumber(medians[0]), FormatNumber(medians[1]), FormatNumber(medians[2]), FormatNumber(medians[3]));
            statisticsTable.Rows.Add("표준편차", FormatNumber(stdDevs[0]), FormatNumber(stdDevs[1]), FormatNumber(stdDevs[2]), FormatNumber(stdDevs[3]));
            statisticsTable.Rows.Add("최대값", FormatNumber(maxValues[0]), FormatNumber(maxValues[1]), FormatNumber(maxValues[2]), FormatNumber(maxValues[3]));
            statisticsTable.Rows.Add("최소값", FormatNumber(minValues[0]), FormatNumber(minValues[1]), FormatNumber(minValues[2]));

            return statisticsTable;
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
                LabelPoint = point => columnName == "INSP" ? point.Y.ToString("F2") : point.Y.ToString("F0")
            };

            chart.Series.Clear();
            chart.Series.Add(lineSeries);

            chart.AxisX.Clear();
            chart.AxisX.Add(new Axis
            {
                Title = title,
                Labels = table.Rows.Cast<DataRow>().Select(row => row["Statistic"].ToString()).ToArray(),
                FontSize = 16
            });

            chart.AxisY.Clear();
            chart.AxisY.Add(new Axis
            {
                Title = "Values",
                LabelFormatter = value => columnName == "INSP" ? value.ToString("F2") : value.ToString("F0"),
                FontSize = 16
            });
        }

        private void PlotNGDistribution(DataTable table)
        {
            int okCount = table.AsEnumerable().Count(row => row["TAG"].ToString() == "OK");
            int ngCount = table.AsEnumerable().Count(row => row["TAG"].ToString() == "NG");
            int totalCount = okCount + ngCount;

            double okPercentage = (double)okCount / totalCount * 100;
            double ngPercentage = (double)ngCount / totalCount * 100;

            ngPieChartControl.ngPieChart.Series.Clear();
            ngPieChartControl.ngPieChart.Series.Add(new PieSeries
            {
                Title = "OK",
                Values = new ChartValues<int> { okCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Blue),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });
            ngPieChartControl.ngPieChart.Series.Add(new PieSeries
            {
                Title = "NG",
                Values = new ChartValues<int> { ngCount },
                DataLabels = true,
                Fill = new SolidColorBrush(Colors.Red),
                LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P1})"
            });

            ngPieChartControl.txtOKCount.Text = $"OK: {okCount} ({okPercentage:F1}%)";
            ngPieChartControl.txtNGCount.Text = $"NG: {ngCount} ({ngPercentage:F1}%)";
        }

        private string FormatNumber(double number)
        {
            return number % 1 == 0 ? number.ToString("F0") : number.ToString("F2");
        }
    }

    public static class DataTableExtensions
    {
        public static double StandardDeviation(this EnumerableRowCollection<DataRow> rows, Func<DataRow, double> selector)
        {
            var values = rows.Select(selector).ToArray();
            var average = values.Average();
            var sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            var sd = Math.Sqrt(sumOfSquaresOfDifferences / values.Length);
            return sd;
        }

        public static double Median(this EnumerableRowCollection<DataRow> rows, Func<DataRow, double> selector)
        {
            var orderedValues = rows.Select(selector).OrderBy(val => val).ToArray();
            int count = orderedValues.Length;
            if (count == 0)
                throw new InvalidOperationException("빈 컬렉션의 중앙값은 정의되지 않습니다.");

            double median;
            if (count % 2 == 0)
            {
                median = (orderedValues[count / 2 - 1] + orderedValues[count / 2]) / 2;
            }
            else
            {
                median = orderedValues[count / 2];
            }

            return median;
        }
    }
}
