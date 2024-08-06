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
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
            DateRangePickerControl.DateRangeChanged += DateRangePickerControl_DateRangeChanged;
        }

        private void DateRangePickerControl_DateRangeChanged(object sender, DateRangeChangedEventArgs e)
        {
            if (e.StartDate.HasValue)
            {
                StartDateText.Text = $"{e.StartDate.Value.ToShortDateString()}";
            }
            else
            {
                StartDateText.Text = "N/A";
            }

            if (e.EndDate.HasValue)
            {
                EndDateText.Text = $"{e.EndDate.Value.ToShortDateString()}";
            }
            else
            {
                EndDateText.Text = "N/A";
            }
        }
    }
}
