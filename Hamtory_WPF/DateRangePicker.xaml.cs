using System;
using System.Collections.Generic;
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
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime?), typeof(DateRangePicker), new PropertyMetadata(null, OnDateChanged));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime?), typeof(DateRangePicker), new PropertyMetadata(null, OnDateChanged));

        public event EventHandler<DateRangeChangedEventArgs> DateRangeChanged;

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
                    DatePickerControl.IsDropDownOpen = true;  // Keep the calendar open
                }
                else
                {
                    EndDate = DatePickerControl.SelectedDate.Value;
                    if (EndDate < StartDate)
                    {
                        // Swap if EndDate is before StartDate
                        var temp = StartDate;
                        StartDate = EndDate;
                        EndDate = temp;
                    }
                    DatePickerControl.DisplayDateEnd = EndDate;
                    isSelectingStartDate = true;
                }
                OnDateRangeChanged();
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
