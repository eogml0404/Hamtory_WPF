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