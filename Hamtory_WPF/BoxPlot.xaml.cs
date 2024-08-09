using ScottPlot;
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
    /// BoxPlot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BoxPlot : UserControl
    {
        public BoxPlot()
        {
            InitializeComponent();

            // Create a new ScottPlot Plot
            Plot myPlot = WpfPlot1.Plot;

            Box box1 = new()
            {
                Position = 5,
                BoxMin = 81,
                BoxMax = 93,
                WhiskerMin = 76,
                WhiskerMax = 107,
                BoxMiddle = 84,
            };

            Box box2 = new()
            {
                Position = 3,
                BoxMin = 81,
                BoxMax = 93,
                WhiskerMin = 76,
                WhiskerMax = 107,
                BoxMiddle = 84,
            };

            Box box3 = new()
            {
                Position = 7,
                BoxMin = 81,
                BoxMax = 93,
                WhiskerMin = 76,
                WhiskerMax = 107,
                BoxMiddle = 84,
            };
            myPlot.Add.Box(box1);
            myPlot.Add.Box(box2);
            myPlot.Add.Box(box3);
            myPlot.Axes.SetLimits(0, 10, 70, 110);

        }

    }
}
