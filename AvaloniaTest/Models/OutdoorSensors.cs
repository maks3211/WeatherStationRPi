﻿using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaTest.Helpers;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Services;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.Services.Enums;
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
        public SensorInfo<double> _outdoorTemperature;
        [ObservableProperty]
        public SensorInfo<double> _outdoorPressure;
        [ObservableProperty]
        public SensorInfo<int> _outdoorAltitude;
        [ObservableProperty]
        public SensorInfo<double> _outdoorHumidity;
        [ObservableProperty]
        public SensorInfo<double> _outdoorIlluminance;
        [ObservableProperty]
        public SensorInfo<double> _outdoorNO2;
        [ObservableProperty]
        public SensorInfo<double> _outdoorNH3;
        [ObservableProperty]
        public SensorInfo<double> _outdoorCO;


        [ObservableProperty]
        public StreamGeometry _temperatureicon;
      


        private double lastTemp;


        [ObservableProperty]
        public SensorInfo<double> _minTemp;
        [ObservableProperty]
        public SensorInfo<double> _maxTemp;
        [ObservableProperty]
        public SensorInfo<double> _lastTemp;



        private DataBaseService DataBaseService;

        [ObservableProperty]
        private UnitsSettings _unit;

        private UnitsConverter Converter;

        public OutdoorSensors(DataBaseService dataBase, UnitsSettings units, UnitsConverter convert)
        {


            DataBaseService = dataBase;
            Unit = units;
            Converter = convert;
            OutdoorTemperature = new SensorInfo<double>(
                "outdoortemperature",
                false,
                 () => Unit.Temp,
                Converter.CalculateTemp
                );

            OutdoorPressure = new SensorInfo<double>(
                "outdoorpressure",
                true
                );

            OutdoorHumidity = new SensorInfo<double>("outdoorhumidity",
                true
                );


            OutdoorIlluminance = new SensorInfo<double>("outdoorilluminance",true);
            OutdoorAltitude = new SensorInfo<int>("outdooraltitude");
            OutdoorCO = new SensorInfo<double>("outdoorCO", true);
            OutdoorNH3 = new SensorInfo<double>("outdoorNH3", true);
            OutdoorNO2 = new SensorInfo<double>("outdoorNO2",true);
            OutdoorTemperature.PropertyChanged += OnOutdoorTemperatureChanged;
           
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


        }


        //Wykrywanie zmiany dowolnej jednostki 
        private void Unit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Unit.Temp))
            {
                Console.WriteLine("Zmieniono jednostkę - wiem o tym w outdoorsensors");
                OutdoorTemperature.Recalculate();
                MinTemp.Recalculate();
                MaxTemp.Recalculate();
                LastTemp.Recalculate();
            }
            else if (e.PropertyName == nameof(Unit.Wind))
            {
                Console.WriteLine("Zmieniono wiatr- odczytano to w klasie OutdoorSensors");
            }
        }

        private void OnOutdoorTemperatureChanged(object sender, PropertyChangedEventArgs e)
        {
            // Sprawdź, czy zmiana dotyczy wartości
            if (e.PropertyName == nameof(OutdoorTemperature.Value))
            {
                // Tutaj dodaj kod, który ma reagować na zmianę wartości
               
                SetLastTemp();
                SetIcon(Temperatureicon, lastTemp, OutdoorTemperature.Value);
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
                if (min > OutdoorTemperature.Value)
                {
                    min = OutdoorTemperature.Value;
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
                if (max < OutdoorTemperature.Value)
                {
                    max = OutdoorTemperature.Value;
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
