using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows.Media;

namespace Hamtory_WPF
{
    public partial class MainWindow3 : Window
    {
        public MainWindow3()
        {
            DataContext = new MainWindowViewModel();

        }

    }
}