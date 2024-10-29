using Avalonia.Threading;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Services.AppSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models.Sensors
{
    public partial class Wind: ObservableObject
    {




        [ObservableProperty]
        private UnitsSettings _unit;

        private UnitsConverter Converter;


        [ObservableProperty]
        public SensorInfo<double> _windSpeed;
        [ObservableProperty]
        public SensorInfo<double> _gust;


       
        [ObservableProperty]
        public string _direction;
        [ObservableProperty]
        public double _arrowx = 150;
        [ObservableProperty]
        public double _arrowy = 150;
        [ObservableProperty]
        public double _circlex = 150;
        [ObservableProperty]
        public double _circley = 150;

        [ObservableProperty]
        public double _angle = 0;

        [ObservableProperty]
        public double _arrowAngle = 0;


        private Queue<double> WindSpeeds = new Queue<double>();
        private int GustCounter = 0;




        private double targetArrowX;
        private double targetArrowY;
        private double targetCircleX;
        private double targetCircleY;
        private double targetArrowAngle;


        public Wind(UnitsSettings units, UnitsConverter convert)
        { 
            Unit = units;
            Converter = convert;

            WindSpeed = new SensorInfo<double>("outdoorwind",
            false,
            () => Unit.Wind,
            Converter.CalculateWind,
            false);

            Gust = new SensorInfo<double>("outdoorwindgust", false, () => Unit.Wind, Converter.CalculateWind, false);

            WindSpeed.PropertyChanged += OnOutdoorWindChanged;
            Unit.PropertyChanged += Unit_PropertyChanged;
            ChangeWindArrow(0);
        }




        partial void OnAngleChanged(double value)
        {
            ChangeWindArrow(value);
        }







        /// <summary>
        /// Calculate and sets arrow position
        /// </summary>
        /// <param name="angle">Direction of wind</param>
        public void ChangeWindArrow(double angle)
        {
                     double radians = Math.PI * angle / 180;
                        double arrowLength = 77;

                        Arrowx = 140 + arrowLength * Math.Sin(radians);
                        Arrowy = 145 - arrowLength * Math.Cos(radians) - 50;

                        double circleAngle = angle + 180;
                        double circleRadians = Math.PI * circleAngle / 180;
                        double circleLength = 77;

                        Circlex = 140 + circleLength * Math.Sin(circleRadians);
                        Circley = 140 - circleLength * Math.Cos(circleRadians) - 50;
                        ArrowAngle = angle - 90 ;
          

            switch (angle)
            {
                case double v when (v >= 0 && v < 22.5) || (v >= 337.5 && v <= 360):
                    Direction = "S";
                    break;
                case double v when v >= 22.5 && v < 67.5:
                    Direction = "SW";
                    break;
                case double v when v >= 67.5 && v < 112.5:
                    Direction = "W";
                    break;
                case double v when v >= 112.5 && v < 157.5:
                    Direction = "NW";
                    break;
                case double v when v >= 157.5 && v < 202.5:
                    Direction = "N";
                    break;
                case double v when v >= 202.5 && v < 247.5:
                    Direction = "NE";
                    break;
                case double v when v >= 247.5 && v < 292.5:
                    Direction = "E";
                    break;
                case double v when v >= 292.5 && v < 337.5:
                    Direction = "SE";
                    break;
            }
        }

        private void Unit_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Unit.Wind))
            {
                WindSpeed.Recalculate();
                Gust.Recalculate();
                Console.WriteLine("Zmieniono wiatr- odczytano to w klasie OutdoorSensors");
            }
        }

        private void OnOutdoorWindChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WindSpeed.Value))
            {
                WindSpeeds.Enqueue(WindSpeed.Value);

                if (WindSpeeds.Count > 5)
                {
                    WindSpeeds.Dequeue();
                }
                double average = WindSpeeds.Average();

                if (average + 2 < WindSpeed.Value)
                {
                    Gust.Value = WindSpeed.Value;
                    GustCounter = 0;
                }

                else
                {
                    GustCounter++;
                }
                if (GustCounter == 5) //Jezeli nie ma podmochow od 5 odczytow to 'reset'
                {
                    Gust.DisplayName = "-";
                }
            }
        }


    }
}
