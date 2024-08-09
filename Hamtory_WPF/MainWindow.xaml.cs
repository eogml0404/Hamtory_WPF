using System;
using System.Windows;

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        private const string USER_FILE = "users.txt";
        private LoginManager loginManager;

        public MainWindow()
        {
            InitializeComponent();
            loginManager = new LoginManager();
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
            frame.Content = new Page2();
        }

        /*
        private void home_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page3();
        }
        */
    }
}
