using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaTest.Helpers;
using AvaloniaTest.Interfaces;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Models.Sensors;
using AvaloniaTest.Services;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.Services.Enums;
using AvaloniaTest.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models
{
    public partial class OutdoorSensors : ObservableObject
    {
        [ObservableProperty]
        public SensorInfo<double> _temperature;
        [ObservableProperty]
        public SensorInfo<double> _pressure;
        [ObservableProperty]
        public SensorInfo<int> _altitude;
        [ObservableProperty]
        public SensorInfo<double> _humidity;
        [ObservableProperty]
        public SensorInfo<double> _illuminance;
        [ObservableProperty]
        public SensorInfo<double> _NO2;
        [ObservableProperty]
        public SensorInfo<double> _NH3;
        [ObservableProperty]
        public SensorInfo<double> _CO;
        [ObservableProperty]
        public SensorInfo<double> _rain;
        [ObservableProperty]
        public Wind _wind;
        [ObservableProperty]
        public StreamGeometry _temperatureicon;
        private double lastTemp;
        [ObservableProperty]
        public SensorInfo<double> _minTemp;
        [ObservableProperty]
        public SensorInfo<double> _maxTemp;
        [ObservableProperty]
        public SensorInfo<double> _lastTemp;


        [ObservableProperty]
        public double _humiditycircle = 0;

        public ObservableCollection<string> Preasurecolors { get; } = new()
        {
            "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50"
        };
        public ObservableCollection<string> Iluminancecolors { get; } = new()
        {
               "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50"
        };

        [ObservableProperty]
        public int _coPosition;
        [ObservableProperty]
        public int _no2Position;
        [ObservableProperty]
        public int _nh3Position;

        private IDataBaseService DataBaseService;

        [ObservableProperty]
        private UnitsSettings _unit;

        private UnitsConverter Converter;

        public OutdoorSensors(IDataBaseService dataBase, UnitsSettings units, UnitsConverter convert)
        {

            
            DataBaseService = dataBase;
            Unit = units;
            Converter = convert;
            Temperature = new SensorInfo<double>(
                "outdoortemperature",
                false,
                 () => Unit.Temp,
                Converter.CalculateTemp
                );
            Temperature.Value = ErrorValues.DoubleError;
            Pressure = new SensorInfo<double>(
                "outdoorpressure",
                true
                );

            Humidity = new SensorInfo<double>("outdoorhumidity",
                true
                );


            Illuminance = new SensorInfo<double>("outdoorilluminance",true);
            Altitude = new SensorInfo<int>("outdooraltitude");
            CO = new SensorInfo<double>("outdoorCO", true);
            NH3 = new SensorInfo<double>("outdoorNH3", true);
            NO2 = new SensorInfo<double>("outdoorNO2",true);
            Wind = new Wind(Unit, Converter);

            Rain = new SensorInfo<double>(
                "outdoorrain",
                false,
                () =>Unit.Rain,
                null,
                true
                );

            /*  Wind = new SensorInfo<double>("outdoorwind",
                  false,
                  () =>Unit.Wind,
                  Converter.CalculateWind,
                  false);

              Gust = new SensorInfo<double>("outdoorwindgust", false,()=> Unit.Wind, Converter.CalculateWind,false);*/

            // Wind.PropertyChanged += OnOutdoorWindChanged;
            Temperature.PropertyChanged += OnOutdoorTemperatureChanged;
            

            MinTemp = new SensorInfo<double>
            (
                "outdoormintemperature",
                false,
                null,
                Converter.CalculateTemp
            );

            MaxTemp = new SensorInfo<double>
            (
                "outdoormaxtemperature",
                false,
                null,
                Converter.CalculateTemp
            );
           LastTemp = new SensorInfo<double>
            (
                "outdoorlasttemperature",
                false,
                null,
                Converter.CalculateTemp
            );
            SetMinMaxTemp();
            SetLastTemp();

            Unit.PropertyChanged += Unit_PropertyChanged;

            Humidity.PropertyChanged += OnHumidityChanged;
            Pressure.PropertyChanged += OnPressureChanged;
            Illuminance.PropertyChanged += OnIlluminanceChanged;
            CO.PropertyChanged += OnCOChanged;
            NH3.PropertyChanged += OnNH3Changed;
            NO2.PropertyChanged += OnNO2Changed;
        }


        //Co zrobić w przypadku zmiany jednostki/ Wykrywanie zmiany dowolnej jednostki 
        private void Unit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Unit.Temp))
            {
                Console.WriteLine("Zmieniono jednostkę - wiem o tym w outdoorsensors");
                Temperature.Recalculate();
                MinTemp.Recalculate();
                MaxTemp.Recalculate();
                LastTemp.Recalculate();
            }
            else if (e.PropertyName == nameof(Unit.Wind))
            {
               //WindSpeed.Recalculate();
               //Gust.Recalculate();
                Console.WriteLine("Zmieniono wiatr- odczytano to w klasie OutdoorSensors");
                
            }
            else if (e.PropertyName == nameof(Unit.Rain))
            {
                Rain.Recalculate(); 
            }

        }

        private void OnCOChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CO.Value)) 
            { 
                CoPosition = GraphicsGauges.GetPointPosition(5, 0, 250, CO.Value);
            }
        }
        private void OnNO2Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NO2.Value))
            {
                No2Position = GraphicsGauges.GetPointPosition(15, 0, 10, NO2.Value);
            }
        }
        private void OnNH3Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NH3.Value))
            {
               Nh3Position = GraphicsGauges.GetPointPosition(15, 0, 180, NH3.Value);
            }
        }
        private void OnPressureChanged(object sender, PropertyChangedEventArgs e)
        {
         
            if (e.PropertyName == nameof(Pressure.Value))
            {
                  GraphicsGauges.SetBarColors(Pressure.Value, 1054, 960, "out", Preasurecolors);
            }
        }


        private void OnIlluminanceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Illuminance.Value))
            {
                GraphicsGauges.SetBarColors(Illuminance.Value, 2000, 0, "out", Iluminancecolors);
            }
        }


        private void OnHumidityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Humidity.Value))
            {
                Humiditycircle = GraphicsGauges.GetCircleGaugeValue(Humidity.Value);
            }
        }

        private void OnOutdoorTemperatureChanged(object sender, PropertyChangedEventArgs e)
        {
            // Sprawdź, czy zmiana dotyczy wartości
            if (e.PropertyName == nameof(Temperature.Value))
            {
                // Tutaj dodaj kod, który ma reagować na zmianę wartości
               //Co robić jeżeli przyjdzie nowa wartość - ustaw co było 24h temu, ikona, ustaw min max
                SetLastTemp();
                SetIcon(Temperatureicon, lastTemp, Temperature.Value);
                SetMinMaxTemp();
            }
        }



        private void SetLastTemp()
        {
           
            lastTemp = DataBaseService.GetValueFrom24HoursAgo<double>("outerTemperature");
            if (lastTemp == ErrorValues.GetErrorValue<double>())
            {
              
                LastTemp.Value = ErrorValues.DoubleError;
            }
            else
            {
             //   double calculatedTemp = Converter.CalculateTemp(lastTemp);
             //   LastTemperature = calculatedTemp.ToString().Replace(",", ".");
             LastTemp.Value = lastTemp; 
            }

        }

        private void SetMinMaxTemp()
        {
            double min;
            double max;
            (min, max) = DataBaseService.GetTodayMinMaxValue<double>("outerTemperature");
            if (min == ErrorValues.GetErrorValue<double>())
            {

                MinTemp.Value = ErrorValues.DoubleError;
            }
            else
            {
                if (min > Temperature.Value && Temperature.Value != ErrorValues.DoubleError)
                {
                    min = Temperature.Value;
                }
                MinTemp.Value = min;
              //  MinTemperature = Converter.CalculateTemp(min).ToString().Replace(",", ".");
            }


            if (max == ErrorValues.GetErrorValue<double>())
            {
                MaxTemp.Value = ErrorValues.DoubleError;
            }
            else
            {
                if (max < Temperature.Value && Temperature.Value != ErrorValues.DoubleError)
                {
                    max = Temperature.Value;
                }
                MaxTemp.Value = max;
               // MaxTemperature = Converter.CalculateTemp(max).ToString().Replace(",", ".");
            }
        }

        private void SetIcon(StreamGeometry icon, double value24h, double value)
        {
            double tolerance = 0.5;
            bool isWithinTolerance = Math.Abs(value24h - value) <= tolerance;
            string iconName;
            if (isWithinTolerance) // wartosc +-taka sama 
            {
                iconName = "ArrowRepeat";
            }
            else if(value24h < value) // teraz jest wiecej
            {
                iconName = "ArrowDown";
            }
            else // teraz jest mniej 
            {
                iconName = "ArrowUp";
            }
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
               Temperatureicon = (StreamGeometry)App.Current.FindResource(iconName);
            });
        }
    }

}
