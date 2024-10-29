using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;
using AvaloniaTest.Helpers;
using AvaloniaTest.Messages;
using AvaloniaTest.Models;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Models.WeatherForecast;
using AvaloniaTest.Services;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.Services.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AvaloniaTest.ViewModels
{
    /// <summary>
    /// View model class for the main window.
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {

        [ObservableProperty]
        public TimeProperties _timeProp = new TimeProperties();




        private ViewModelFactory vMf = new ViewModelFactory();
        private DataBaseService DataBaseService;

        [ObservableProperty]
        private ApperanceSettings _apperanceSettings = new ApperanceSettings();

        [ObservableProperty]
        private UnitsSettings _unitSettings = new UnitsSettings();

        private UnitsConverter UnitsCon;
        [ObservableProperty]
        private WeatherSettings _weatherSettings = new WeatherSettings();


        [ObservableProperty]
        private NetworkManager _networkManager;


        private WeatherForecastController WeatherForecastController;
        public static event EventHandler<string> CurrentPageOpened;
        public static string CurrentPageSub = "";
        public static InDoorSensor inDoorSens = new InDoorSensor();
        public static MQTTcommunication mqqt = new MQTTcommunication();
        public static UARTcommunication uart;
        public static string lastPage = "";
       


        private readonly List<ViewModelBase> pages;




        [ObservableProperty]
        public string _currentPageName = "abc";


        [ObservableProperty]
        public ViewModelBase _currentPage;

        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;

        [ObservableProperty]
        public bool _animationOn = false;

        [ObservableProperty]
        public bool _themeBtnVis;

        /// <summary>
        /// List of navigation items.
        /// </summary>
        public ObservableCollection<ListItemTemplate> Items { get; } = new()
            {
                new ListItemTemplate(typeof(HomePageViewModel),"HomeRegular", "Strona Główna"),
                new ListItemTemplate(typeof(SettingsViewModel),"SettingsRegular","Ustawienia"),
                new ListItemTemplate(typeof(ChartViewModel),"ChartRegular","Wykresy"),
            };




        private readonly SettingsManager settingsManager;
        private TimeSpan LightThemeTime;
        private TimeSpan DarkThemeTime;

        /// <summary>
        /// Constructor for the MainWindowViewModel class.
        /// </summary>
        /// 

        private async void LoadSettings()
        {
            await settingsManager.LoadSettingsAsync("Appearance", ApperanceSettings);
            await settingsManager.LoadSettingsAsync("Units", UnitSettings);
            ThemeBtnVis = ApperanceSettings.ThemeButtonVis;
            SetThemeSchedule(ApperanceSettings.UseSchduleThemeChange);
          
            (ApperanceSettings.LightTheme ? (Action)SetLightTheme : SetDarkTheme)();
            Console.WriteLine($"ZMIENNA PREVIOUS:  {ApperanceSettings.prevLightTheme}");
            Console.WriteLine(ApperanceSettings.LightTheme);


        }





        [ObservableProperty]
        public OutdoorSensors _outdoorSens;

        [ObservableProperty]
        public IndoorSensors _indoorSens;

        [ObservableProperty]
        public ESPnetworkData _espNet;




        public MainWindowViewModel()
        {
            _networkManager = new NetworkManager();



            settingsManager = new SettingsManager("ustawienia.json");
            LoadSettings();
            DataBaseService = new DataBaseService();

            UnitsCon = new UnitsConverter(UnitSettings);

            OutdoorSens = new OutdoorSensors(DataBaseService, UnitSettings, UnitsCon);
            IndoorSens = new IndoorSensors(DataBaseService, UnitSettings, UnitsCon);


            WeatherForecastController = new WeatherForecastController(WeatherSettings, settingsManager, UnitsCon, UnitSettings);
            uart = new UARTcommunication(IndoorSens, "/dev/ttyS0");
            StartUART();
            vMf.DataBaseService = DataBaseService;
            vMf.WeatherController = WeatherForecastController;
            vMf.Settings = settingsManager;
            vMf.ApperanceSettings = ApperanceSettings;

            vMf.UnitSettings = UnitSettings;

            Units.GetInstance().Ustaw = UnitSettings;
            Units.GetInstance().SetSub();
            vMf.TimeProperties = TimeProp;
            vMf.OutdoorSensors = _outdoorSens;
            vMf.IndoorSensors = _indoorSens;
            vMf.NetworkManager = NetworkManager;


            WeakReferenceMessenger.Default.Register<ThemeBtnVisMessage>(this, (r, m) =>
            {
                ThemeBtnVis = m.Value;
            });
            WeakReferenceMessenger.Default.Register<AutoThemeMessage>(this, (r, m) =>
            {
                SetThemeSchedule(m.Value);
            });


            EspNet = ESPnetworkData.GetInstance();

            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(1);
            //timer.Start();
            //StartClock();

            CurrentPageSub = "AvaloniaTest.ViewModels.HomePageViewModel";
            CurrentPageOpened?.Invoke(this, CurrentPageSub ?? "");

            StartDataReading();
            mqqt.Start_Server();
            if (Items.Any())
            {
                SelectedListItem = Items.First();
            }

            Clock.Instance.AddTask("Clock", UpdateClock, TimeSpan.FromSeconds(1));
            Clock.Instance.AddTask("CurrentWifiData", RefreshCurrentNetData, TimeSpan.FromSeconds(20));

            mqqt.AddSensros(OutdoorSens);
            SubESPwifiStrength();
            NetworkManager.GetCurrentNetworkInfo();

            NetworkManager.PropertyChanged += NetMan_PropertyChanged;

            NetworkManager.StrenghIcon = GetNetworkStrengthIcon(0);
            NetworkManager.NetworkConnected += OnInternetConnected;
        }

        private void OnInternetConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Polaczono z netem");
            WeatherForecastController.SetNewerCitys(WeatherSettings.Longitude, WeatherSettings.Latitude);

        }



        private void NetMan_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("jakas zmina w sieci ");
            Console.WriteLine(e.PropertyName);
            if (e.PropertyName == nameof(NetworkManager.SignalStrength))
            {
                Console.WriteLine("zmiana sygnalu");
                NetworkManager.StrenghIcon = GetNetworkStrengthIcon(NetworkManager.SignalStrength);
            }

        }
        public async void RefreshCurrentNetData()
        {
            Console.WriteLine("cykliczne pobieranie danych o aktualnej siecii");
            var (currentSsid, currentSignalStrength) = await NetworkManager.GetCurrentNetworkInfo();
            Console.WriteLine($"SSID: {currentSsid}, Siła sygnału: {currentSignalStrength}");
        }

      

        [RelayCommand]
        public void Getssidrpi()
        {
            Console.WriteLine("KILK pobieranie aktualnej nazwy sieci");
            string nazwa = NetworkManager.GetCurrentSsidRPI();
            Console.WriteLine($"Odczytana nazwa:{nazwa}");
        }
        [RelayCommand]
        public async void Getpassword()
        {
            Console.WriteLine("KILK pobieranie aktualnej nazwy sieci i sily sygnalu");
            var (currentSsid, currentSignalStrength) = await NetworkManager.GetCurrentNetworkInfo();
            Console.WriteLine($"SSID: {currentSsid}, Siła sygnału: {currentSignalStrength}");
        }
        [RelayCommand]
        public async void Getlist()
        {
            Console.WriteLine("GUZIK KKLIKKKKKKKKKKKKKKK");
                    await NetworkManager.GetListRPI();

        }
        [RelayCommand]
        public async void Connection()
        {
            Console.WriteLine("Klik laczenie");
            NetworkManager.ConnectRPI("a", "8310R939V");
            
        }


        private void SubESPwifiStrength()
        {
            if (EspNet.Strength != null)
            {
                EspNet.PropertyChanged += OnSubESPwifiStrengthChanged;
            }
        }





        private StreamGeometry GetNetworkStrengthIcon(double strength)
        {

            if (strength == 0)
            {
                Console.WriteLine("wifi0");
                return (StreamGeometry)Application.Current.FindResource("NoWifi");
            }
            if (strength <= -70)
            {
                Console.WriteLine("wifi3");
               // Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
               // {
                    return (StreamGeometry)Application.Current.FindResource("Wifi3");
              //  });
            }
            else if (strength > -70 & strength <= -50)
            {
                Console.WriteLine("wifi2");
                //Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
               // {
                    return (StreamGeometry)Application.Current.FindResource("Wifi2");
               // });
            }
            else 
            {
                Console.WriteLine("wifi1");
             //   Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
               // {
                    return (StreamGeometry)Application.Current.FindResource("Wifi1");
                //});
            }

        }

    
        private void OnSubESPwifiStrengthChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("sila sygnalu");
            Console.WriteLine(EspNet.Strength);
            EspNet.StrenghIcon = GetNetworkStrengthIcon(EspNet.Strength);

        }


        //zmiany wszystkich doubli
        private void SubscribeToPropertyChanges()
        {
            // Używamy reflection, aby subskrybować zmiany właściwości we wszystkich obiektach MqttTopic
            foreach (var property in typeof(OutdoorSensors).GetProperties())
            {
                if (property.GetValue(OutdoorSens) is SensorInfo<double> topic)
                {
                    topic.PropertyChanged += OnMqttTopicPropertyChanged;
                }
            }
        }
        //co sie dzieje przy zmianie wszystkich doubli
        private void OnMqttTopicPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SensorInfo<string>.DisplayName))
            {
                // Odpowiednia reakcja na zmianę wartości
                Console.WriteLine($"{((SensorInfo<double>)sender).Name} zmieniło się na {((SensorInfo<double>)sender).DisplayName}");
            }
        }



        private void UpdateClock()
        {
            
            DateTime t = Clock.Instance.CurrentTime;
            TimeProp.CurrentDate =  t.ToString("dd-MM-yyyy");
            TimeProp.CurrentTime = t.Hour.ToString("D2") + ":" + t.Minute.ToString("D2");
            TimeProp.CurrentDay = char.ToUpper(t.ToString("dddd")[0]) + t.ToString("dddd").Substring(1);
        }

        private void SetThemeSchedule(bool auto)
        {
            //
            if (auto)
            {
                //  TimeOnly l = new(LightThemeTime.Hours, LightThemeTime.Minutes);
                //  Clock.Instance.AddSpecificTimeTask("AutoLightThemeChange", SetLightTheme, new List<TimeOnly> { l });
                //
                //  TimeOnly d = new(DarkThemeTime.Hours, DarkThemeTime.Minutes);
                //  Clock.Instance.AddSpecificTimeTask("AutoDarkThemeChange", SetDarkTheme, new List<TimeOnly> { d });


                TimeOnly moring = new(ApperanceSettings.CustomLightThemeTime?.Hours ?? 6, ApperanceSettings.CustomLightThemeTime?.Minutes ?? 0);
                TimeOnly night = new(ApperanceSettings.CustomDarkThemeTime?.Hours ?? 20, ApperanceSettings.CustomDarkThemeTime?.Minutes ?? 0);
                Clock.Instance.AddSpecificTimeTask("AutoLightThemeChange", SetLightTheme, new List<TimeOnly> { moring });
                Clock.Instance.AddSpecificTimeTask("AutoDarkThemeChange", SetDarkTheme, new List<TimeOnly> { night });
            }
            else 
            {
                Clock.Instance.RemoveSpecificTimeTask("AutoLightThemeChange");
                Clock.Instance.RemoveSpecificTimeTask("AutoDarkThemeChange");
            }
        }



        private async Task StartUART()
        {      
            Task task1 = uart.ReadDataAsync();
            await Task.WhenAll(task1);

        }

        private async void SetLightTheme()
        {
            Console.WriteLine("Motyw jasny");
            App app = (App)Application.Current;
            if (app is not null)
            {
                app.SetTheme(ThemeVariant.Light);
                ApperanceSettings.LightTheme = true;
            }
           
        }

        private void SetDarkTheme()
        {
            Console.WriteLine("Motyw cinmy");
            App app = (App)Application.Current;
            if (app is not null)
            {
                app.SetTheme(ThemeVariant.Dark);
                ApperanceSettings.LightTheme = false;
            }
        }

        /// <summary>
        /// Method to start server.
        /// </summary>
        static async Task StartSERWER()
        {
            await mqqt.Start_Server();
        }

        /// <summary>
        /// Method to start reading data.
        /// </summary>
        public async Task StartDataReading()
        {            
            //Task task1 = inDoorSens.RunReadData();
            //await Task.WhenAll(task1);
        }


        private readonly Dictionary<Type, ViewModelBase> _viewModelCache = new();
        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {

            AnimationOn = false;

            if (value == null) return;
            if (!_viewModelCache.TryGetValue(value.ModelType, out var instance))
            {
                //   instance = (ViewModelBase)Activator.CreateInstance(value.ModelType, "napis")!;
                instance = vMf.CreateViewModel(value.ModelType);
                _viewModelCache[value.ModelType] = instance;
            }

            CurrentPage = instance;
            CurrentPageSub = CurrentPage.ToString();


            //CurrentPageOpened?.Invoke(this, CurrentPageSub ?? "");      


            CurrentPageName = value.Label;
            AnimationOn = true;


            WeakReferenceMessenger.Default.Send(new ViewActivatedMessages(CurrentPageSub));
        }
       

        [RelayCommand]
        public void Odczyt()
        {
            Console.WriteLine($"Odczyt:");
            Clock.Instance.UpdateTaskInterval("Clocker", TimeSpan.FromSeconds(1));
        }



        /// <summary>
        /// Method to change the application theme.
        /// </summary>
        [RelayCommand]
        public void ChangeTheme()
        {
            App app = (App)Application.Current;
            app.ChangeTheme();
            ApperanceSettings.LightTheme = app.ActualThemeVariant == ThemeVariant.Light;
            Console.WriteLine(ApperanceSettings.LightTheme);
        }

        /// <summary>
        /// Method to set the default list item.
        /// </summary>
        public void SetDefaultItem()
        {
            SelectedListItem = Items[0];
            if (SelectedListItem is not null)
            {
                OnSelectedListItemChanged(SelectedListItem);
            }
           
        }

        /// <summary>
        /// Method to navigate to the settings view.
        /// </summary>
        //KOMENTARZ DO FUNKCJI NAPRAEWIC
        public void handleGoToSettgins()
        {        
            var settingsItem = Items.FirstOrDefault(item => item.ModelType == typeof(SettingsViewModel));
            if (settingsItem != null)
            {
                int index = Items.IndexOf(settingsItem);
                if (SelectedListItem != Items[index])
                {
                    SelectedListItem = Items[index];
                    OnSelectedListItemChanged(SelectedListItem);
                }
                else {
                    Console.WriteLine("juz jest");
                }
            }
        }
    }

    /// <summary>
    /// Class representing a template for a list item.
    /// </summary>
    public class ListItemTemplate
    {

        public ListItemTemplate(Type type, string iconKey, string name)
        {
            ModelType = type;
            //Label = type.Name.Replace("ViewModel", "");
            Label = name;
            Application.Current!.TryFindResource(iconKey, out var res);
            ListItemIcon = (StreamGeometry)res!;

        }
        public string Label { get; }
        public Type ModelType { get; }
        public StreamGeometry ListItemIcon { get; }
    }

}