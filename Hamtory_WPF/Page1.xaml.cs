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
using System.Windows.Threading;

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
        private DispatcherTimer timer;

        public Page1()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel(new DateTime(2020, 4, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));

            DataContext = viewModel;
            
            ToDay.Content = "ToDay = " + new DateTime(2020, 4, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            UpdateDateTimeDisplay();

            // DispatcherTimer 설정
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 1초 간격으로 타이머 설정
            };
            timer.Tick += Timer_Tick; // Tick 이벤트 핸들러 설정
            timer.Start(); // 타이머 시작
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // ToDay의 Content를 현재 시간으로 업데이트
            UpdateDateTimeDisplay();
        }

        private void UpdateDateTimeDisplay()
        {
            ToDay.Content = "ToDay : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private void stopGraph(object sender, MouseEventArgs e)
        {
             viewModel.StopTimer();
        }

        private void startGraph(object sender, MouseEventArgs e)
        {
            viewModel.StartTimer();
        }

    }

}