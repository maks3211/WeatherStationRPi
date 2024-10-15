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
using AvaloniaTest.Helpers;

using System.Windows.Input;
using Avalonia.Platform;
using System.Diagnostics.Contracts;
using AvaloniaTest.Models.WeatherForecast;
using AvaloniaTest.Models.AddressSearch;
using AvaloniaTest.Messages;
using CommunityToolkit.Mvvm.Messaging;
using AvaloniaTest.Models.ObservablesProperties;
using ScottPlot.Colormaps;
using static AvaloniaTest.Models.WeatherForecast.WeatherForecast;
using AvaloniaTest.Services;
//using AvaloniaTest.ViewModels.WeatherForecastTemplate;


namespace AvaloniaTest.ViewModels
{

    /// <summary>
    /// This class provides properties and commands for the HomePage view. 
    /// It manages the state and behavior of various UI elements on the HomePage.
    /// </summary>
    public partial class HomePageViewModel : ViewModelBase
    {


        private double _sliderValue;
        [ObservableProperty]
        public WeatherForecastController _weatherController;

        AddressSearchController AddressSearchController = new AddressSearchController();
        //Hourly Forecast 
        [ObservableProperty]
        private HourlyForecastTemplate? _selectedHorulyForecast;

      
      





       
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
        public string _windSpeed = Units.GetInstance().CalculatWind(MainWindowViewModel.inDoorSens.temperature).ToString().Replace(',', '.');
        [ObservableProperty]
        public int _windGust = 0;
        [ObservableProperty]
        public string _windUnit = Units.GetInstance().GetWindUnit();

        private string[] _yellowcolorslist = { "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00" };
        private string[] _greencolorslist = { "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30", "#355132", "#4dba50" };

        public ObservableCollection<string> Indoorpreasurecolors { get; } = new()
        {
            "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00"
        };
        public ObservableCollection<string> Indooriluminancecolors { get; } = new()
        {
             "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00"
        };

        public ObservableCollection<string> Outdoorpreasurecolors { get; } = new()
        {
            "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50"
        };
        public ObservableCollection<string> Outdooriluminancecolors { get; } = new()
        {
               "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30","#355132", "#4dba50"
        };



        [ObservableProperty]
        public double _outdoorhumiditycircle;

        [ObservableProperty]
        public int _outdoorcocircle;

        [ObservableProperty]
        public int _outdoornh3circle;

        [ObservableProperty]
        public int _outdoorno2circle;







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
        public string _indoorpreasure = "sdsd";

        [ObservableProperty]
        public int _indoorluminance;

        [ObservableProperty]
        public int _indooraltitude;

        [ObservableProperty]
        public int _indoorco;

        [ObservableProperty]
        public int _indoorcocircle = 5;

        //TIME
     //   private DispatcherTimer timer;

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



        [ObservableProperty]
        private double _weatherWidth = 243.5;
        [ObservableProperty]
        private double _weatherHeight = 293.0;   
        [ObservableProperty]
        private double _weatherLeftPos = 10;
        [ObservableProperty]
        private double _weatherTopPos = 166.5;

        private string lastCityImput = "";
        private string _cityInput;
        public string CityInput
        {
            get => _cityInput;
            set
            {
                if (_cityInput != value)
                {
                    _cityInput = value;
                    OnPropertyChanged(nameof(CityInput));
                    UpdateCitySuggestions();  // Wywołanie metody przy zmianie tekstu
                }
            }
        }

        public async Task UpdateCitySuggestions()
        {


 
            if (CityInput.Length < 3)
            {
                return;
            }
            else
            {
             Console.WriteLine("OK wieksze od 3 ");
            }
            if (CityInput.Length - lastCityImput.Length < 5)
            {
                Console.WriteLine("Za mała różnica");
                if (CityInput.Length - lastCityImput.Length < -5)
                {
                    Console.WriteLine("ZERO");
                    lastCityImput = CityInput;
                }
                else
                {
                    return;
                }
            }
            else
            {
                lastCityImput = CityInput;
            }
           
          
         
            lastCityImput = CityInput;

            Console.WriteLine("SZUKANIE miasta w homepagevm");
             var resuts = await AddressSearchController.Search(CityInput);
               if (resuts != null)
               {
                   CitySuggestions.Clear();

                   foreach (var a in resuts)
                   {
                       CitySuggestions.Add(a.display_name);
                   }
               }
               else
               {
                   Console.WriteLine("NULLLL");
               }


        }

