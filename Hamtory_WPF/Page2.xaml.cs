using CsvHelper;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace Hamtory_WPF
{
    public partial class Page2 : Page
    {
        private DataTable dataTable;
        private DataTable filteredDataTable;

        public Page2()
        {
            InitializeComponent();
            LoadData();
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
                {
                    using (var dr = new CsvDataReader(csv))
                    {
                        var dt = new DataTable();
                        dt.Load(dr);
                        dataTable = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 중 오류 발생: {ex.Message}");
            }
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (dataTable == null)
            {
                MessageBox.Show("데이터를 먼저 로드하세요.");
                return;
            }

            try
            {
                if (datePickerStart.SelectedDate.HasValue && datePickerEnd.SelectedDate.HasValue)
                {
                    DateTime startDate = datePickerStart.SelectedDate.Value;
                    DateTime endDate = datePickerEnd.SelectedDate.Value;

                    string startTime = txtStartTime.Text;
                    string endTime = txtEndTime.Text;

                    if (DateTime.TryParseExact(startDate.ToString("yyyy-MM-dd") + " " + startTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDateTime) &&
                        DateTime.TryParseExact(endDate.ToString("yyyy-MM-dd") + " " + endTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDateTime))
                    {
                        filteredDataTable = await Task.Run(() =>
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
                        });

                        Dispatcher.Invoke(() => dataGrid.ItemsSource = filteredDataTable.Rows.Count > 0 ? filteredDataTable.DefaultView : null);
                    }
                    else
                    {
                        MessageBox.Show("유효한 시간을 HH:mm 형식으로 입력하세요.");
                    }
                }
                else
                {
                    MessageBox.Show("유효한 날짜를 선택하세요.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 필터링 중 오류 발생: {ex.Message}");
            }
        }

        private void BtnShowStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (filteredDataTable != null)
            {
                var statisticsTable = CalculateStatistics(filteredDataTable);
                var statisticsWindow = new StatisticsWindow(statisticsTable);
                statisticsWindow.Show();
            }
            else
            {
                MessageBox.Show("먼저 데이터를 필터링하세요.");
            }
        }

        private void BtnShowNGDistribution_Click(object sender, RoutedEventArgs e)
        {
            if (filteredDataTable != null)
            {
                var ngDistributionWindow = new NGDistributionWindow(filteredDataTable);
                ngDistributionWindow.Show();
            }
            else
            {
                MessageBox.Show("먼저 데이터를 필터링하세요.");
            }
        }

        private void BtnShowLineChart_Click(object sender, RoutedEventArgs e)
        {
            if (filteredDataTable != null)
            {
                var lineChartWindow = new LineChartWindow(GetHourlyIntervalData(filteredDataTable), 60); // 기본값으로 60분 사용
                lineChartWindow.Show();
            }
            else
            {
                MessageBox.Show("먼저 데이터를 필터링하세요.");
            }
        }

        private DataTable GetHourlyIntervalData(DataTable dataTable)
        {
            var resultTable = dataTable.Clone();
            DateTime lastAddedTime = DateTime.MinValue;

            foreach (DataRow row in dataTable.Rows)
            {
                if (DateTime.TryParse(row["STD_DT"].ToString(), out DateTime rowDateTime))
                {
                    if (lastAddedTime == DateTime.MinValue || (rowDateTime - lastAddedTime).TotalMinutes >= 90)
                    {
                        resultTable.ImportRow(row);
                        lastAddedTime = rowDateTime;
                    }
                }
            }

            return resultTable;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.ItemsSource == null)
            {
                MessageBox.Show("저장할 데이터가 없습니다.");
                return;
            }

            var dataView = dataGrid.ItemsSource as DataView;
            var dataTable = dataView.ToTable();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV 파일 (*.csv)|*.csv|Excel 파일 (*.xlsx)|*.xlsx",
                Title = "데이터 저장"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath);

                try
                {
                    if (fileExtension == ".csv")
                    {
                        using (var writer = new StreamWriter(filePath))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(dataTable.AsEnumerable());
                        }
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            workbook.Worksheets.Add(dataTable, "Data");
                            workbook.SaveAs(filePath);
                        }
                    }

                    MessageBox.Show("데이터가 성공적으로 저장되었습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"데이터 저장 중 오류 발생: {ex.Message}");
                }
            }
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
            statisticsTable.Rows.Add("최소값", FormatNumber(minValues[0]), FormatNumber(minValues[1]), FormatNumber(minValues[2]), FormatNumber(minValues[3]));

            return statisticsTable;
        }

        private string FormatNumber(double number)
        {
            return number % 1 == 0 ? number.ToString("F0") : number.ToString("F2");
        }

        //private void BtnUpdateChart_Click(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(txtInterval.Text, out int interval))
        //    {
        //        var intervalDataTable = GetHourlyIntervalData(filteredDataTable); // 여기에 interval 값을 적용
        //        var lineChartWindow = new LineChartWindow(intervalDataTable, interval);
        //        lineChartWindow.Show();
        //    }
        //    else
        //    {
        //        MessageBox.Show("유효한 분 단위를 입력하세요.");
        //    }
        //}
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
