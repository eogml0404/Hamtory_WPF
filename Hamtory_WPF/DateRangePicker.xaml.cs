using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hamtory_WPF
{
    public partial class DateRangePicker : UserControl
    {
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime?), typeof(DateRangePicker), new PropertyMetadata(null, OnDateChanged));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime?), typeof(DateRangePicker), new PropertyMetadata(null, OnDateChanged));

        public event EventHandler<DateRangeChangedEventArgs> DateRangeChanged;
        public event EventHandler<DateRangeChangedEventArgs> EndDateChanged;

        private bool isSelectingStartDate;

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
                    DatePickerControl.DisplayDateStart = StartDate;
                    isSelectingStartDate = false;
                    DatePickerControl.IsDropDownOpen = true;
                }
                else
                {
                    EndDate = DatePickerControl.SelectedDate.Value;
                    if (EndDate < StartDate)
                    {
                        var temp = StartDate;
                        StartDate = EndDate;
                        EndDate = temp;
                    }
                    DatePickerControl.DisplayDateEnd = EndDate;
                    isSelectingStartDate = true;
                    OnDateRangeChanged();
                    OnEndDateChanged();
                }
            }
        }

        private void DatePickerControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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

        private void OnEndDateChanged()
        {
            EndDateChanged?.Invoke(this, new DateRangeChangedEventArgs(StartDate, EndDate));
        }

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DateRangePicker;
            control?.OnDateRangeChanged();
        }
    }

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
