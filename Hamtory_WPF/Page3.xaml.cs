using DocumentFormat.OpenXml.Wordprocessing;
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
    /// Page3.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page3 : Page
    {
        private MainWindow mainWindow;

        public Page3(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void monitoringButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.frame.Content = new Page1();
            mainWindow.MainLabel.Content = "Monitoring";
        }

        private void analyticsButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.frame.Content = new Page2();
            mainWindow.MainLabel.Content = "Data Analytics";
        }
    }
}
