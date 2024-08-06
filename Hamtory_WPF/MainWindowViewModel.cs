using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Hamtory_WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // MainWindowViewModel의 필드
        ProcessData dataLoader = new ProcessData();
        private List<DateTime> filteredDates;
        private List<int> filteredTemperatures;
        private List<int> filteredIndex;
        private List<int> filteredMotorSpeed;
        private List<int> fillteredMeltWeight;
        private int currentDataIndex = 0;

        // MainWindowViewModel2의 필드
        private List<double> filteredMoisture;
        private int currentMoistureDataIndex = 0;

        public MainWindowViewModel()
        {
            List<DataValues> datas = dataLoader.LoadDataFile("melting_tank.csv");

            DateTime targetDate = new DateTime(2020, 3, 4);

            List<DateTime> date_datas = new List<DateTime>();
            List<int> melt_temperature_datas = new List<int>();
            List<int> index = new List<int>();
            List<int> motor_speed = new List<int>();
            List<int> melt_weight = new List<int>();
            List<double> moisture = new List<double>();

            // 데이터 값 채우기
            foreach (DataValues data in datas)
            {
                date_datas.Add(data.date);
                melt_temperature_datas.Add(data.melt_temperature);
                index.Add(data.index);
                motor_speed.Add(data.motor_speed);
                melt_weight.Add(data.melt_weight);
                moisture.Add(data.moisture);
            }

            // 필터링된 데이터 생성
            filteredDates = new List<DateTime>();
            filteredTemperatures = new List<int>();
            filteredIndex = new List<int>();
            filteredMotorSpeed = new List<int>();
            fillteredMeltWeight = new List<int>();
            filteredMoisture = new List<double>();

            for (int i = 0; i < date_datas.Count; i++)
            {
                if (date_datas[i].Date == targetDate.Date)
                {
                    filteredDates.Add(date_datas[i]);
                    filteredTemperatures.Add(melt_temperature_datas[i]);
                    filteredIndex.Add(index[i]);
                    filteredMotorSpeed.Add(motor_speed[i]);
                    fillteredMeltWeight.Add(melt_weight[i]);
                    filteredMoisture.Add(moisture[i]);
                }
            }

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

            // SeriesCollection 설정
            SeriesCollectionTemperature = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "용해온도",
                    Values = ChartValuesTemperature, // ChartValues를 LineSeries의 값으로 설정
                    PointGeometrySize = 10,         // 데이터 점의 크기
                    StrokeThickness = 4             // 선의 두께
                },
                new LineSeries
                {
                    Title = "모터 속도",
                    Values = ChartValuesMotorSpeed, // ChartValuesMotorSpeed를 LineSeries의 값으로 설정
                    PointGeometrySize = 10,         // 데이터 점의 크기
                    StrokeThickness = 4             // 선의 두께
                },
                new LineSeries
                {
                    Title = "용해 무게",
                    Values = ChartValuesMeltWeight, // ChartValuesMeltWeight를 LineSeries의 값으로 설정
                    PointGeometrySize = 10,         // 데이터 점의 크기
                    StrokeThickness = 4             // 선의 두께
                }
            };

            SeriesCollectionMoisture = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "수분 함유량(%)",
                    Values = ChartValuesMoisture, // ChartValuesMoisture를 LineSeries의 값으로 설정
                    PointGeometrySize = 10,       // 데이터 점의 크기
                    StrokeThickness = 4           // 선의 두께
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
            AxisYCollection2 = new AxesCollection
            {
                new Axis
                {
                    Title = "Moisture (%)",
                    LabelFormatter = value => value.ToString("0.00") // Y축 레이블 포맷 (소수점 둘째 자리까지)
                }
            };

            // 초기 축 한계 설정
            SetAxisLimits(0);

            // 데이터를 500ms마다 업데이트하는 타이머 설정
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // 300ms 간격으로 타이머 설정
            };
            Timer.Tick += TimerOnTick; // 타이머의 Tick 이벤트에 처리기 추가
            Timer.Start(); // 타이머 시작
        }

        // 차트에 표시할 데이터 컬렉션

        public SeriesCollection SeriesCollectionTemperature { get; set; }
        public SeriesCollection SeriesCollectionMoisture { get; set; }
        public AxesCollection AxisXCollectionTemperature { get; set; }
        public AxesCollection AxisXCollectionMoisture { get; set; }
        public AxesCollection AxisYCollection2 { get; set; }
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