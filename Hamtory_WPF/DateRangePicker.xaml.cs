using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hamtory_WPF
{
    public partial class DateRangePicker : UserControl
    {
        // Dependency properties for StartDate and EndDate
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime?), typeof(DateRangePicker), new PropertyMetadata(null, OnDateChanged));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime?), typeof(DateRangePicker), new PropertyMetadata(null, OnDateChanged));

        // Event to notify when the date range changes
        public event EventHandler<DateRangeChangedEventArgs> DateRangeChanged;

        private bool isSelectingStartDate;

        // CLR properties for StartDate and EndDate
        public DateTime? StartDate
        {
            get { return (DateTime?)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public DateTime? EndDate
        {
            get { return (DateTime?)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        public DateRangePicker()
        {
            InitializeComponent();
            isSelectingStartDate = true;
        }

        private void DatePickerControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerControl.SelectedDate.HasValue)
            {
                if (isSelectingStartDate)
                {
                    StartDate = DatePickerControl.SelectedDate.Value;
                    isSelectingStartDate = false;
                    DatePickerControl.IsDropDownOpen = true;  // Keep the calendar open to select the end date
                }
                else
                {
                    EndDate = DatePickerControl.SelectedDate.Value;

                    if (EndDate < StartDate)
                    {
                        // If the EndDate is before StartDate, swap them
                        var temp = StartDate;
                        StartDate = EndDate;
                        EndDate = temp;
                    }

                    // Notify that the date range has changed
                    OnDateRangeChanged();

                    // Reset to allow new selection
                    isSelectingStartDate = true;
                }
            }
        }


        private void DatePickerControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Only reset dates if both StartDate and EndDate are set
            if (StartDate.HasValue && EndDate.HasValue)
            {
                StartDate = null;
                EndDate = null;
                DatePickerControl.SelectedDate = null;
                DatePickerControl.DisplayDateStart = null;
                DatePickerControl.DisplayDateEnd = null;
                isSelectingStartDate = true;
                OnDateRangeChanged();
            }
        }

        private void OnDateRangeChanged()
        {
            DateRangeChanged?.Invoke(this, new DateRangeChangedEventArgs(StartDate, EndDate));
        }

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DateRangePicker;
            control?.OnDateRangeChanged();
        }

        // Method to filter data based on the date range and time
        public async Task<DataTable> FilterDataAsync(DataTable dataTable, string startTime, string endTime)
        {
            try
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    DateTime startDate = StartDate.Value;
                    DateTime endDate = EndDate.Value;

                    if (DateTime.TryParseExact(startDate.ToString("yyyy-MM-dd") + " " + startTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDateTime) &&
                        DateTime.TryParseExact(endDate.ToString("yyyy-MM-dd") + " " + endTime, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDateTime))
                    {
                        return await Task.Run(() =>
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
                    }
                    else
                    {
                        MessageBox.Show("유효한 시간을 HH:mm 형식으로 입력하세요.");
                        return null;
                    }
                }
                else
                {
                    MessageBox.Show("유효한 날짜를 선택하세요.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 필터링 중 오류 발생: {ex.Message}");
                return null;
            }
        }
    }

    // EventArgs class for the DateRangeChanged event
    public class DateRangeChangedEventArgs : EventArgs
    {
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }

        public DateRangeChangedEventArgs(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
