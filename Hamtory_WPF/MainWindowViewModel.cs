using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;


namespace Hamtory_WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // MainWindowViewModel의 필드

        private List<DateTime> filteredDates;
        private List<int> filteredTemperatures;
        private List<int> filteredIndex;
        private List<int> filteredMotorSpeed;
        private List<int> fillteredMeltWeight;
        private int currentDataIndex = 0;

        // MainWindowViewModel2의 필드
        private List<double> filteredMoisture;
        private int currentMoistureDataIndex = 0;

        //현재 온도 변수 추가 (원형 차트)
        private int currentTemperature;
        public int CurrentTemperature
        {
            get { return currentTemperature; }
            set
            {
                currentTemperature = value;
                OnPropertyChanged(nameof(CurrentTemperature));
            }
        }

        private int currentMotorSpeed;
        public int CurrentMotorSpeed
        {
            get { return currentMotorSpeed; }
            set
            {
                currentMotorSpeed = value;
                OnPropertyChanged(nameof(CurrentMotorSpeed));
            }
        }

        private int currentMeltWeight;
        public int CurrentMeltWeight
        {
            get { return currentMeltWeight; }
            set
            {
                currentMeltWeight = value;
                OnPropertyChanged(nameof(CurrentMeltWeight));
            }
        }

        private double currentMoisture;
        public double CurrentMoisture
        {
            get { return currentMoisture; }
            set
            {
                currentMoisture = value;
                OnPropertyChanged(nameof(CurrentMoisture));
            }
        }
        public MainWindowViewModel(DateTime targetDate)
        {
        List<DataValues> datas = DataList.datas;
            // 데이터 값 채우기 및 필터링된 데이터 생성
            var filteredData = datas
                .Where(data => data.date.Date == targetDate.Date)
                .ToList();

            filteredDates = filteredData.Select(data => data.date).ToList();
            filteredTemperatures = filteredData.Select(data => data.melt_temperature).ToList();
            filteredIndex = filteredData.Select(data => data.index).ToList();
            filteredMotorSpeed = filteredData.Select(data => data.motor_speed).ToList();
            fillteredMeltWeight = filteredData.Select(data => data.melt_weight).ToList();
            filteredMoisture = filteredData.Select(data => data.moisture).ToList();

            // MeasureModel을 전역적으로 설정
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.Index) // Index를 X 값으로 사용
                .Y(model => model.Value); // Value 속성을 Y 값으로 사용

            // MeasureModel에 대한 매퍼를 전역적으로 설정
            Charting.For<MeasureModel>(mapper);

            // ChartValues 초기화
            ChartValuesTemperature = new ChartValues<MeasureModel>();
            ChartValuesMotorSpeed = new ChartValues<MeasureModel>();
            ChartValuesMeltWeight = new ChartValues<MeasureModel>();
            ChartValuesMoisture = new ChartValues<MeasureModel>();


            //용해속도, 용해량, 용해온도 Y 축
            AxisYMoisture = new AxesCollection
            {
                new Axis
                {
                    LabelFormatter = value => value.ToString("0")
                }
            };
            // SeriesCollection 설정
            SeriesCollectionTemperature = new SeriesCollection
            {

                new LineSeries
                {
                    Title = "모터 속도",
                    Values = ChartValuesMotorSpeed, // ChartValuesMotorSpeed를 LineSeries의 값으로 설정
                    PointGeometrySize = 5,         // 데이터 점의 크기
                    StrokeThickness = 2,             // 선의 두께
                    Stroke = new SolidColorBrush(Color.FromRgb(160, 110, 220)),
                    Fill = new SolidColorBrush(Color.FromArgb(90, 160, 110, 220)),
                    LineSmoothness = 0
                },
                new LineSeries
                {
                    Title = "용해온도",
                    Values = ChartValuesTemperature, // ChartValues를 LineSeries의 값으로 설정
                    PointGeometrySize = 5,         // 데이터 점의 크기
                    StrokeThickness = 2,             // 선의 두께
                    Stroke = new SolidColorBrush(Color.FromRgb(200, 50, 50)),
                    Fill = new SolidColorBrush(Color.FromArgb(90, 190, 50, 50)),
                    LineSmoothness = 0
                },
                new LineSeries
                {
                    Title = "용해 무게",
                    Values = ChartValuesMeltWeight, // ChartValuesMeltWeight를 LineSeries의 값으로 설정
                    PointGeometrySize = 5,         // 데이터 점의 크기
                    StrokeThickness = 2,             // 선의 두께
                    Stroke = new SolidColorBrush(Color.FromRgb(90, 180, 80)),
                    Fill = new SolidColorBrush(Color.FromArgb(50, 90, 190, 80)),
                    LineSmoothness = 0
                }
            };

            SeriesCollectionMoisture = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "수분 함유량",
                    Values = ChartValuesMoisture, // ChartValuesMoisture를 LineSeries의 값으로 설정
                    PointGeometrySize = 5,       // 데이터 점의 크기
                    StrokeThickness = 2,           // 선의 두께   
                    Stroke = new SolidColorBrush(Color.FromRgb(12, 130, 210)),
                    //Fill = Brushes.LightBlue,     
                    LineSmoothness = 1
                }
            };

            // AxisX 설정
            AxisXCollectionTemperature = new AxesCollection
            {
                new Axis
                {
                    DisableAnimations = true, // 애니메이션 비활성화
                    LabelFormatter = value =>
                    {
                        int index = (int)value;
                        if (index >= 0 && index < filteredDates.Count)
                        {
                            return filteredDates[index].ToString("HH:mm");
                        }
                        return "";
                    }, // X축 레이블 포맷
 
                    Separator = new Separator
                    {
                        Step = 1 // X축 구분선 간격 설정
                    }
                }
            };

            AxisXCollectionMoisture = new AxesCollection
            {
                new Axis
                {
                    DisableAnimations = true, // 애니메이션 비활성화
                    LabelFormatter = value =>
                    {
                        int index = (int)value;
                        if (index >= 0 && index < filteredDates.Count)
                        {
                            return filteredDates[index].ToString("HH:mm");
                        }
                        return "";
                    }, // X축 레이블 포맷
                    Separator = new Separator
                    {
                        Step = 1 // X축 구분선 간격 설정
                    },
                }
            };
            //수분 함유량 Y 축
            AxisYMoisture = new AxesCollection
            {
                new Axis
                {
                    DisableAnimations = true,
                    Title = "Moisture (%)",
                    LabelFormatter = value => value.ToString("0.00"), // Y축 레이블 포맷 (소수점 둘째 자리까지)
                }
            };


            // 초기 축 한계 설정
            SetAxisLimits(0);

            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // 300ms 간격으로 타이머 설정
            };
            Timer.Tick += TimerOnTick; // 타이머의 Tick 이벤트에 처리기 추가
            Timer.Start(); // 타이머 시작
        }
        public void StartTimer()
        {
            Timer.Start();
        }

        public void StopTimer()
        {
            Timer.Stop();
        }
        // 차트에 표시할 데이터 컬렉션

        public SeriesCollection SeriesCollectionTemperature { get; set; }
        public SeriesCollection SeriesCollectionMoisture { get; set; }
        public AxesCollection AxisXCollectionTemperature { get; set; }
        public AxesCollection AxisXCollectionMoisture { get; set; }
        public AxesCollection AxisYMoisture { get; set; }
        public AxesCollection AxisYMelt { get; set; }
        public ChartValues<MeasureModel> ChartValuesTemperature { get; set; }
        public ChartValues<MeasureModel> ChartValuesMotorSpeed { get; set; }
        public ChartValues<MeasureModel> ChartValuesMeltWeight { get; set; }
        public ChartValues<MeasureModel> ChartValuesMoisture { get; set; }
        public DispatcherTimer Timer { get; set; }


        // 축 한계 설정 메서드
        private void SetAxisLimits(int nowIndex)
        {
            var axisTemp = AxisXCollectionTemperature[0]; // 첫 번째 X축 가져오기
            axisTemp.MaxValue = nowIndex + 1; // 축 최대값 설정
            axisTemp.MinValue = nowIndex - 8 >= 0 ? nowIndex - 8 : 0; // 축 최소값 설정

            var axisMoist = AxisXCollectionMoisture[0]; // 첫 번째 X축 가져오기
            axisMoist.MaxValue = nowIndex + 1; // 축 최대값 설정
            axisMoist.MinValue = nowIndex - 8 >= 0 ? nowIndex - 8 : 0; // 축 최소값 설정
        }

        // 타이머 Tick 이벤트 처리기
        private void TimerOnTick(object sender, EventArgs e)
        {
            if (currentDataIndex >= filteredIndex.Count) return;

            var nowIndex = filteredIndex[currentDataIndex]; // 필터된 인덱스 가져오기

            //현재 값들 저장 (Binding 목적)
            CurrentTemperature = filteredTemperatures[currentDataIndex];
            CurrentMotorSpeed = filteredMotorSpeed[currentDataIndex];
            CurrentMeltWeight = fillteredMeltWeight[currentDataIndex];
            CurrentMoisture = filteredMoisture[currentDataIndex];

            // 새로운 데이터 포인트 추가
            ChartValuesTemperature.Add(new MeasureModel
            {
                Index = nowIndex,
                Value = filteredTemperatures[currentDataIndex] // 필터된 온도 가져오기
            });

            ChartValuesMotorSpeed.Add(new MeasureModel
            {
                Index = nowIndex,
                Value = filteredMotorSpeed[currentDataIndex] // 필터된 모터 속도 가져오기
            });

            ChartValuesMeltWeight.Add(new MeasureModel
            {
                Index = nowIndex,
                Value = fillteredMeltWeight[currentDataIndex] // 필터된 용해 무게 가져오기
            });

            ChartValuesMoisture.Add(new MeasureModel
            {
                Index = nowIndex,
                Value = filteredMoisture[currentDataIndex] // 필터된 수분함유량
            });

            SetAxisLimits(nowIndex); // 축 한계 업데이트

            // 최근 30개의 값만 유지
            if (ChartValuesTemperature.Count > 30) ChartValuesTemperature.RemoveAt(0); // 가장 오래된 데이터 포인트 제거
            if (ChartValuesMotorSpeed.Count > 30) ChartValuesMotorSpeed.RemoveAt(0);
            if (ChartValuesMeltWeight.Count > 30) ChartValuesMeltWeight.RemoveAt(0);
            if (ChartValuesMoisture.Count > 30) ChartValuesMoisture.RemoveAt(0);

            currentDataIndex++;
        }

        // PropertyChanged 이벤트 (값 바뀔 때 마다 실행)
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