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
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NGstackChart : UserControl
    {

        public NGstackChart()
        {
            InitializeComponent();

            List<DataValues> datas = DataList.datas;
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = new ChartValues<int> {OKCounting(4, datas),OKCounting(3, datas),OKCounting(2, datas),OKCounting(1, datas) },
                    StackMode = StackMode.Percentage,
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromArgb(250, 100, 90, 190)),
                    Title = "OK"
                },

                new StackedColumnSeries
                {
                    Values = new ChartValues<int> {NGCounting(4, datas),NGCounting(3, datas),NGCounting(2, datas),NGCounting(1, datas) },
                    StackMode = StackMode.Percentage,
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromArgb(250, 190, 50, 50)),
                    Title = "NG"
                }
            };

            Labels = new[] {"-4h", "-3h", "-2h" , "-1h"};
            // Formatter = value => value + " 번";

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public int OKCounting(int time , List<DataValues> datas)
        {
            int OkCnt = 0;

            foreach (DataValues data in datas)
            {

                if (data.date.Year == 2020 &&
                    data.date.Month == 4 &&
                    data.date.Day == DateTime.Now.Day &&
                    data.date.Hour == DateTime.Now.Hour - time)
                {
                    if (data.ok_ng == "OK")
                    {
                        OkCnt++;
                    }
                }

            }
            return OkCnt;
        }

        public int NGCounting(int time , List<DataValues> datas)
        {
            int NGCnt = 0;

            foreach (DataValues data in datas)
            {

                if (data.date.Year == 2020 &&
                    data.date.Month == 4 &&
                    data.date.Day == DateTime.Now.Day &&
                    data.date.Hour == DateTime.Now.Hour - time)
                {
                    if (data.ok_ng == "NG")
                    {
                        NGCnt++;
                    }

                }

            }
            return NGCnt;
        }
    }
}