        private ObservableCollection<string> _citySuggestions;
        public ObservableCollection<string> CitySuggestions
        {
            get { return _citySuggestions; }
            set
            {
                _citySuggestions = value;
                OnPropertyChanged(nameof(CitySuggestions));
            }
        }

       
       

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Command handler for change theme button.
        /// </summary>
   /*     [RelayCommand]        RACZEJ NIE POTRZEBNE ~!!!! --       BEZ TEGO ZMIANA MOTYWU TEZ DZIALA
       private void ThemeBtn()
        {
          
            App app = (App)Application.Current;
            app.ChangeTheme();

        }*/

        /// <summary>
        /// Initializes a new instance of the HomePageViewModel class.
        /// </summary>
        /// 


        [ObservableProperty]
        TimeProperties timeProp;

        [ObservableProperty]
        private OutdoorSensors _outdoorSensorsProp;

        [ObservableProperty]
        private IndoorSensors _indoorSensorsProp;
        public HomePageViewModel(TimeProperties timeProperties, OutdoorSensors outdoorSensors, WeatherForecastController weatherController, IndoorSensors indoorSensors)
        {
            //HourlyForecastItems = WeatherController.HourlyForecastItems;
            OutdoorSensorsProp = outdoorSensors;
            IndoorSensorsProp = indoorSensors;

            TimeProp = timeProperties;
            WeatherController = weatherController;
          

            // WeatherController.GetWeather();
            _iconname = new Bitmap(AssetLoader.Open(new Uri("avares://AvaloniaTest/Assets/Images/icons8-sun-144.png")));
       
            //    timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMinutes(1);

            // MainWindowViewModel.CurrentPageOpened += ViewModel_Activated;
            //dodane

            //timer.Start();
           // StartClock();
            StartOutDoorReading();

            //InitializeForecast(); TU BYLO !!!

            CitySuggestions = new ObservableCollection<string>();



            SubOutdoorPressureChange();
            SubOutdoorHumChanges();
            SubOutdoorIlluChanges();
            SubOutdoorCOChanges();
            SubOutdoorNH3Changes();
            SubOutdoorNO2Changes();


            WeakReferenceMessenger.Default.Register<ViewActivatedMessages>(this, (r, m) =>
            {

                if (m.Value == GetType().FullName)
                {
                    Console.WriteLine("Otwarto strone główną");
                }
                else 
                {
                    Console.WriteLine("Zamknięto stornę główną");
                }
            });

           

          
        }
        private void UpdateZegar()
        {
           
            //  DateTime t = DateTime.Now;
            DateTime t = Clock.Instance.CurrentTime;
           
            CurrentDate = t.ToString("dd-MM-yyyy");
            CurrentDay =  char.ToUpper(t.ToString("dddd")[0]) + t.ToString("dddd").Substring(1);
            Currenttime = t.Hour.ToString("D2") + ":" + t.Minute.ToString("D2");
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
           //     timer.Start();
                //StartClock();
                StartOutDoorReading();
                // InitializeForecast();          TU BYŁO      
            }
            else
            {
               UnsubscribeAll();
            //   timer.Stop();
             //  StopClock();
            }
        }

        private void UnsubscribeAll()
        {

           
         

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
        }



      /*  private void StartClock()
        {
            Console.WriteLine("ZEGAR START");
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
            Console.WriteLine("ZEGAR STOP");
            timer.Tick -= (sender, e) =>
            {
            };
        }
*/

