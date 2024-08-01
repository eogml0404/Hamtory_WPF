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

namespace Hamtory_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadDataFromFile("melting_tank.csv");
        }

        private void LoadDataFromFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    // 첫 번째 줄에서 컬럼 이름을 읽어옴
                    var header = reader.ReadLine();
                    if (header != null)
                    {
                        var columns = header.Split(',');
                        foreach (var column in columns)
                        {
                            dataTable.Columns.Add(column);
                        }

                        // 데이터 행 추가
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (line != null)
                            {
                                var values = line.Split(',');
                                dataTable.Rows.Add(values);
                            }
                        }
                    }
                }

                // 필터링된 데이터만 새로운 DataTable로 변환
                var filteredRows = dataTable.AsEnumerable()
                    .Where(row => row.Field<string>("STD_DT").StartsWith("2020-03-04"));

                if (filteredRows.Any())
                {
                    var filteredTable = filteredRows.CopyToDataTable();
                    dataGrid.ItemsSource = filteredTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("No matching data found.");
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}");
            }
        }
    }
}