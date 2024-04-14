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
using System;

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

        static bool jest = false;
        double a = 0;
        double step = 0.1;

        // OutDoorSensor sen = new OutDoorSensor();
        private OutDoorSensor sen = new OutDoorSensor();//.Instance;
        public ObservableCollection<ObservableCollection<BorderModel>> BorderItemsCollection { get; } = new ObservableCollection<ObservableCollection<BorderModel>>();
        public ObservableCollection<BorderModel> OneColumn { get; } = new ObservableCollection<BorderModel>();
        public ObservableCollection<BorderModel> TwoColumn { get; } = new ObservableCollection<BorderModel>();
        public ObservableCollection<BorderModel> ThreeColumn { get; } = new ObservableCollection<BorderModel>();
        public ObservableCollection<BorderModel> FourColumn { get; } = new ObservableCollection<BorderModel>();


        [ObservableProperty]
        public double _arrowx = 150;
        [ObservableProperty]
        public double _arrowy = 150;
        [ObservableProperty]
        public double _angle = 0;
        [ObservableProperty]
        public string _direction = "N";

        //INDOOR SENSORS 
        [ObservableProperty]
        public string _indoortemperature = "-°C";


        private DispatcherTimer timer;

        [ObservableProperty]
        private string _currentDate = DateTime.Now.ToString("dd-MM-yyyy");

        [ObservableProperty]
        public string _currenttime = DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");

        

        [ObservableProperty]
        private string _currentDay = char.ToUpper(DateTime.Now.ToString("dddd")[0]) + DateTime.Now.ToString("dddd").Substring(1);

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


        [RelayCommand]
        private void ThemeBtn()
        {
            App app = (App)Application.Current;
            app.ChangeTheme();
            
        
        }

      
        public void ButtonClokceCommand()
        {
            Console.WriteLine("guzik");
            ChangeIcon();
        }
        [ObservableProperty]
        public string text = "puste";






        public HomePageViewModel()
        {
            Console.WriteLine("NOWY HomePageViewModel" + jest);
            _iconname = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-sun-144.png")));
           
            for (int i = 0; i < 4; i++)
            {
                BorderItemsCollection.Add(new ObservableCollection<BorderModel>());
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            StartDataReading();
            MainWindowViewModel.outDoorSens.DataUpdated += OutDoorSensor_DataUpdated;
            MainWindowViewModel.outDoorSens.DataUpdatedTwo += OutDoorSensor_DataUpdatedTwo;
               
                    MainWindowViewModel.CurrentPageOpened += ViewModel_Activated;
            //daje suba na zmiane strony
            //jezeli wykryto zmiane to funkcja ktora unsubuje wszystko 
        }

        private void ViewModel_Activated(object sender, string e)
        {
            if (MainWindowViewModel.CurrentPageSub == "AvaloniaTest.ViewModels.HomePageViewModel")
            {
                Console.WriteLine("------------witam---------------");
                timer.Start();
                StartClock();
            }
            else
            {
                Console.WriteLine("-------------------DO WIDZENIA----------------------");
                //  sen.DataUpdated -= OutDoorSensor_DataUpdated;
                //  sen.DataUpdatedTwo -= OutDoorSensor_DataUpdatedTwo;
                MainWindowViewModel.outDoorSens.DataUpdated -= OutDoorSensor_DataUpdated;
                MainWindowViewModel.outDoorSens.DataUpdatedTwo -= OutDoorSensor_DataUpdatedTwo;
                sen.StopEvery();
                MainWindowViewModel.CurrentPageOpened -= ViewModel_Activated;
                timer.Stop();
                StopClock();
            }
        }

    
        private void StartClock()
        {
            timer.Tick += (sender, e) =>
            {
                Console.WriteLine("czytanie godziny");
                DateTime t = DateTime.Now;
                CurrentDate = t.ToString("dd-MM-yyyy");
                CurrentDay = char.ToUpper(t.ToString("dddd")[0]) + t.ToString("dddd").Substring(1);
                Currenttime = t.Hour.ToString("D2") + ":" + t.Minute.ToString("D2");
                Console.WriteLine(Currenttime);
            };
        }
        private void StopClock()
        {

            timer.Tick -= (sender, e) =>
            {
            };
        }


        private void OutDoorSensor_DataUpdated(object sender, double e)
        {
          
            Indoortemperature = e.ToString().Replace(',', '.') + "°C";
            Console.WriteLine("Czytanie tempe");

        }
        private void OutDoorSensor_DataUpdatedTwo(object sender, double e)
        {
            BorderItemsCollection[0][0].StackPanels[1].Text = e.ToString() + "%";
        }


        public void ChangeIcon()
        {
            // _iconname.Dispose();
            //  _iconname = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-cloud-96.png")));
            Mybitmap = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-cloud-96.png")));

            Console.WriteLine("asdasdasd");
        }

        public void ChangeArrow(double angle)
        {
            double kat = (2 * Math.PI * angle) / 360;
            Arrowx = 140 + 95 * Math.Cos(kat);
            Arrowy = 145 + 95 * Math.Sin(kat);
            //a += step;
            //PROMIEN BYL 110
          
            double angleBetweenArrowAndCenter = Math.Atan2(Arrowy - 150, Arrowx - 150);
            double rotationAngle = angleBetweenArrowAndCenter - Math.PI;
            Console.WriteLine("Ustawiony kat: " + angle);
            //  Angle += 5.732 * 0.95;
            Angle = angle;
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

        public async Task StartDataReading()
        {
                jest = true;
                Task task1 = sen.RunReadData();
                //Task task2 = sen.RunReadDataTwo();
                await Task.WhenAll(task1);         
        }

        public class BorderModel
        {
            public ObservableCollection<StackPanelModel> StackPanels { get; set; }
            public int Height { get; set; }
        }
   
        

        public partial class StackPanelModel : ObservableObject
        {
          public StackPanelModel(string text, int height, Thickness margin, string iconKey) 
          { 
              Text = text;
            
              ElementHeight = height;
              Margin = margin;
              Application.Current!.TryFindResource(iconKey, out var res);
             MyIcon = (StreamGeometry)res!;
          }

            [ObservableProperty]
            private string text;

            public string ButtonContent { get; set; }
            public int ElementHeight { get; set; }
            public Thickness Margin { get; set; }
        
            public StreamGeometry MyIcon { get; set; }
        }

        public int getHeight(int first, int second)
        {
            int result = 0;
            foreach (var el in BorderItemsCollection[0][0].StackPanels)
            {
                
                Console.WriteLine(el.ElementHeight);
                result += el.ElementHeight;
            }
            return result;
        }


    
    }



}