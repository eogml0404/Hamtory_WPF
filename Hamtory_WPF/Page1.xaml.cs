using DocumentFormat.OpenXml.Drawing;
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
    /// Page1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page1 : Page
    {
        public bool IsReading { get; set; }
        private MainWindowViewModel viewModel;
        ProcessData dataLoader = new ProcessData();
        

        public Page1()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel(new DateTime(2020,3,4));

            DataContext = viewModel;
        }

        private void stopGraph(object sender, MouseEventArgs e)
        {
             viewModel.StopTimer();
        }

        private void startGraph(object sender, MouseEventArgs e)
        {
            viewModel.StartTimer();
        }

        private void RealdatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            List<DataValues> datas = dataLoader.LoadDataFile("melting_tank.csv");

            List<DateTime> date_datas = new List<DateTime>();
          
            // 데이터 값 채우기
            foreach (DataValues data in datas)
            {
                date_datas.Add(data.date);
            }

            if (date_datas.Contains(RealdatePicker.SelectedDate.Value))
            {
                if (RealdatePicker.SelectedDate.HasValue)
                {
                    viewModel = new MainWindowViewModel(RealdatePicker.SelectedDate.Value);
                    DataContext = viewModel;
                }
            }
            else
            {
                MessageBox.Show("해당 날짜 데이터값이 없습니다.");
            }
                
        }
    }

}