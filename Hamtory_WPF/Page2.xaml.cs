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
using System.Collections.Generic;

namespace Hamtory_WPF
{
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
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
            RawData rawDataWindow = new RawData();
            rawDataWindow.Show();
        }

    }
}
