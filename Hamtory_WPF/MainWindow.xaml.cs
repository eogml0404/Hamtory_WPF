using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        private DataTable dataTable;

        public MainWindow()
        {
            InitializeComponent();
            LoadCsvFile(); // 윈도우가 초기화될 때 CSV 파일을 자동으로 로드
        }

        private void LoadCsvFile()
        {
            try
            {
                // CSV 파일의 절대 경로를 설정합니다.
                string relativePath = "melting_tank.csv"; // CSV 파일의 이름
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

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
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
                        var filteredRows = dataTable.AsEnumerable()
                            .Where(row =>
                            {
                                if (DateTime.TryParse(row["STD_DT"].ToString(), out DateTime rowDateTime))
                                {
                                    // 날짜와 시간 비교
                                    return rowDateTime >= startDateTime && rowDateTime <= endDateTime;
                                }
                                return false;
                            });

                        if (filteredRows.Any())
                        {
                            DataTable filteredTable = filteredRows.CopyToDataTable();
                            dataGrid.ItemsSource = filteredTable.DefaultView;
                        }
                        else
                        {
                            dataGrid.ItemsSource = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid times in HH:mm format.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select valid dates.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering data: {ex.Message}");
            }
        }
    }
}
