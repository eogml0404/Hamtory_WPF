using System;
using System.Windows;

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        private LoginManager loginManager;
        private Page2 page2;

        public MainWindow()
        {
            InitializeComponent();
            loginManager = new LoginManager();
            page2 = new Page2();
            page2.LoadData(); // 파일 로드
            ShowLoginPage(); // 로그인 페이지 표시
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
        }

        private void data_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = page2;
        }

        private void home_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page3();
        }
    }
}
