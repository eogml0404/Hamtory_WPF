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
        public async Task<List<DataValues>> LoadDataFileAsync(string filePath)
        {
            List<DataValues> dataList = new List<DataValues>();

            // StreamReader를 비동기로 사용
            using (var data = new StreamReader(filePath))
            {
                bool firstLine = true;

                while (!data.EndOfStream)
                {
                    var row = await data.ReadLineAsync(); // 비동기적으로 한 줄 읽기

                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    var columns = row.Split(',');

                    // 데이터 파싱 후 리스트에 추가
                    dataList.Add(new DataValues(
                        DateTime.Parse(columns[0]),
                        int.Parse(columns[1]),
                        int.Parse(columns[2]),
                        int.Parse(columns[3]),
                        int.Parse(columns[4]),
                        double.Parse(columns[5]),
                        columns[6]));
                }
            }

            return dataList;
        }
    }
}