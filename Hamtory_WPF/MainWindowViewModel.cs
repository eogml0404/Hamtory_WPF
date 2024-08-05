using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using ScottPlot.Plottables;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Hamtory_WPF
{
    // MainWindow의 ViewModel
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        ProcessData dataLoader = new ProcessData();
        private List<DateTime> filteredDates;
        private List<int> filteredTemperatures;
        private List<int> filteredIndex;
        private int currentDataIndex = 0;

        public MainWindowViewModel()
        {
            List<DataValues> datas = dataLoader.LoadDataFile("melting_tank.csv");

            DateTime targetDate = new DateTime(2020, 3, 4);

            List<DateTime> date_datas = new List<DateTime>();
            List<int> melt_temperature_datas = new List<int>();
            List<int> index = new List<int>();

            // 데이터 값 채우기
            foreach (DataValues data in datas)
            {
                date_datas.Add(data.date);
                melt_temperature_datas.Add(data.melt_temperature);
                index.Add(data.index);
            }

            // 필터링된 데이터 생성
            filteredDates = new List<DateTime>();
            filteredTemperatures = new List<int>();
            filteredIndex = new List<int>();

            for (int i = 0; i < date_datas.Count; i++)
            {
                if (date_datas[i].Date == targetDate.Date)
                {
                    filteredDates.Add(date_datas[i]);
                    filteredTemperatures.Add(melt_temperature_datas[i]);
                    filteredIndex.Add(index[i]);
                }
            }

            // MeasureModel을 전역적으로 설정
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.Index) // Index를 X 값으로 사용
                .Y(model => model.Value); // Value 속성을 Y 값으로 사용

            // MeasureModel에 대한 매퍼를 전역적으로 설정
            Charting.For<MeasureModel>(mapper);

            // ChartValues 초기화
            ChartValues = new ChartValues<MeasureModel>();

            // SeriesCollection을 설정
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "용해온도",
                    Values = ChartValues,           // ChartValues를 LineSeries의 값으로 설정
                    PointGeometrySize = 18,         // 데이터 점의 크기
                    StrokeThickness = 4             // 선의 두께
                }
            };

            // AxisX 설정
            AxisXCollection = new AxesCollection
            {
                new Axis
                {
                    DisableAnimations = true, // 애니메이션 비활성화
                    LabelFormatter = value =>
                    {
                        int index = (int)value;
                        if (index >= 0 && index < filteredDates.Count)
                        {
                            return filteredDates[index].ToString("mm:ss");
                        }
                        return "";
                    }, // X축 레이블 포맷
                    Separator = new Separator
                    {
                        Step = 1 // X축 구분선 간격 설정
                    }
                }
            };

            // 초기 축 한계 설정
            SetAxisLimits(0);

            // 데이터를 500ms마다 업데이트하는 타이머 설정
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // 1000ms 간격으로 타이머 설정
            };
            Timer.Tick += TimerOnTick; // 타이머의 Tick 이벤트에 처리기 추가
            Timer.Start(); // 타이머 시작
        }

        // 차트에 표시할 데이터 컬렉션
        public SeriesCollection SeriesCollection { get; set; }
        public AxesCollection AxisXCollection { get; set; }
        public ChartValues<MeasureModel> ChartValues { get; set; }
        public DispatcherTimer Timer { get; set; }

        // 축 한계 설정 메서드
        private void SetAxisLimits(int nowIndex)
        {
            var axis = AxisXCollection[0]; // 첫 번째 X축 가져오기
            axis.MaxValue = nowIndex + 1; // 축 최대값 설정
            axis.MinValue = nowIndex - 8 >= 0 ? nowIndex - 8 : 0; // 축 최소값 설정
        }

        // 타이머 Tick 이벤트 처리기
        private void TimerOnTick(object sender, EventArgs e)
        {
            if (currentDataIndex >= filteredIndex.Count) return;

            var nowIndex = filteredIndex[currentDataIndex]; // 필터된 인덱스 가져오기
            var value = filteredTemperatures[currentDataIndex]; // 필터된 온도 가져오기

            // 새로운 데이터 포인트 추가
            ChartValues.Add(new MeasureModel
            {
                Index = nowIndex,
                Value = value
            });

            SetAxisLimits(nowIndex); // 축 한계 업데이트

            // 최근 30개의 값만 유지
            if (ChartValues.Count > 30) ChartValues.RemoveAt(0); // 가장 오래된 데이터 포인트 제거

            currentDataIndex++;
        }

        // PropertyChanged 이벤트
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 데이터 모델 클래스
    public class MeasureModel
    {
        public int Index { get; set; }
        public double Value { get; set; }
    }

}