using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        private const string USER_FILE = "users.txt";

        public MainWindow()
        {
            InitializeComponent();
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            tabControl.Visibility = Visibility.Visible;
            frame.Visibility = Visibility.Hidden;
            real_time_button.Visibility = Visibility.Hidden;
            data_button.Visibility = Visibility.Hidden;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var users = File.ReadAllLines(USER_FILE)
                            .Select(line => line.Split(','))
                            .ToDictionary(parts => parts[0], parts => parts[1]);

            if (users.TryGetValue(LoginIDTextBox.Text, out var password) && password == LoginPasswordBox.Password)
            {
                tabControl.Visibility = Visibility.Hidden;
                frame.Visibility = Visibility.Visible;
                real_time_button.Visibility = Visibility.Visible;
                data_button.Visibility = Visibility.Visible;
                frame.Content = new Page1(); // Navigate to default page after login
            }
            else
            {
                MessageBox.Show("ID 또는 비밀번호가 잘못되었습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void real_time_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page1();
        }

        private void data_button_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Page2();
        }
    }
}
