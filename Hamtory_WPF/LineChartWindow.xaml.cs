using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace Hamtory_WPF
{
    public partial class LineChartWindow : Window
    {
        private DataTable originalDataTable;
        private int interval;

        public LineChartWindow(DataTable dataTable, int interval = 0)
        {
            InitializeComponent();
            originalDataTable = dataTable;
            this.interval = interval;
            LoadLineChart(dataTable); // 초기 데이터 로드
        }

        private void LoadLineChart(DataTable dataTable)
        {
            Dispatcher.Invoke(() =>
            {
                lineChart.Series.Clear();

                var meltTempValues = new ChartValues<double>();
                var motorSpeedValues = new ChartValues<double>();
                var meltWeightValues = new ChartValues<double>();
                var inspValues = new ChartValues<double>();

                foreach (DataRow row in dataTable.Rows)
                {
                    if (double.TryParse(row["MELT_TEMP"].ToString(), out double meltTemp) &&
                        double.TryParse(row["MOTORSPEED"].ToString(), out double motorSpeed) &&
                        double.TryParse(row["MELT_WEIGHT"].ToString(), out double meltWeight) &&
                        double.TryParse(row["INSP"].ToString(), out double insp))
                    {
                        meltTempValues.Add(meltTemp);
                        motorSpeedValues.Add(motorSpeed);
                        meltWeightValues.Add(meltWeight);
                        inspValues.Add(insp);
                    }
                }

                lineChart.Series.Add(new LineSeries
                {
                    Title = "Melt Temp",
                    Values = meltTempValues,
                    Stroke = new SolidColorBrush(Colors.Blue),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("F2")
                });

                lineChart.Series.Add(new LineSeries
                {
                    Title = "Motor Speed",
                    Values = motorSpeedValues,
                    Stroke = new SolidColorBrush(Colors.Red),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("F2")
                });

                lineChart.Series.Add(new LineSeries
                {
                    Title = "Melt Weight",
                    Values = meltWeightValues,
                    Stroke = new SolidColorBrush(Colors.Green),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("F2")
                });

                lineChart.Series.Add(new LineSeries
                {
                    Title = "INSP",
                    Values = inspValues,
                    Stroke = new SolidColorBrush(Colors.Purple),
                    Fill = Brushes.Transparent,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("F2")
                });

                lineChart.AxisX.Clear();
                lineChart.AxisX.Add(new Axis
                {
                    Title = "Time",
                    Labels = dataTable.AsEnumerable().Select(row => row["STD_DT"].ToString()).ToArray(),
                    Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = false }
                });

                lineChart.AxisY.Clear();
                lineChart.AxisY.Add(new Axis
                {
                    Title = "Values",
                    LabelFormatter = value => value.ToString("F2")
                });
            });
        }

        private void BtnUpdateChart_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtInterval.Text, out int interval))
            {
                var filteredData = GetIntervalData(originalDataTable, interval);
                LoadLineChart(filteredData);
            }
            else
            {
                MessageBox.Show("유효한 분 단위를 입력하세요.");
            }
        }

        private DataTable GetIntervalData(DataTable dataTable, int intervalMinutes)
        {
            var resultTable = dataTable.Clone();
            DateTime lastAddedTime = DateTime.MinValue;

            foreach (DataRow row in dataTable.Rows)
            {
                if (DateTime.TryParse(row["STD_DT"].ToString(), out DateTime rowDateTime))
                {
                    if (lastAddedTime == DateTime.MinValue || (rowDateTime - lastAddedTime).TotalMinutes >= intervalMinutes)
                    {
                        resultTable.ImportRow(row);
                        lastAddedTime = rowDateTime;
                    }
                }
            }

            return resultTable;
        }
    }
}
