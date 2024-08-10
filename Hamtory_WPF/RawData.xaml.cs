using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Data;
using System.IO;
using CsvHelper;
using System.Globalization;
using ClosedXML.Excel;

namespace Hamtory_WPF
{
    public partial class RawData : Window
    {
        public RawData()
        {
            InitializeComponent();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.ItemsSource == null)
            {
                MessageBox.Show("저장할 데이터가 없습니다.");
                return;
            }

            var dataView = dataGrid.ItemsSource as DataView;
            var dataTable = dataView?.ToTable();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV 파일 (*.csv)|*.csv|Excel 파일 (*.xlsx)|*.xlsx",
                Title = "데이터 저장"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath);

                try
                {
                    if (fileExtension == ".csv")
                    {
                        using (var writer = new StreamWriter(filePath))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(dataTable.AsEnumerable());
                        }
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            workbook.Worksheets.Add(dataTable, "Data");
                            workbook.SaveAs(filePath);
                        }
                    }

                    MessageBox.Show("데이터가 성공적으로 저장되었습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"데이터 저장 중 오류 발생: {ex.Message}");
                }
            }
        }
    }
}
