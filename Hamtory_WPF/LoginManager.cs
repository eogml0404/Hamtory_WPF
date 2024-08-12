using Hamtory_WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

public class LoginManager
{
    private readonly string USER_FILE = "users.txt"; // 사용자 파일 경로

    public void ShowLoginPage(MainWindow mainWindow)
    {
        mainWindow.Login.Visibility = Visibility.Visible;
        mainWindow.MainLabel.Visibility = Visibility.Hidden;
        mainWindow.real_time_button.Visibility = Visibility.Hidden;
        mainWindow.data_button.Visibility = Visibility.Hidden;
        mainWindow.home_button.Visibility = Visibility.Hidden;
        mainWindow.ToDay.Visibility = Visibility.Hidden;
    }

    public void Login(MainWindow mainWindow, string loginId, string password)
    {
        var users = File.ReadAllLines(USER_FILE)
                        .Select(line => line.Split(','))
                        .ToDictionary(parts => parts[0], parts => parts[1]);

        if (users.TryGetValue(loginId, out var storedPassword) && storedPassword == password)
        {
            mainWindow.Login.Visibility = Visibility.Hidden;
            mainWindow.MainLabel.Visibility = Visibility.Visible;
            mainWindow.real_time_button.Visibility = Visibility.Visible;
            mainWindow.data_button.Visibility = Visibility.Visible;
            mainWindow.home_button.Visibility = Visibility.Visible;
            mainWindow.ToDay.Visibility = Visibility.Visible;
            AnimationHelper.NavigateWithFade(mainWindow.frame, new Page3(mainWindow)); // 로그인 성공 시 Page3으로 이동
        }
        else
        {
            MessageBox.Show("ID 또는 비밀번호가 잘못되었습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
