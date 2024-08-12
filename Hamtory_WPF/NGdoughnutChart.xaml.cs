using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;
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
    /// <summary>
    /// NGdoughnutChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NGdoughnutChart : UserControl
    {
        public NGdoughnutChart()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Chrome",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromArgb(250, 100, 90, 190)),
                },
                new PieSeries
                {
                    Title = "Mozilla",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromArgb(250, 190, 50, 50)),
                },
            };

            //adding values or series will update and animate the chart automatically
            //SeriesCollection.Add(new PieSeries());
            //SeriesCollection[0].Values.Add(5);

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }

    }
}
