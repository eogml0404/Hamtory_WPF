using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hamtory_WPF
{
    public class ProcessData
    {
        public List<DataValues> LoadDataFile(string filePath)
        {
            List<DataValues> dataList = new List<DataValues>();

            using (var data = new StreamReader(filePath))
            {
                bool firstLine = true;

                while (!data.EndOfStream)
                {
                    var row = data.ReadLine();

                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    var columns = row.Split(',');


                    dataList.Add(new DataValues(DateTime.Parse(columns[0]), int.Parse(columns[1]), int.Parse(columns[2]), int.Parse(columns[3]), int.Parse(columns[4]) , double.Parse(columns[5]), columns[6]));
                    
                }
            }

            return dataList;
        }
    }
}