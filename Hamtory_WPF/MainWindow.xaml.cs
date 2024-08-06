using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //LoadDataFromFile("melting_tank.csv");

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