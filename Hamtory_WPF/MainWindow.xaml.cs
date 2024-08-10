using OpenTK.Windowing.Desktop;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        private const string USER_FILE = "users.txt";
        private LoginManager loginManager;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            loginManager = new LoginManager();
            ShowLoginPage(); // 로그인 페이지 표시

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
            ToDay.Content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }


        private void ShowLoginPage()
        {
            loginManager.ShowLoginPage(this);
        }
        

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            loginManager.Login(this, LoginIDTextBox.Text, LoginPasswordBox.Password);
        }

        private void real_time_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page1();
            MainLabel.Content = "Monitoring";
        }

        private void data_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page2();
            MainLabel.Content = "Data Analytics";
        }

        private void home_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page3();
            MainLabel.Content = "Home";
        }
    }
}
