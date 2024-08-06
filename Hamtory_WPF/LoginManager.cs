using Hamtory_WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

public class LoginManager
{
    private readonly string USER_FILE = "users.txt"; // 사용자 파일 경로

    public void ShowLoginPage(MainWindow mainWindow)
    {
        mainWindow.tabControl.Visibility = Visibility.Visible;
        mainWindow.frame.Visibility = Visibility.Hidden;
        mainWindow.real_time_button.Visibility = Visibility.Hidden;
        mainWindow.data_button.Visibility = Visibility.Hidden;
    }

    public void Login(MainWindow mainWindow, string loginId, string password)
    {
        var users = File.ReadAllLines(USER_FILE) // 파일에서 모든 줄을 읽음
                        .Select(line => line.Split(',')) // 각 줄을 쉼표로 나눔
                        .ToDictionary(parts => parts[0], parts => parts[1]); // 사전으로 변환

        if (users.TryGetValue(loginId, out var storedPassword) && storedPassword == password)
        {
            mainWindow.tabControl.Visibility = Visibility.Hidden;
            mainWindow.frame.Visibility = Visibility.Visible;
            mainWindow.real_time_button.Visibility = Visibility.Visible;
            mainWindow.data_button.Visibility = Visibility.Visible;
            mainWindow.frame.Content = new Page1(); // 로그인 후 기본 페이지로 이동
        }
        else
        {
            MessageBox.Show("ID 또는 비밀번호가 잘못되었습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
