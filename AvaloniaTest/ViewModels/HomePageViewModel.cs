using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Avalonia.Media.Imaging;
using Avalonia.Controls.Presenters;
using static AvaloniaTest.ViewModels.HomePageViewModel;
using System.Security.Cryptography.X509Certificates;
using AvaloniaTest.Models;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.Controls.ApplicationLifetimes;


using System.Windows.Input;
using Avalonia.Platform;

namespace AvaloniaTest.ViewModels
{


    public partial class HomePageViewModel : ViewModelBase
    {
        private double _sliderValue;
        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                if (_sliderValue != value)
                {
                    _sliderValue = value;

                    ChangeArrow(value);
                }
            }
        }


        // OutDoorSensor sen = new OutDoorSensor();
      //TUTAJ
        //private OutDoorSensor sen = new OutDoorSensor();//.Instance;


        //WIND DIRECTION
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
        public string _direction = "N";

        //INDOOR SENSORS 
        [ObservableProperty]
        public string _indoortemperature = "-°C";

        //TEMPERATURA
        //IKONA ZMIANY TEMP WZGLEDEM WCZORAJSZEJ
        [ObservableProperty]
        public StreamGeometry _indoortemperaturechangeicon = (StreamGeometry)Application.Current.FindResource("ArrowUp");
        [ObservableProperty]
        public string _indoortodaymintemp = "-°C";
        [ObservableProperty]
        public string _indoortodaymaxtemp = "-°C";
        [ObservableProperty]
        public string _indoorysterdaytemp = "-°C";

        //GODZINA
        private DispatcherTimer timer;
        [ObservableProperty]
        private string _currentDate = DateTime.Now.ToString("dd-MM-yyyy");
        [ObservableProperty]
        public string _currenttime = DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");
        [ObservableProperty]
        private string _currentDay = char.ToUpper(DateTime.Now.ToString("dddd")[0]) + DateTime.Now.ToString("dddd").Substring(1);

        //IKONA POGODY
        [ObservableProperty]
        private Bitmap _mybitmap = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-sun-144.png")));
        private Bitmap _iconname;
        public event PropertyChangedEventHandler PropertyChanged;
        //ABY ZMIENIC IKONKE POPROSTU PRZYPISZ NOWA BITMAPE DO ICONNAME
        public Bitmap Iconname
        {
            get => _iconname;
            set
            {
                if (_iconname != value)
                {
                    _iconname = value;
                    RaisePropertyChanged(nameof(Iconname));
                }
            }
        }
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Zamiana motywu
        [RelayCommand]
        private void ThemeBtn()
        {
            App app = (App)Application.Current;
            app.ChangeTheme();
            
        
        }

      //TESTOWY PRZYCISK 
        public void ButtonClokceCommand()
        {
            Console.WriteLine("guzik");
            ChangeIcon();
        }
        [ObservableProperty]
        public string text = "puste";






        public HomePageViewModel()
        {
          //  Console.WriteLine("NOWY HomePageViewModel" + jest);
          //przypisanie wartosci poczatkowej ikony pogody
            _iconname = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-sun-144.png")));
           
           //uruchomienie timera- wykorzystany do oddczytywania godziny
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);

            //CZYTANIE DANYCH ROZPOCZYNANE JEST W MAINWINDOWSVIEW - I TRWA CZALY CZAS W TLE
            //  StartDataReading();
            //SUB DLA ZMIANY 
            
            MainWindowViewModel.CurrentPageOpened += ViewModel_Activated;
            //daje suba na zmiane strony
            //jezeli wykryto zmiane to funkcja ktora unsubuje wszystko 
        }

        private void ViewModel_Activated(object sender, string e)
        {
            if (MainWindowViewModel.CurrentPageSub == "AvaloniaTest.ViewModels.HomePageViewModel")
            {
                Console.WriteLine("------------WITAM HomePage---------------");
                timer.Start();
                StartClock();
                StartOutDoorReading();
            }
            else
            {
                Console.WriteLine("-------------------DO WIDZENIA HomePage----------------------");
                //  sen.DataUpdated -= OutDoorSensor_DataUpdated;
                //  sen.DataUpdatedTwo -= OutDoorSensor_DataUpdatedTwo;
                MainWindowViewModel.outDoorSens.IndoorTempUpdated -= OutDoorTemp_DataUpdated;
                MainWindowViewModel.CurrentPageOpened -= ViewModel_Activated;
                timer.Stop();
                StopClock();
            }
        }

    
        private void StartClock()
        {
            timer.Tick += (sender, e) =>
            {
                DateTime t = DateTime.Now;
                CurrentDate = t.ToString("dd-MM-yyyy");
                CurrentDay = char.ToUpper(t.ToString("dddd")[0]) + t.ToString("dddd").Substring(1);
                Currenttime = t.Hour.ToString("D2") + ":" + t.Minute.ToString("D2");
            };
        }
        private void StopClock()
        {

            timer.Tick -= (sender, e) =>
            {
            };
        }


        private void StartOutDoorReading()
        {
            MainWindowViewModel.outDoorSens.IndoorTempUpdated += OutDoorTemp_DataUpdated;
        }


        private void OutDoorTemp_DataUpdated(object sender, double e)
        {
            Console.WriteLine("Update temperatury");
            Indoortemperature = e.ToString().Replace(',', '.') + "°C";          
        }



        public void ChangeIcon()
        {          
            Mybitmap = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-cloud-96.png")));
            Indoortemperaturechangeicon = (StreamGeometry)Application.Current.FindResource("ArrowRepeat");
            Console.WriteLine("asdasdasd");
        }

        public void ChangeArrow(double angle)
        {            
            double radians = Math.PI * angle / 180;
            double arrowLength = 95; 

            // Obliczanie pozycji strzałki na okręgu
            Arrowx = 140 + arrowLength * Math.Sin(radians);
            Arrowy = 145 - arrowLength * Math.Cos(radians); 

            double circleAngle = angle + 180; // Kąt przeciwny do kąta strzałki
            double circleRadians = Math.PI * circleAngle / 180;
            double circleLength = 98;

            Circlex = 140 + circleLength * Math.Sin(circleRadians);
            Circley = 140 - circleLength * Math.Cos(circleRadians);
            Angle = angle-90;
            switch (angle)
            {
                case double v when (v >= 0 && v < 22.5) || (v >= 337.5 && v <= 360):
                    Direction = "E";
                    break;
                case double v when v >= 22.5 && v < 67.5:
                    Direction = "SE";
                    break;
                case double v when v >= 67.5 && v < 112.5:
                    Direction = "S";
                    break;
                case double v when v >= 112.5 && v < 157.5:
                    Direction = "SW";
                    break;
                case double v when v >= 157.5 && v < 202.5:
                    Direction = "W";
                    break;
                case double v when v >= 202.5 && v < 247.5:
                    Direction = "NW";
                    break;
                case double v when v >= 247.5 && v < 292.5:
                    Direction = "N";
                    break;
                case double v when v >= 292.5 && v < 337.5:
                    Direction = "NE";
                    break;
            }
        }

   
    }
}