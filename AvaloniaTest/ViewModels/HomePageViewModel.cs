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
using System.Diagnostics.Contracts;



namespace AvaloniaTest.ViewModels
{

    /// <summary>
    /// This class provides properties and commands for the HomePage view. 
    /// It manages the state and behavior of various UI elements on the HomePage.
    /// </summary>
    public partial class HomePageViewModel : ViewModelBase
    {

        private double _sliderValue;

        /// <summary>
        /// Gets or sets the value of the slider.
        /// </summary>
        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                if (_sliderValue != value)
                {
                    _sliderValue = value;

                    ChangeWindArrow(value);
                }
            }
        }


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
        public string _windDirection = "N";
        [ObservableProperty]
        public string _windSpeed = Units.GetInstance().CalculatWind(MainWindowViewModel.inDoorSens.temperature).ToString().Replace(',','.');
        [ObservableProperty]
        public int _windGust = 0;
        [ObservableProperty]
        public string _windUnit = Units.GetInstance().GetWindUnit();

        private string[] _yellowcolorslist = { "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00" };
        private string[] _greencolorslist = {  "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50" };

        public ObservableCollection<string> Indoorpreasurecolors { get; } = new()
        {
            "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00"
        };
        public ObservableCollection<string> Outdoorpreasurecolors { get; } = new()
        {
            "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50"
        };
        public ObservableCollection<string> Outdooriluminancecolors { get; } = new()
        {
             "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00"
        };
        public ObservableCollection<string> Indooriluminancecolors { get; } = new()
        {
            "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50"
        };
 

        //OUTDOOR SENSORS
        [ObservableProperty]
        public string _outdoortemperature = Units.GetInstance().CalculateTemp(MainWindowViewModel.mqqt.OutDoorTemp).ToString().Replace(',', '.').Replace("-999", "-") + Units.GetInstance().GetTempUnit();

        [ObservableProperty]
        public double _outdoorpreasure;

        [ObservableProperty]
        public string _outdoorhumidity = MainWindowViewModel.inDoorSens.humidity.ToString();

        [ObservableProperty]
        public double _outdoorhumiditycircle = 60 - MainWindowViewModel.inDoorSens.humidity / 100 * 60;

        [ObservableProperty]
        public int _outdoorluminance;

        [ObservableProperty]
        public int _outdooraltitude;

        [ObservableProperty]
        public int _outdoorco;

        [ObservableProperty]
        public int _outdoorcocircle = 5;

        [ObservableProperty]
        public int _outdoornh3;

        [ObservableProperty]
        public int _outdoornh3circle = 5;

        [ObservableProperty]
        public int _outdoorno2;

        [ObservableProperty]
        public int _outdoorno2circle = 5;



        //INDOOR SENSORS
        [ObservableProperty]
        public string _indoortemperature = Units.GetInstance().CalculateTemp(MainWindowViewModel.inDoorSens.temperature).ToString().Replace(',', '.') + Units.GetInstance().GetTempUnit();
        
        [ObservableProperty]
        public StreamGeometry _indoortemperaturechangeicon = (StreamGeometry)Application.Current.FindResource("ArrowUp");
        
        [ObservableProperty]
        public string _indoortodaymintemp = "-°C";
        
        [ObservableProperty]
        public string _indoortodaymaxtemp = "-°C";
        
        [ObservableProperty]
        public string _indoorysterdaytemp = "-°C";

        [ObservableProperty]
        public string _indoorhumidity = MainWindowViewModel.inDoorSens.humidity.ToString();
        
        [ObservableProperty]
        public double _indoorhumiditycircle = 54 - MainWindowViewModel.inDoorSens.humidity / 100 * 54;

        [ObservableProperty]
        public string _indoorpreasure ="sdsd";

        [ObservableProperty]
        public int _indoorluminance;

        [ObservableProperty]
        public int _indooraltitude;

        [ObservableProperty]
        public int _indoorco;
        
        [ObservableProperty]
        public int _indoorcocircle = 5;

        //TIME
        private DispatcherTimer timer;
        
        [ObservableProperty]
        private string _currentDate = DateTime.Now.ToString("dd-MM-yyyy");
        
        [ObservableProperty]
        public string _currenttime = DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2");
        
        [ObservableProperty]
        private string _currentDay = char.ToUpper(DateTime.Now.ToString("dddd")[0]) + DateTime.Now.ToString("dddd").Substring(1);

        // WEATHER ICON
        [ObservableProperty]
        private Bitmap _mybitmap = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-sun-144.png")));
        
        private Bitmap _iconname;
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Gets or sets the weather icon.
        /// </summary>
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

        /// <summary>
        /// Command handler for change theme button.
        /// </summary>
        [RelayCommand]
        private void ThemeBtn()
        {
            App app = (App)Application.Current;
            app.ChangeTheme();
                
        }

        /// <summary>
        /// Initializes a new instance of the HomePageViewModel class.
        /// </summary>
        public HomePageViewModel()
        {
            _iconname = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-sun-144.png")));
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            MainWindowViewModel.CurrentPageOpened += ViewModel_Activated;
        }

        /// <summary>
        /// Handles the activation of the view model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_Activated(object sender, string e)
        {
            if (MainWindowViewModel.CurrentPageSub == "AvaloniaTest.ViewModels.HomePageViewModel")
            {
                timer.Start();
                StartClock();
                StartOutDoorReading();
            }
            else
            {
                MainWindowViewModel.mqqt.OutdoorTempUpdated -= OutDoorTemp_DataUpdated;
                MainWindowViewModel.mqqt.OutdoorPresUpdated -= OutDoorPres_DataUpdated;
                MainWindowViewModel.mqqt.OutdoorHumiUpdated -= OutDoorHum_DataUpdated;
                MainWindowViewModel.mqqt.OutdoorLumiUpdated -= OutDoorLumi_DataUpdated;
                MainWindowViewModel.mqqt.OutdoorCOUpdated -= OutDoorCo_DataUpdated;
                MainWindowViewModel.mqqt.OutdoorNH3Updated -= OutDoornh3_DataUpdated;
                MainWindowViewModel.mqqt.OutdoorNO2Updated -= OutDoorno2_DataUpdated;

                MainWindowViewModel.inDoorSens.IndoorTempUpdated -= InDoorTemp_DataUpdated;
                MainWindowViewModel.inDoorSens.IndoorHumUpdated -= InDoorHum_DataUpdated;
                MainWindowViewModel.inDoorSens.WindDirectionUpdated -= WindDirection_DataUpdated;
                MainWindowViewModel.inDoorSens.WindSpeedUpdated -= WindSpeed_DataUpdated;
                MainWindowViewModel.inDoorSens.WindGustUpdated -= WindGust_DataUpdated;
                MainWindowViewModel.inDoorSens.IndoorPreasureUpdated -= IndoorPres_DataUpdated;
                MainWindowViewModel.inDoorSens.IndoorLumiUpdated -= InDoorLumi_DataUpdated;
                MainWindowViewModel.inDoorSens.IndoorAltiUpdated -= InDoorAlti_DataUpdated;
                MainWindowViewModel.inDoorSens.IndoorCOUpdated -= InDoorCo_DataUpdated;
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
           
           // MainWindowViewModel.outDoorSens.IndoorTempUpdated += OutDoorTemp_DataUpdated;
           //OUTDOOR SENSORS
            MainWindowViewModel.mqqt.OutdoorTempUpdated += OutDoorTemp_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorPresUpdated += OutDoorPres_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorHumiUpdated += OutDoorHum_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorLumiUpdated += OutDoorLumi_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorAltiUpdated += OutDoorAlti_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorCOUpdated += OutDoorCo_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorNH3Updated += OutDoornh3_DataUpdated;
            MainWindowViewModel.mqqt.OutdoorNO2Updated += OutDoorno2_DataUpdated;

            MainWindowViewModel.inDoorSens.IndoorTempUpdated += InDoorTemp_DataUpdated;
            MainWindowViewModel.inDoorSens.IndoorHumUpdated += InDoorHum_DataUpdated;
            MainWindowViewModel.inDoorSens.WindDirectionUpdated += WindDirection_DataUpdated;
            MainWindowViewModel.inDoorSens.WindSpeedUpdated += WindSpeed_DataUpdated;
            MainWindowViewModel.inDoorSens.WindGustUpdated += WindGust_DataUpdated;
            MainWindowViewModel.inDoorSens.IndoorPreasureUpdated += IndoorPres_DataUpdated;
            MainWindowViewModel.inDoorSens.IndoorLumiUpdated += InDoorLumi_DataUpdated;
            MainWindowViewModel.inDoorSens.IndoorAltiUpdated += InDoorAlti_DataUpdated;
            MainWindowViewModel.inDoorSens.IndoorCOUpdated += InDoorCo_DataUpdated;
        }

        /// <summary>
        /// Update outdoor temperature value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorTemp_DataUpdated(object sender, double e)
        {
            Outdoortemperature = Units.GetInstance().CalculateTemp(e).ToString().Replace(',', '.') + Units.GetInstance().GetTempUnit();
        }

        /// <summary>
        /// Update outdoor pressur value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorPres_DataUpdated(object sender, double e)
        {
            Outdoorpreasure = (int)e;
            ChangeVerticalBarColor(e, 1054, 960, _greencolorslist, Outdoorpreasurecolors);
        }

        /// <summary>
        /// Update outdoor humidity value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorHum_DataUpdated(object sender, double e)
        {
            // Console.WriteLine("tUpdate wilgotnosci");

            Outdoorhumiditycircle = 60 - e / 100 * 60;
            string[] parts1 = e.ToString().Split('.', ',');
            Outdoorhumidity = parts1[0];
        }

        /// <summary>
        /// Update outdoor luminance value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorLumi_DataUpdated(object sender, double e)
        {

            Outdoorluminance = (int)e;
            ChangeVerticalBarColor(e, 2000, 0, _yellowcolorslist, Outdooriluminancecolors);
        }

        /// <summary>
        /// Update outdoor altitude value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorAlti_DataUpdated(object sender, int e)
        {
            Outdooraltitude = e;
        }

        /// <summary>
        /// Update outdoor Co value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorCo_DataUpdated(object sender, double e)
        {

            Outdoorco = (int)e;
            Outdoorcocircle = MoveCricle(5, 0, 250, e);
        }

        /// <summary>
        /// Update outdoor NH3 value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoornh3_DataUpdated(object sender, double e)
        {

            Outdoornh3 = (int)e;
            Outdoornh3circle = MoveCricle(15, 0, 180, e);
        }

        /// <summary>
        /// Update outdoor NO2 value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorno2_DataUpdated(object sender, double e)
        {

            Outdoorno2 = (int)e;
            Outdoorno2circle = MoveCricle(15, 0, 10, e);
        }

        /// <summary>
        /// Update indoor temperature value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InDoorTemp_DataUpdated(object sender, double e)
        {
            Indoortemperature = Units.GetInstance().CalculateTemp(e).ToString().Replace(',', '.') + Units.GetInstance().GetTempUnit();
        }

        /// <summary>
        /// Update indoor luminance value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InDoorLumi_DataUpdated(object sender, double e)
        {
            Indoorluminance = (int)e;
            ChangeVerticalBarColor(e, 2000, 0, _yellowcolorslist, Indooriluminancecolors);
        }

        /// <summary>
        /// Update indoor altitude value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InDoorAlti_DataUpdated(object sender, int e)
        {
            Indooraltitude = e;
        }

        /// <summary>
        /// Update indoor humidity value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InDoorHum_DataUpdated(object sender, double e)
        {
            // Console.WriteLine("tUpdate wilgotnosci");

            Indoorhumiditycircle = 54 - e / 100 * 54;
            string[] parts1 = e.ToString().Split('.', ',');
            Indoorhumidity = parts1[0];
        }

        /// <summary>
        /// Update wind direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindDirection_DataUpdated(object sender, double e)
        {
            Angle = e;
            ChangeWindArrow(e);
        }

        /// <summary>
        /// Update wind speed value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindSpeed_DataUpdated(object sender, int e)
        {
            WindSpeed = Units.GetInstance().CalculatWind(e).ToString().Replace(',', '.');
        }

        /// <summary>
        /// Update wind gust value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindGust_DataUpdated(object sender, int e)
        {
            WindGust = e;
        }

        /// <summary>
        /// Update indoor pressure value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IndoorPres_DataUpdated(object sender, int e)
        {
            Indoorpreasure = e.ToString();
            ChangeVerticalBarColor(e, 1054, 960, _yellowcolorslist, Indoorpreasurecolors);

        }

        /// <summary>
        /// Update indoor CO value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InDoorCo_DataUpdated(object sender, double e)
        {
            Indoorco = (int)e;
            Indoorcocircle = MoveCricle(5, 0, 250, e);
        }
        
        /// <summary>
        /// Sets icon image from local png file
        /// </summary>
        public void ChangeIcon()
        {          
            Mybitmap = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-cloud-96.png")));
            Indoortemperaturechangeicon = (StreamGeometry)Application.Current.FindResource("ArrowRepeat");
         
        }

        private int MoveCricle(int start, int minVal, int maxVal, double value)
        {
            double range = maxVal - minVal;
            double width = start + 145;
            return (int)(width * value / range);
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
            Circley = 140 - circleLength * Math.Cos(circleRadians)-50;
            Angle = angle-90;
            switch (angle)
            {
                case double v when (v >= 0 && v < 22.5) || (v >= 337.5 && v <= 360):
                    WindDirection = "S";
                    break;
                case double v when v >= 22.5 && v < 67.5:
                    WindDirection = "SW";
                    break;
                case double v when v >= 67.5 && v < 112.5:
                    WindDirection = "W";
                    break;
                case double v when v >= 112.5 && v < 157.5:
                    WindDirection = "NW";
                    break;
                case double v when v >= 157.5 && v < 202.5:
                    WindDirection = "N";
                    break;
                case double v when v >= 202.5 && v < 247.5:
                    WindDirection = "NE";
                    break;
                case double v when v >= 247.5 && v < 292.5:
                    WindDirection = "E";
                    break;
                case double v when v >= 292.5 && v < 337.5:
                    WindDirection = "SE";
                    break;
            }
        }

        /// <summary>
        /// Update Bar colors to be displayed
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="max">Max bar value.</param>
        /// <param name="min">Min bar value.</param>
        /// <param name="colors">Arrays of colors to be displayed</param>
        /// <param name="axamlColors">Colors to be set</param>
        public void ChangeVerticalBarColor(double value, double max, double min, string[] colors, ObservableCollection<string> axamlColors)
        {
            int tmp = 0;
            int range = (int)((max - min) / 7);
            if (value <= min + range)   //956
            {
                for (int i = 0; i < 6; i++)
                {
                    axamlColors[i] = colors[i];
                }
            }
            else if (value > min && value <= min + 2*range)
            {
                for (int i = 0; i < 5; i++)
                {
                    // tmp = i + 1;
                    axamlColors[i] = colors[i];
                }
                axamlColors[5] = colors[6];
                axamlColors[6] = colors[6];
            }
            else if (value > min + 2*range && value <= min + 3*range)
            {
                for (int i = 0; i < 4; i++)
                {
                    axamlColors[i] = colors[i + 1];
                }
                axamlColors[4] = colors[6];
                axamlColors[5] = colors[6];
                axamlColors[6] = colors[6];
            }
            else if (value > min + 3 * range && value <= min + 4 * range)
            {
                for (int i = 0; i < 3; i++)
                {
                    axamlColors[i] = colors[i + 2];
                }
                axamlColors[3] = colors[6];
                axamlColors[4] = colors[6];
                axamlColors[5] = colors[6];
                axamlColors[6] = colors[6];
            }
            else if (value > min + 4 * range && value <= min + 5 * range)
            {
                for (int i = 0; i < 2; i++)
                {
                    axamlColors[i] = colors[i + 3];
                }
                axamlColors[2] = colors[6];
                axamlColors[3] = colors[6];
                axamlColors[4] = colors[6];
                axamlColors[5] = colors[6];
                axamlColors[6] = colors[6];
            }
            else if (value > min + 5 * range && value <= min + 6 * range)  
            {
                axamlColors[0] = colors[4];
                axamlColors[1] = colors[6];
                axamlColors[2] = colors[6];
                axamlColors[3] = colors[6];
                axamlColors[4] = colors[6];
                axamlColors[5] = colors[6];
                axamlColors[6] = colors[6];
            }
            else
            {
                axamlColors[0] = colors[6];
                axamlColors[1] = colors[6];
                axamlColors[2] = colors[6];
                axamlColors[3] = colors[6];
                axamlColors[4] = colors[6];
                axamlColors[5] = colors[6];
                axamlColors[6] = colors[6];
            }


        }
        /// <summary>
        /// Update color of pressure bar
        /// </summary>
        /// <param name="preasure">Current pressure.</param>
        public void ChangeIndoorPreasureBar(double preasure)
        {
            int tmp;
            if (preasure <= 974)
            { 
                for (int i = 0; i < 6; i++)
                {
                    Indoorpreasurecolors[i] = _yellowcolorslist[i];
                }
            }
            else if (preasure > 974 && preasure <= 988)
            {
                for (int i = 0; i < 5; i++)
                {
                   // tmp = i + 1;
                    Indoorpreasurecolors[i] = _yellowcolorslist[i];
                }
                Indoorpreasurecolors[5] = _yellowcolorslist[6];
                Indoorpreasurecolors[6] = _yellowcolorslist[6];
            }
            else if (preasure > 988 && preasure <= 1002)
            {
                for (int i = 0; i < 4; i++)
                {               
                    Indoorpreasurecolors[i] = _yellowcolorslist[i+1];
                }
                Indoorpreasurecolors[4] = _yellowcolorslist[6];
                Indoorpreasurecolors[5] = _yellowcolorslist[6];
                Indoorpreasurecolors[6] = _yellowcolorslist[6];
            }
            else if (preasure > 1002 && preasure <= 1016)
            {
                for (int i = 0; i < 3; i++)
                {
                    Indoorpreasurecolors[i] = _yellowcolorslist[i + 2];
                }
                Indoorpreasurecolors[3] = _yellowcolorslist[6];
                Indoorpreasurecolors[4] = _yellowcolorslist[6];
                Indoorpreasurecolors[5] = _yellowcolorslist[6];
                Indoorpreasurecolors[6] = _yellowcolorslist[6];
            }
            else if (preasure > 1016 && preasure <= 1030)
            {
                for (int i = 0; i < 2; i++)
                {
                    Indoorpreasurecolors[i] = _yellowcolorslist[i + 3];
                }
                Indoorpreasurecolors[2] = _yellowcolorslist[6];
                Indoorpreasurecolors[3] = _yellowcolorslist[6];
                Indoorpreasurecolors[4] = _yellowcolorslist[6];
                Indoorpreasurecolors[5] = _yellowcolorslist[6];
                Indoorpreasurecolors[6] = _yellowcolorslist[6];
            }
            else if (preasure > 1030 && preasure <= 1044)
            {
                Indoorpreasurecolors[0] = _yellowcolorslist[4];
                Indoorpreasurecolors[1] = _yellowcolorslist[6];
                Indoorpreasurecolors[2] = _yellowcolorslist[6];
                Indoorpreasurecolors[3] = _yellowcolorslist[6];
                Indoorpreasurecolors[4] = _yellowcolorslist[6];
                Indoorpreasurecolors[5] = _yellowcolorslist[6];
                Indoorpreasurecolors[6] = _yellowcolorslist[6];
            }
            else 
            {
                Indoorpreasurecolors[0] = _yellowcolorslist[6];
                Indoorpreasurecolors[1] = _yellowcolorslist[6];
                Indoorpreasurecolors[2] = _yellowcolorslist[6];
                Indoorpreasurecolors[3] = _yellowcolorslist[6];
                Indoorpreasurecolors[4] = _yellowcolorslist[6];
                Indoorpreasurecolors[5] = _yellowcolorslist[6];
                Indoorpreasurecolors[6] = _yellowcolorslist[6];
            }
        }

   
    }
}