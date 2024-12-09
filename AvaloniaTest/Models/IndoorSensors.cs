using Avalonia.Media;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using AvaloniaTest.Services.Enums;
using Avalonia.Controls;
using AvaloniaTest.Helpers;
using System.Collections.ObjectModel;

namespace AvaloniaTest.Models
{
    public partial class IndoorSensors : ObservableObject
    {
        [ObservableProperty]
        public SensorInfo<double> _temperature;

        [ObservableProperty]
        public SensorInfo<double> _humidity;

        [ObservableProperty]
        public SensorInfo<double> _pressure;

        [ObservableProperty]
        public SensorInfo<int> _altitude;

        [ObservableProperty]
        public SensorInfo<double> _illuminance;

        [ObservableProperty]
        public SensorInfo<double> _CO;

        [ObservableProperty]
        public StreamGeometry _tempIcon;


        [ObservableProperty]
        public SensorInfo<double> _minTemp;
        [ObservableProperty]
        public SensorInfo<double> _maxTemp;
        [ObservableProperty]
        public SensorInfo<double> _lastTemp;
        private double lastTemp;

        [ObservableProperty]
        public double _humiditycircle = 0;

        public ObservableCollection<string> Preasurecolors { get; } = new()
        {
            "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00"
        };
        public ObservableCollection<string> Iluminancecolors { get; } = new()
        {
             "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00"
        };

        [ObservableProperty]
        public int _coPosition;


        private DataBaseService DataBaseService;
        [ObservableProperty]
        private UnitsSettings _unit;
        private UnitsConverter Converter;


        public IndoorSensors(DataBaseService dataBase, UnitsSettings units, UnitsConverter convert)
        {
            DataBaseService = dataBase;
            Unit = units;
            Converter = convert;

            Temperature = new SensorInfo<double>
            (
                "indoortemperature",
                false,
                ()=>Unit.Temp,
                Converter.CalculateTemp
            );
            Temperature.Value = ErrorValues.DoubleError;
            Humidity = new SensorInfo<double>
                (
                "indoorhumidity",
                true
                );

            Pressure = new SensorInfo<double>
                (
                "indoorpressure",
                true
                );
            Altitude = new SensorInfo<int>("indooraltitude");
            Illuminance = new SensorInfo<double>("indoorilluminance");
            CO = new SensorInfo<double>("indoorCO", true);


            MinTemp = new SensorInfo<double>
            (
                "indoormintemperature",
                false,
                null,
                Converter.CalculateTemp
            );

            MaxTemp = new SensorInfo<double>
            (
                "indoormaxtemperature",
                false,
                null,
                Converter.CalculateTemp
            );
            LastTemp = new SensorInfo<double>
            (
                "indoorlasttemperature",
                false,
                null,
                Converter.CalculateTemp
            );

            Temperature.PropertyChanged += OnTemperatureChanged;
            SetMinMaxTemp();
            SetLastTemp();
            Unit.PropertyChanged += Unit_PropertyChanged;
            Humidity.PropertyChanged += OnHumidityChanged;
            Pressure.PropertyChanged += OnPressureChanged;
            Illuminance.PropertyChanged += OnIlluminanceChanged;
            CO.PropertyChanged += OnCOChanged;

        }

        private void OnCOChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CO.Value))
            {
                CoPosition = GraphicsGauges.GetPointPosition(5, 0, 250, CO.Value);
            }
        }


        private void OnPressureChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Pressure.Value))
            {
                GraphicsGauges.SetBarColors(Pressure.Value, 1054, 960, "in", Preasurecolors);
            }
        }


        private void OnIlluminanceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Illuminance.Value))
            {
                GraphicsGauges.SetBarColors(Illuminance.Value, 2000, 0, "in", Iluminancecolors);
            }
        }


        private void OnHumidityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Humidity.Value))
            {
                Humiditycircle = GraphicsGauges.GetCircleGaugeValue(Humidity.Value);
            }
        }




        private void Unit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Unit.Temp))
            {
                Temperature.Recalculate();
                MinTemp.Recalculate();
                MaxTemp.Recalculate();
                LastTemp.Recalculate();
            }

        }

        private void OnTemperatureChanged(object sender, PropertyChangedEventArgs e)
        {
            // Sprawdź, czy zmiana dotyczy wartości
            if (e.PropertyName == nameof(Temperature.Value))
            {
                // Tutaj dodaj kod, który ma reagować na zmianę wartości
                SetLastTemp();
                SetIcon(TempIcon, lastTemp, Temperature.Value);
                SetMinMaxTemp();
            }
        }


        private void SetLastTemp()
        {
            lastTemp = DataBaseService.GetValueFrom24HoursAgo<double>("innerTemperature");
            if (lastTemp == ErrorValues.GetErrorValue<double>())
            {
                LastTemp.Value = ErrorValues.DoubleError;
            }
            else
            {
                LastTemp.Value = lastTemp;
            }
        }
        private void SetMinMaxTemp()
        {
            double min;
            double max;
            (min, max) = DataBaseService.GetTodayMinMaxValue<double>("innerTemperature");
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
            else if (value24h < value) // teraz jest wiecej
            {
                iconName = "ArrowDown";
            }
            else // teraz jest mniej 
            {
                iconName = "ArrowUp";
            }
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                TempIcon = (StreamGeometry)App.Current.FindResource(iconName);
            });
        }

    }
}
