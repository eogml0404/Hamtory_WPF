using LiveCharts.Wpf;
using LiveCharts;
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
    public partial class NGstackChart : UserControl
    {
        private DispatcherTimer timer;
        private int currentIndex;
        private List<DataValues> datas;

        public class OK_NG
        {
            public int okCnt;
            public int ngCnt;
        }

        public NGstackChart()
        {
            InitializeComponent();

            // 데이터 리스트 초기화
            datas = DataList.datas;

            // 현재 시점의 인덱스 계산
            currentIndex = GetCurrentIndex(datas);

            // OK_NG 리스트 생성
            List<OK_NG> okNgList = GetOKNGList(datas, currentIndex);

            // SeriesCollection 설정 (초기화 한 번만 수행)
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = new ChartValues<int> { okNgList[0].okCnt, okNgList[1].okCnt, okNgList[2].okCnt, okNgList[3].okCnt, okNgList[4].okCnt, okNgList[5].okCnt },
                    StackMode = StackMode.Percentage,
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromArgb(250, 100, 90, 190)),
                    Title = "OK"
                },

                new StackedColumnSeries
                {
                    Values = new ChartValues<int> { okNgList[0].ngCnt, okNgList[1].ngCnt, okNgList[2].ngCnt, okNgList[3].ngCnt, okNgList[4].ngCnt, okNgList[5].ngCnt },
                    StackMode = StackMode.Percentage,
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromArgb(250, 190, 50, 50)),
                    Title = "NG"
                }
            };

            // 레이블 설정
            Labels = new[] { "-6h", "-5h", "-4h", "-3h", "-2h", "-1h" };

            // 데이터 컨텍스트 설정
            DataContext = this;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 새로운 데이터를 가져옴
            List<OK_NG> okNgList = GetOKNGList(datas, currentIndex);

            // 기존 bar의 값
            var okSeries = SeriesCollection[0] as StackedColumnSeries;
            var ngSeries = SeriesCollection[1] as StackedColumnSeries;

            if (okSeries != null && ngSeries != null)
            {
                // 각 시리즈의 값 업데이트 (막대 그래프의 값만 변경)
                for (int i = 0; i < 6; i++)
                {
                    okSeries.Values[i] = okNgList[i].okCnt;
                    ngSeries.Values[i] = okNgList[i].ngCnt;
                }
            }

            // currentIndex를 증가시켜 다음 시간 데이터를 가리키도록 설정
            currentIndex++;
        }

        // 현재 시점의 인덱스를 계산하는 메서드
        private int GetCurrentIndex(List<DataValues> datas)
        {
            DateTime now = DateTime.Now;

            for (int i = datas.Count - 1; i >= 0; i--)
            {
                if (datas[i].date.Year == 2020 &&
                    datas[i].date.Month == 4 &&
                    datas[i].date.Day == now.Day &&
                    datas[i].date.Hour == now.Hour &&
                    datas[i].date.Minute == now.Minute)
                {
                    return i;
                }
            }

            return 36000; // 해당 시간대의 데이터를 찾지 못했을 때
        }

        // 특정 범위의 OK 또는 NG 개수를 세는 메서드
        public int CountValuesInRange(string okNgType, int time, int currentIndex, List<DataValues> datas)
        {
            int count = 0;

            // 시간에 따라 타겟 인덱스 계산
            int targetStartIndex = currentIndex - (time * 600);   // 예: time 시간 전 데이터의 시작 인덱스
            int targetEndIndex = targetStartIndex + 600;          // 예: time 시간 전 데이터의 끝 인덱스

            // 인덱스 범위가 음수인 경우 처리
            if (targetStartIndex < 0)
            {
                targetStartIndex = 0;
            }
            if (targetEndIndex < 0)
            {
                targetEndIndex = 0;
            }

            // 인덱스 범위 내에서 개수 계산
            for (int i = targetStartIndex; i < targetEndIndex; i++)
            {
                if (i >= 0 && i < datas.Count)
                {
                    if (datas[i].ok_ng == okNgType)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        // OK_NG 리스트를 생성하는 메서드
        public List<OK_NG> GetOKNGList(List<DataValues> datas, int currentIndex)
        {
            List<OK_NG> okNgList = new List<OK_NG>();

            for (int i = 6; i >= 1; i--)
            {
                OK_NG okNg = new OK_NG
                {
                    okCnt = CountValuesInRange("OK", i, currentIndex, datas),
                    ngCnt = CountValuesInRange("NG", i, currentIndex, datas)
                };
                okNgList.Add(okNg);
            }

            return okNgList;
        }
        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }
        // SeriesCollection, Labels, Formatter 프로퍼티
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
    }
}
