using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hamtory_WPF
{
    public partial class Page2 : Page
    {
        private List<DataValues> _filteredData;

        public Page2()
        {
            InitializeComponent();

            DateRangePickerControl.StartDate = new DateTime(2020, 3, 16);
            DateRangePickerControl.EndDate = new DateTime(2020, 3, 20);

            UpdateChartsAndGraphs();

            // 이벤트 핸들러 등록
            DateRangePickerControl.EndDateChanged += OnEndDateChanged;
        }

        private void OnEndDateChanged(object sender, DateRangeChangedEventArgs e)
        {
            // 종료 날짜가 변경되었을 때 그래프와 차트를 업데이트합니다.
            UpdateChartsAndGraphs();
        }

        private void RawDataButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedStartDate = DateRangePickerControl.StartDate;
            DateTime? selectedEndDate = DateRangePickerControl.EndDate;

            string startTime = txtStartTime.Text;
            string endTime = txtEndTime.Text;

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                try
                {
                    DateTime startDateTime = DateTime.ParseExact($"{selectedStartDate.Value:yyyy-MM-dd} {startTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    DateTime endDateTime = DateTime.ParseExact($"{selectedEndDate.Value:yyyy-MM-dd} {endTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                    // 선택된 날짜 범위를 이용해 RawData 창을 엽니다.
                    RawData rawDataWindow = new RawData(startDateTime, endDateTime);
                    rawDataWindow.Show();
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"날짜 및 시간 형식 오류: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("유효한 시작 날짜와 종료 날짜를 선택하세요.");
            }
        }

        private void UpdateChartsAndGraphs()
        {
            DateTime? selectedStartDate = DateRangePickerControl.StartDate;
            DateTime? selectedEndDate = DateRangePickerControl.EndDate;

            string startTime = txtStartTime.Text;
            string endTime = txtEndTime.Text;

            if (selectedStartDate.HasValue && selectedEndDate.HasValue)
            {
                try
                {
                    DateTime startDateTime = DateTime.ParseExact($"{selectedStartDate.Value:yyyy-MM-dd} {startTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    DateTime endDateTime = DateTime.ParseExact($"{selectedEndDate.Value:yyyy-MM-dd} {endTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                    // 필터링된 데이터를 가져옵니다.
                    _filteredData = DataList.datas
                        .Where(data => data.date >= startDateTime && data.date <= endDateTime)
                        .ToList();

                    if (_filteredData.Any())
                    {
                        // 통계값을 계산합니다.
                        DataTable statisticsTable = CalculateStatistics(_filteredData);

                        // 통계 데이터를 막대 그래프로 표시합니다.
                        string[] categories = new[] { "MeltTemperature", "MotorSpeed", "MeltWeight" };
                        statisticsChartControl.DisplayStatisticsAndChart("Statistics", statisticsTable, categories);

                        // opChart에 데이터를 표시합니다.
                        opChartControl.LoadDataWithInterval(_filteredData, 90); // 90분 간격으로 데이터를 표시

                        // OK와 NG 개수 계산
                        int okCount = _filteredData.Count(data => data.ok_ng == "OK");
                        int ngCount = _filteredData.Count(data => data.ok_ng == "NG");

                        // OK와 NG 비율 계산
                        double total = okCount + ngCount;
                        double okPercentage = total > 0 ? (okCount / total) * 100 : 0;
                        double ngPercentage = total > 0 ? (ngCount / total) * 100 : 0;

                        // NgPieChart에 업데이트
                        ngPieChartControl.UpdateChart(okPercentage, ngPercentage, okCount, ngCount);
                    }
                    else
                    {
                        MessageBox.Show("선택한 날짜 및 시간 범위에 해당하는 데이터가 없습니다.");
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show($"날짜 및 시간 형식 오류: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("유효한 시작 날짜와 종료 날짜를 선택하세요.");
            }
        }

        private DataTable CalculateStatistics(List<DataValues> filteredData)
        {
            var statisticsTable = new DataTable();
            statisticsTable.Columns.Add("Metric");
            statisticsTable.Columns.Add("MeltTemperature");
            statisticsTable.Columns.Add("MotorSpeed");
            statisticsTable.Columns.Add("MeltWeight");

            var temperatureData = filteredData.Select(d => (double)d.melt_temperature).ToList();
            var speedData = filteredData.Select(d => (double)d.motor_speed).ToList();
            var weightData = filteredData.Select(d => (double)d.melt_weight).ToList();

            statisticsTable.Rows.Add(CreateStatisticsRow(statisticsTable, "Mean", temperatureData, speedData, weightData));
            statisticsTable.Rows.Add(CreateStatisticsRow(statisticsTable, "Median", temperatureData, speedData, weightData, calculateMedian: true));
            statisticsTable.Rows.Add(CreateStatisticsRow(statisticsTable, "Std Dev", temperatureData, speedData, weightData, calculateStdDev: true));
            statisticsTable.Rows.Add(CreateStatisticsRow(statisticsTable, "Max", temperatureData, speedData, weightData, calculateMax: true));
            statisticsTable.Rows.Add(CreateStatisticsRow(statisticsTable, "Min", temperatureData, speedData, weightData, calculateMin: true));

            return statisticsTable;
        }

        private DataRow CreateStatisticsRow(DataTable statisticsTable, string metric, List<double> temperatureData, List<double> speedData, List<double> weightData,
                                            bool calculateMedian = false, bool calculateStdDev = false, bool calculateMax = false, bool calculateMin = false)
        {
            DataRow row = statisticsTable.NewRow();
            row["Metric"] = metric;

            row["MeltTemperature"] = calculateMedian ? CalculateMedian(temperatureData) :
                                    calculateStdDev ? CalculateStandardDeviation(temperatureData) :
                                    calculateMax ? temperatureData.Max() :
                                    calculateMin ? temperatureData.Min() :
                                    temperatureData.Average();

            row["MotorSpeed"] = calculateMedian ? CalculateMedian(speedData) :
                                calculateStdDev ? CalculateStandardDeviation(speedData) :
                                calculateMax ? speedData.Max() :
                                calculateMin ? speedData.Min() :
                                speedData.Average();

            row["MeltWeight"] = calculateMedian ? CalculateMedian(weightData) :
                                calculateStdDev ? CalculateStandardDeviation(weightData) :
                                calculateMax ? weightData.Max() :
                                calculateMin ? weightData.Min() :
                                weightData.Average();

            return row;
        }

        private double CalculateMedian(List<double> values)
        {
            var sortedValues = values.OrderBy(v => v).ToArray();
            int count = sortedValues.Length;
            return count % 2 == 0 ? (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2 : sortedValues[count / 2];
        }

        private double CalculateStandardDeviation(List<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }

}
