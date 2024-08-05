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

namespace Hamtory_WPF
{
    public partial class MainWindow2 : Window
    {
        private DataTable dataTable;

        public MainWindow2()
        {
            LoadCsvFile();
        }

        private void LoadCsvFile()
        {
            try
            {
                string relativePath = @"C:\Users\dksje\source\repos\Hamtory_WPF\Hamtory_WPF\bin\Debug\melting_tank.csv";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                if (File.Exists(fullPath))
                {
                    using (var reader = new StreamReader(fullPath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<dynamic>().ToList();
                        dataTable = new DataTable();

                        if (records.Count > 0)
                        {
                            foreach (var header in ((IDictionary<string, object>)records[0]).Keys)
                            {
                                dataTable.Columns.Add(header);
                            }

                            foreach (var record in records)
                            {
                                var row = dataTable.NewRow();
                                foreach (var kvp in (IDictionary<string, object>)record)
                                {
                                    row[kvp.Key] = kvp.Value;
                                }
                                dataTable.Rows.Add(row);
                            }

                            dataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("CSV file not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CSV file: {ex.Message}");
            }
        }

        private async void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
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
                        var filteredRows = await Task.Run(() =>
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

                        if (filteredRows.Rows.Count > 0)
                        {
                            Dispatcher.Invoke(() => dataGrid.ItemsSource = filteredRows.DefaultView);
                        }
                        else
                        {
                            Dispatcher.Invoke(() => dataGrid.ItemsSource = null);
                        }
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
            if (dataGrid.ItemsSource != null)
            {
                var filteredDataTable = ((DataView)dataGrid.ItemsSource).ToTable();
                var statisticsTable = CalculateStatistics(filteredDataTable);
                var statisticsWindow = new StatisticsWindow(statisticsTable);
                statisticsWindow.Show();
            }
            else
            {
                MessageBox.Show("필터링된 데이터가 없습니다.");
            }
        }

        private void BtnShowNGDistribution_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.ItemsSource != null)
            {
                var filteredDataTable = ((DataView)dataGrid.ItemsSource).ToTable();
                var ngDistributionWindow = new NGDistributionWindow(filteredDataTable);
                ngDistributionWindow.Show();
            }
            else
            {
                MessageBox.Show("필터링된 데이터가 없습니다.");
            }
        }

        private void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.ItemsSource != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save an Excel File";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var filteredDataTable = ((DataView)dataGrid.ItemsSource).ToTable();
                        wb.Worksheets.Add(filteredDataTable, "FilteredData");
                        wb.SaveAs(saveFileDialog.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("필터링된 데이터가 없습니다.");
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
