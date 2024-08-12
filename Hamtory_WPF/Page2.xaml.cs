<<<<<<< HEAD
﻿using System.Windows.Controls;

=======
﻿using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
>>>>>>> 7578c49a875a400b46ce4f29c79532d8e776ad17

namespace Hamtory_WPF
{
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
<<<<<<< HEAD
=======
            DateRangePickerControl.DateRangeChanged += DateRangePickerControl_DateRangeChanged;
            opChart.LoadSampleData();
        }

        private void DateRangePickerControl_DateRangeChanged(object sender, DateRangeChangedEventArgs e)
        {
            StartDateText.Text = e.StartDate.HasValue ? e.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            EndDateText.Text = e.EndDate.HasValue ? e.EndDate.Value.ToString("yyyy-MM-dd") : "End Date";
        }

        private void RawDataButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = DateRangePickerControl.StartDate;
            DateTime? endDate = DateRangePickerControl.EndDate;
            string startTime = txtStartTime.Text;
            string endTime = txtEndTime.Text;

            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime startDateTime = DateTime.ParseExact($"{startDate.Value:yyyy-MM-dd} {startTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                DateTime endDateTime = DateTime.ParseExact($"{endDate.Value:yyyy-MM-dd} {endTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                RawData rawDataWindow = new RawData(startDateTime, endDateTime);
                rawDataWindow.Show();
            }
            else
            {
                MessageBox.Show("유효한 날짜를 선택하세요.");
            }
>>>>>>> 7578c49a875a400b46ce4f29c79532d8e776ad17
        }
    }
}
