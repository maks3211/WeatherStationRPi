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
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
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

  

        private WeatherForecastController WeatherForecastController;
        public static event EventHandler<string> CurrentPageOpened;
        public static string CurrentPageSub = "";
        public static InDoorSensor inDoorSens = new InDoorSensor();
        public static MQTTcommunication mqqt = new MQTTcommunication();
        public static string lastPage = "";
        public static Network siec = new Network();

     
        private readonly List <ViewModelBase>pages;
        
        


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
       

        [RelayCommand]
        public async Task SaveCommand()
        {
              Console.WriteLine("Czytanie siecii");
              string esp32Url = "http://192.168.4.1/getData";
              using (HttpClient client = new HttpClient())
              {
                  Console.WriteLine("Czytanie sieciisss");
                  var response = await client.GetStringAsync($"{esp32Url}");


                  var jsonResponseArray = JArray.Parse(response);

                  foreach (var item in jsonResponseArray)
                  {
                      Console.WriteLine($"Nr {item["nr"]}");
                      Console.WriteLine($"SSID: {item["ssid"]}");
                      Console.WriteLine($"RRSI: {item["rssi"]}%");
                      Console.WriteLine($"CHANEL: {item["channel"]}");
                      Console.WriteLine("------");
                  }
        }
        }


        [RelayCommand]
        public async Task SendCommand()
        {
            /* Console.WriteLine("Wysylanie wiadomosci");
             string baseUrl = "http://192.168.4.1/";

             // Parametry zapytania
             string message = "TP-Link_0E21";
             string queryString = $"wiadomosc={Uri.EscapeDataString(message)}";

             // Tworzymy pełny URL z parametrami
             string fullUrl = $"{baseUrl}?{queryString}";

             using (HttpClient client = new HttpClient())
             {
                 try
                 {
                     // Wysyłamy zapytanie GET do pełnego URL
                     HttpResponseMessage response = await client.GetAsync(fullUrl);

                     // Sprawdzamy, czy odpowiedź była udana
                     response.EnsureSuccessStatusCode();

                     // Odczytujemy treść odpowiedzi jako string
                     string responseBody = await response.Content.ReadAsStringAsync();

                     // Wyświetlamy treść odpowiedzi na konsoli
                     Console.WriteLine(responseBody);
                 }
                 catch (HttpRequestException e)
                 {
                     Console.WriteLine($"Błąd podczas zapytania: {e.Message}");
                 }
             }*/
            string esp32Url = "http://192.168.4.1/postData";  // Adres URL do ESP32

            // Przygotowanie danych JSON
            string jsonData = "{\"wifi_ssid\":\"TP-Link_0E21\",\"wifi_pass\":\"66275022\"}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Tworzymy zawartość zapytania POST (JSON)
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Wysłanie zapytania POST
                    HttpResponseMessage response = await client.PostAsync(esp32Url, content);

                    // Sprawdzenie, czy odpowiedź jest poprawna
                    response.EnsureSuccessStatusCode();

                    // Odczytanie odpowiedzi jako tekst
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Wyświetlenie odpowiedzi w konsoli
                    Console.WriteLine("Odpowiedź z ESP32: " + responseData);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nBłąd: " + e.Message);
                }
            
        }

        }





        public OutdoorVal Wartosci { get; } = new OutdoorVal();

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


         //   await settingsManager.LoadSettingsAsync("Units", UnitSettings);

            ThemeBtnVis = ApperanceSettings.ThemeButtonVis;
            

            SetThemeSchedule(ApperanceSettings.UseSchduleThemeChange);
            (ApperanceSettings.LightTheme ? (Action)SetLightTheme : SetDarkTheme)();
  

            
        }





        [ObservableProperty]
        public OutdoorSensors _sensei;

        [ObservableProperty]
        public ESPnetworkData _espNet;


  

        public MainWindowViewModel()
        {
            Console.WriteLine("KONSTRUKOR !!!!!!!!!!");

            
            settingsManager = new SettingsManager("ustawienia.json");
            LoadSettings();
            DataBaseService = new DataBaseService();

            UnitsCon = new UnitsConverter(UnitSettings);
            Sensei = new OutdoorSensors(DataBaseService, UnitSettings, UnitsCon);
            WeatherForecastController = new WeatherForecastController(WeatherSettings, settingsManager, UnitsCon, UnitSettings);


            vMf.DataBaseService = DataBaseService;
            vMf.WeatherController = WeatherForecastController;
            vMf.Settings = settingsManager;
            vMf.ApperanceSettings = ApperanceSettings;
  
            vMf.UnitSettings = UnitSettings;

            Units.GetInstance().Ustaw = UnitSettings;
            Units.GetInstance().SetSub();
            vMf.TimeProperties = TimeProp;
            vMf.OutdoorSensors = _sensei;
            

             
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
           
            mqqt.AddSensros(Sensei);
            SubESPwifiStrength();
           

        }


        

        private void SubESPwifiStrength()
        {
            if (EspNet.Strength != null)
            {
                EspNet.PropertyChanged += OnSubESPwifiStrengthChanged;
            }
        }


    
        private void OnSubESPwifiStrengthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (EspNet.Strength <= -70)
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    EspNet.StrenghIcon = (StreamGeometry)Application.Current.FindResource("Wifi3");
                });
            }
           else if (EspNet.Strength > -70 & EspNet.Strength <= -50)
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    EspNet.StrenghIcon = (StreamGeometry)Application.Current.FindResource("Wifi2");
                });
            }
           else 
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    EspNet.StrenghIcon = (StreamGeometry)Application.Current.FindResource("Wifi1");
                });
            }


        }


        //zmiany wszystkich doubli
        private void SubscribeToPropertyChanges()
        {
            // Używamy reflection, aby subskrybować zmiany właściwości we wszystkich obiektach MqttTopic
            foreach (var property in typeof(OutdoorSensors).GetProperties())
            {
                if (property.GetValue(Sensei) is MqttTopic<double> topic)
                {
                    topic.PropertyChanged += OnMqttTopicPropertyChanged;
                }
            }
        }
        //co sie dzieje przy zmianie wszystkich doubli
        private void OnMqttTopicPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MqttTopic<string>.DisplayName))
            {
                // Odpowiednia reakcja na zmianę wartości
                Console.WriteLine($"{((MqttTopic<double>)sender).Name} zmieniło się na {((MqttTopic<double>)sender).DisplayName}");
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
            Task task1 = inDoorSens.RunReadData();
            await Task.WhenAll(task1);
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
        /// Method to start reading WIFI data.
        /// </summary>
        public async Task StartWifiReading() {
            Task t1 = siec.GetWifiList();      
            await Task.WhenAll(t1);
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