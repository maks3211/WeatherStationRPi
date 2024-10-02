using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Services;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models
{
    public partial  class OutdoorSensors : ObservableObject
    {
        [ObservableProperty]
        public MqttTopic<double> _outdoorTemperature;
        [ObservableProperty]
        public MqttTopic<double> _outdoorPressure;
        [ObservableProperty]
        public MqttTopic<int> _outdoorAltitude;
        [ObservableProperty]
        public MqttTopic<double> _outdoorHumidity;
        [ObservableProperty]
        public MqttTopic<double> _outdoorIlluminance;
        [ObservableProperty]
        public MqttTopic<double> _outdoorNO2;
        [ObservableProperty]
        public MqttTopic<double> _outdoorNH3;
        [ObservableProperty]
        public MqttTopic<double> _outdoorCO;


        [ObservableProperty]
        public StreamGeometry _temperatureicon;
        [ObservableProperty]
        public string _lastTemperature;
        [ObservableProperty]
        public string _minTemperature;
        [ObservableProperty]
        public string _maxTemperature;

        private double lastTemp;

     

        private DataBaseService DataBaseService;

        [ObservableProperty]
        private UnitsSettings _unit;

        private UnitsConverter Converter;

        public OutdoorSensors(DataBaseService dataBase, UnitsSettings units, UnitsConverter convert)
        {
            DataBaseService = dataBase;
            Unit = units;
            Converter = convert;
            OutdoorTemperature = new MqttTopic<double>(
                "outdoortemperature",
                false,
                 () => Unit.Temp,
                Converter.CalculateTemp
                );

            OutdoorPressure = new MqttTopic<double>(
                "outdoorpressure",
                true
                );

            OutdoorHumidity = new MqttTopic<double>("outdoorhumidity",
                true
                );

            OutdoorIlluminance = new MqttTopic<double>("outdoorilluminance");
            OutdoorAltitude = new MqttTopic<int>("outdooraltitude");
            OutdoorCO = new MqttTopic<double>("outdoorCO", true);
            OutdoorNH3 = new MqttTopic<double>("outdoorNH3", true);
            OutdoorNO2 = new MqttTopic<double>("outdoorNO2",true);
            OutdoorTemperature.PropertyChanged += OnOutdoorTemperatureChanged;
            SetMinMaxTemp();
            SetLastTemp();

            Unit.PropertyChanged += Unit_PropertyChanged;

        }

        private void Unit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Unit.Temp))
            {
                OutdoorTemperature.Recalculate();
            }
        }

        private void OnOutdoorTemperatureChanged(object sender, PropertyChangedEventArgs e)
        {
            // Sprawdź, czy zmiana dotyczy wartości
            if (e.PropertyName == nameof(OutdoorTemperature.Value))
            {
                // Tutaj dodaj kod, który ma reagować na zmianę wartości
                Console.WriteLine($"Nowa wartość temperatury na zewnątrz: {OutdoorTemperature.Value}");
                SetLastTemp();
                SetIcon(Temperatureicon, lastTemp, OutdoorTemperature.Value);
                SetMinMaxTemp();
            }
        }

        private void SetLastTemp()
        {
            lastTemp = DataBaseService.GetValueFrom24HoursAgo<double>("outerTemperature");
            double calculatedTemp = Converter.CalculateTemp(lastTemp);
            LastTemperature = calculatedTemp.ToString().Replace(",", ".");
        }

        private void SetMinMaxTemp()
        {
            double min;
            double max;
            (min, max) = DataBaseService.GetTodayMinMaxValue<double>("outerTemperature");
            if (min > OutdoorTemperature.Value)
            {
                min = OutdoorTemperature.Value;
            }
            if (max < OutdoorTemperature.Value)
            {
                max = OutdoorTemperature.Value;
            }
            MinTemperature = Converter.CalculateTemp(min).ToString().Replace(",", ".");
            MaxTemperature = Converter.CalculateTemp(max).ToString().Replace(",", ".");
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
                Console.WriteLine("teraz wiecej");
                iconName = "ArrowDown";
            }
            else // teraz jest mniej 
            {
                Console.WriteLine("mniej");
                iconName = "ArrowUp";
            }
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
               Temperatureicon = (StreamGeometry)App.Current.FindResource(iconName);
            });
        }
    }

}