        private void StartOutDoorReading()
        {
           
           // MainWindowViewModel.outDoorSens.IndoorTempUpdated += OutDoorTemp_DataUpdated;
           //OUTDOOR SENSORS

     
         
         

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
        //  private void OutDoorTemp_DataUpdated(object sender, double e)
        //  {
        //      Outdoortemperature = Units.GetInstance().CalculateTemp(e).ToString().Replace(',', '.') + Units.GetInstance().GetTempUnit();
        // }


        [RelayCommand]
        public void OnRefreshForecastClicked()
        {
            WeatherController.GetAllData();
        }
        private void SubOutdoorPressureChange()
        {
            if (OutdoorSensorsProp.OutdoorPressure != null)
            {
                OutdoorSensorsProp.OutdoorPressure.PropertyChanged += OutDoorPres_DataUpdated;  
            }
        }
        /// <summary>
        /// Update outdoor pressur value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutDoorPres_DataUpdated(object sender, PropertyChangedEventArgs e)
        {
            
            ChangeVerticalBarColor(OutdoorSensorsProp.OutdoorPressure.Value, 1054, 960, _greencolorslist, Outdoorpreasurecolors);
        }

   


        private void SubOutdoorHumChanges()
        {
            if (OutdoorSensorsProp.OutdoorHumidity != null)
            {
                OutdoorSensorsProp.OutdoorHumidity.PropertyChanged += OnOutdoorHumidityChanged;
            }
        }
        private void OnOutdoorHumidityChanged(object sender, PropertyChangedEventArgs e)
        {
                Outdoorhumiditycircle = 60 - OutdoorSensorsProp.OutdoorHumidity.Value / 100 * 60;  
        }

  

        private void SubOutdoorIlluChanges()
        {
            if (OutdoorSensorsProp.OutdoorIlluminance != null)
            {
                OutdoorSensorsProp.OutdoorIlluminance.PropertyChanged += OnOutdoorIlluminanceChanged;
            }
        }
        private void OnOutdoorIlluminanceChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangeVerticalBarColor(OutdoorSensorsProp.OutdoorIlluminance.Value, 2000, 0, _greencolorslist, Outdooriluminancecolors);
        }




    



        private void SubOutdoorCOChanges()
        {
            if (OutdoorSensorsProp.OutdoorCO != null)
            {
                OutdoorSensorsProp.OutdoorCO.PropertyChanged += OnOutdoorCOChanged;
            }
        }
        private void OnOutdoorCOChanged(object sender, PropertyChangedEventArgs e)
        {
            Outdoorcocircle = MoveCricle(5, 0, 250, OutdoorSensorsProp.OutdoorCO.Value);
        }


 

        private void SubOutdoorNH3Changes()
        {
            if (OutdoorSensorsProp.OutdoorNH3 != null)
            {
                OutdoorSensorsProp.OutdoorNH3.PropertyChanged += OnOutdoorNH3Changed;
            }
        }
        private void OnOutdoorNH3Changed(object sender, PropertyChangedEventArgs e)
        {
            Outdoornh3circle = MoveCricle(15, 0, 180, OutdoorSensorsProp.OutdoorNH3.Value);
        }

        private void SubOutdoorNO2Changes()
        {
            if (OutdoorSensorsProp.OutdoorNO2 != null)
            {
                OutdoorSensorsProp.OutdoorNO2.PropertyChanged += OnOutdoorNO2Changed;
            }
        }
        private void OnOutdoorNO2Changed(object sender, PropertyChangedEventArgs e)
        {
            Outdoorno2circle = MoveCricle(15, 0, 10, OutdoorSensorsProp.OutdoorNO2.Value);
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

    public abstract class ForecastBase
    {
        public Bitmap Icon { get; }
        public bool IsRain { get; }
        public int PrecipitationProb { get; }
        public double RainVol { get; }

        protected ForecastBase(Bitmap icon, bool isRain, int precipitationProb, double rainVol)
        {
            Icon = icon;
            IsRain = isRain;
            PrecipitationProb = precipitationProb;
            RainVol = rainVol;
        }
    }

  

    public class DailyForecastTemplate : ForecastBase
    {
        public string Day { get; }
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }

        public DailyForecastTemplate(
            string day,
            Bitmap icon,
            int minTemp,
            int maxTemp,
            bool isRain,
            int precipitationProb,
            double rainVol)
            : base(icon, isRain, precipitationProb, rainVol)
        {
            Day = day;
            MinTemp = minTemp;
            MaxTemp = maxTemp;
        }
    }
}