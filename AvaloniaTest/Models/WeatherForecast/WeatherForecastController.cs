using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Avalonia.Media.Imaging;
using System.IO;
using System.Security.Policy;
using ScottPlot.Colormaps;
using static AvaloniaTest.Models.WeatherForecast.WeatherForecast;
using System.Reflection;
using AvaloniaTest.Helpers;
using System.Collections.ObjectModel;
using AvaloniaTest.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using MySqlX.XDevAPI;
using AvaloniaTest.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Platform;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Avalonia.Animation;
using AvaloniaTest.Services.AppSettings;

namespace AvaloniaTest.Models.WeatherForecast
{
    public partial class WeatherForecastController : ObservableObject
    {
        [ObservableProperty]
        public WeatherSettings _settings;

        public SettingsManager SettingsMan;

        [ObservableProperty]
        public string _city;

      //  private float lon;
     //   private float lat;

        [ObservableProperty]
        private Bitmap _currentIcon;

        [ObservableProperty]
        public int _currentTemperature;

        [ObservableProperty]
        public string _weatherDesc;

        [ObservableProperty]
        public int _minTemperature;
        [ObservableProperty]
        public int _maxTemperature;

        [ObservableProperty]
        private string _currentDay = char.ToUpper(DateTime.Now.ToString("dddd")[0]) + DateTime.Now.ToString("dddd").Substring(1);

        [ObservableProperty]
        private string _currentDate = DateTime.Now.ToString("dd-MM-yyyy");

        [ObservableProperty]
        public int _updateIntervalMinuts;

        [ObservableProperty]
        private bool _autoWeatherRefresh;

        [ObservableProperty]
        public string _lastUpdateTime;

        [ObservableProperty]
        public bool _httpSucces;

        private UnitsConverter UnitsConverter;
        private UnitsSettings Units;
       // private static WeatherForecastController _instance;

        public ObservableCollection<HourlyForecastTemplate> HourlyForecastItems { get; set; } = new() { };

        private bool isFirstLoad = true;

        public ObservableCollection<DailyForecastTemplate> DailyForecastItems { get; set; } = new() { };

        partial void OnCityChanged(string? value)
        {
            Console.WriteLine("Zmieniono miasto!!");
            GetAllData();

        }


        private HttpClient web;
        // private readonly string APIkey = "13740c980d70f1a49a62b027708cb097";


       // [ObservableProperty]
      //  private string _aPIkey;


        //private readonly string APIkey = "18dede3a5891aa7f0c4f991203e451c0"; kod z neta

     
        //

       // public string city = "Pszczyna";
        public WeatherInfo CurrentWeather;
        public HourlyForecastInfo Forecast3h;
        public HourlyForecastInfo Forecast1h;
        public DailyForecsastInfo DailyForecsast;


        private Dictionary<string, Bitmap> _iconCache = new Dictionary<string, Bitmap>();
        public long TimeZone;

     /*   public static WeatherForecastController GetInstance()
        {
            if (_instance == null)
            {
                _instance = new WeatherForecastController();
            }
            return _instance;
        }*/



        

        public DailyForecsastInfo GetDailyForecastdwa()
        {
            if (DailyForecsast == null)
                GetDailyhForecast();
            return DailyForecsast;

            
        }

        public WeatherForecastController(WeatherSettings settings, SettingsManager settingsManager, UnitsConverter unitConverter, UnitsSettings units)
        {
            Settings = settings;
            SettingsMan = settingsManager;
            UnitsConverter = unitConverter;
            Units = units;

      


            web = new HttpClient();
/*            WeakReferenceMessenger.Default.Register<UnitChangedMessage>(this, (r, m) =>
            {
                UpdateUnits();
            });*/

            LoadAllData();


            Clock.Instance.AddTask("autoWeather",GetAllData,TimeSpan.FromMinutes(Settings.RefreshInterval));
            Units.PropertyChanged += Units_PropertyChanged;
        }

        private void Units_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {   
            if (e.PropertyName == nameof(Units.Temp))
            {
                if (!isFirstLoad)
                {
                    UpdateUnits();
                }
            }
        }

        private async Task LoadAllData()
        {
            await LoadSettings();
            await SetNewerCitys(Settings.Longitude, Settings.Latitude);
            isFirstLoad = false;
        }


        private async Task LoadSettings()
        {
            await SettingsMan.LoadSettingsAsync("Weather", Settings);
            UpdateIntervalMinuts = Settings.RefreshInterval;

            AutoWeatherRefresh = !(UpdateIntervalMinuts == 0);
          //  lat = Settings.Latitude;
          //  lon = Settings.Longitude;
          //  APIkey = Settings.ApiKey;
        }


        public async Task SaveSettings()
        {
            await SettingsMan.SaveSettingsAsync("Weather", Settings);
        }

        partial void OnUpdateIntervalMinutsChanged(int value)
        {
            Settings.RefreshInterval = value;
            SaveSettings();
            Clock.Instance.UpdateTaskInterval("autoWeather", TimeSpan.FromMinutes(Settings.RefreshInterval)); 
        }

        public async void GetAllData()
        {
            if (!HttpSucces)
            {
                return; 
            }
            var t = Clock.Instance.CurrentTime;
            LastUpdateTime = t.Hour.ToString("D2") + ":" + t.Minute.ToString("D2");
            CurrentDay = char.ToUpper(DateTime.Now.ToString("dddd")[0]) + DateTime.Now.ToString("dddd").Substring(1);
            CurrentDate = DateTime.Now.ToString("dd-MM-yyyy");
            await GetCurrentWeather();
            await Get1hForecast();
            await GetDailyhForecast();
        }
        private void UpdateUnits()
        {
            if (UnitsConverter == null)
                return;
            string Currentday = _currentDate.Split('-')[0];
            CurrentTemperature = (int)Math.Round(UnitsConverter.CalculateTemp(CurrentWeather.main.temp));
            MinTemperature = (int)Math.Round(UnitsConverter.CalculateTemp(Forecast1h.GetDailyMinTemp(Currentday)));
            MaxTemperature = (int)Math.Round(UnitsConverter.CalculateTemp(Forecast1h.GetDailyMaxTemp(Currentday)));
            int i = 0;
            foreach (var a in Forecast1h.list)
            {
                HourlyForecastItems[i].Temperature = (int)Math.Round(UnitsConverter.CalculateTemp(a.main.temp));
                // a.dt + 86400 -> go to next day 
                i++;
            }
            i = 0;
            foreach (var a in DailyForecsast.list)
            {

                DailyForecastItems[i].MinTemp = (int)Math.Round(UnitsConverter.CalculateTemp(a.temp.min));
                DailyForecastItems[i].MaxTemp = (int)Math.Round(UnitsConverter.CalculateTemp(a.temp.max));
                i++;
            }
            i = 0;
        }

        public async Task CheckAPIkey()
        {
            
        }

   


        public async Task SetNewCitys(float lon, float lat)
        {
           
            string url = string.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&lang=pl&units=metric&APPID={2}", lat, lon, Settings.ApiKey);

           
                // Utworzenie żądania
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await web.SendAsync(request);
                    HttpSucces = response.IsSuccessStatusCode;
                // Sprawdzenie, czy odpowiedź jest sukcesem
                if (!HttpSucces)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    JObject errorObject = JObject.Parse(errorContent);
                    string errorMessage = errorObject["message"]?.ToString();
                    int cod = (int)errorObject["cod"];
                    Console.WriteLine($"Błąd API: {cod} - {errorMessage}");
                if (cod == 401)
                {
                    City = "Błędny kod API";
                }
                    return;
                }

                // Jeśli odpowiedź jest sukcesem, odczytaj zawartość
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);

                string cityName = jsonObject["name"]?.ToString();
                if (cityName is null) return;

                City = cityName;
                Settings.Latitude = lat;
                Settings.Longitude = lon;
                await SaveSettings();
   
        }

        public async Task SetNewerCitys(float lon, float lat)
        {
            int maxRetryAttempts = 5; // Maksymalna liczba prób
            int retryDelayInSeconds = 5; // Opóźnienie między próbami
            int attempt = 0;
            while (attempt < maxRetryAttempts)
            {
                attempt++;
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&lang=pl&units=metric&APPID={2}", lat, lon, Settings.ApiKey);

                // Utworzenie żądania
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await web.SendAsync(request);
                HttpSucces = response.IsSuccessStatusCode;

                // Sprawdzenie, czy odpowiedź jest sukcesem
                if (!HttpSucces)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    JObject errorObject = JObject.Parse(errorContent);
                    string errorMessage = errorObject["message"]?.ToString();
                    int cod = (int)errorObject["cod"];
                    Console.WriteLine($"Błąd API: {cod} - {errorMessage}");

                    // Jeśli kod to 401 (błędny klucz API), poczekaj i ponów próbę
                    if (cod == 401)
                    {
                        City = "Błędny kod API";
                        if (attempt < maxRetryAttempts)
                        {
                            Console.WriteLine("Ponawianie próby za kilka sekund...");
                            await Task.Delay(retryDelayInSeconds * 1000); // Opóźnienie przed kolejną próbą
                            continue; // Powrót do początku pętli
                        }
                        else
                        {
                            Console.WriteLine("Osiągnięto maksymalną liczbę prób. Przerywanie...");
                            return; // Przerywamy, jeśli osiągnięto maksymalną liczbę prób
                        }
                    }
                }
                else { 
                // Jeśli odpowiedź jest sukcesem, odczytaj zawartość
                var json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);

                string cityName = jsonObject["name"]?.ToString();
                if (cityName is null) return;

                City = cityName;
                Settings.Latitude = lat;
                Settings.Longitude = lon;
                await SaveSettings();

                break; // Wychodzimy z pętli, jeśli operacja zakończyła się sukcesem
                }
            }
        }

        public async Task GetCurrentWeather()
        {
            if (City == "")
                return;
            //    string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q=Pszczyna,pl&APPID=13740c980d70f1a49a62b027708cb097");
           // string url = "https://api.openweathermap.org/data/2.5/weather?q=Pszczyna,pl&lang=pl&APPID=13740c980d70f1a49a62b027708cb097";
            string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&lang=pl&units=metric&APPID={1}",City , Settings.ApiKey);
            try
            {
                var json = await web.GetStringAsync(url);
                if (json is null)
                {
                    return;
                }
                CurrentWeather = JsonConvert.DeserializeObject<WeatherForecast.WeatherInfo>(json);
                TimeZone = CurrentWeather.timezone;
                CurrentTemperature = (int)Math.Round(UnitsConverter.CalculateTemp(CurrentWeather.main.temp));
                WeatherDesc = CurrentWeather.weather[0].description.FirstCharToUpper();
                CurrentIcon = await GetIcon();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public async Task GetDailyhForecast()
        {
            if (City == "") return;
            string url = string.Format("https://api.openweathermap.org/data/2.5/forecast/daily?q={0}&lang=pl&units=metric&APPID={1}", City, Settings.ApiKey);
            try
            {
                var json = await web.GetStringAsync(url);
                if (json is null)
                {
                    return;
                }
                //    DailyForecsastInfo i = new DailyForecsastInfo();
                DailyForecsast = JsonConvert.DeserializeObject<DailyForecsastInfo>(json);
                bool isRain = false;
                DailyForecastItems.Clear();
                foreach (var a in DailyForecsast.list)
                {
                    var nazwaDzien = TimeConverters.ConvertDayOfWeekToShort(TimeConverters.ConvertDateTimeToDayOfWeek(a.dt, TimeZone));
                    Bitmap icon = await GetBetterIcon(a);
                    int prob = (int)(a.pop * 100);
                    double vol = a.rain ?? 0.0;
                    string day = a.dt.ToString();
                    if (a.pop > 0)
                    {
                        isRain = true;
                    }
                    else
                        isRain = false;
                    DailyForecastItems.Add(new DailyForecastTemplate(nazwaDzien, icon, (int)Math.Round(UnitsConverter.CalculateTemp(a.temp.min)), (int)Math.Round(UnitsConverter.CalculateTemp(a.temp.max)), isRain, prob, vol));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

           



        }


        public async Task Get1hForecast()
        {
            if (City == "") return;
            string w = "https://pro.openweathermap.org/data/2.5/forecast/hourly?q=Pszczyna,pl&lang=pl&appid=13740c980d70f1a49a62b027708cb097";
            string url = string.Format("https://pro.openweathermap.org/data/2.5/forecast/hourly?q={0}&lang=pl&units=metric&APPID={1}", City, Settings.ApiKey);
            try {
                var jsonPogoda = await web.GetStringAsync(url);
                if (jsonPogoda is null)
                {
                    return;
                }
                Forecast1h = JsonConvert.DeserializeObject<WeatherForecast.HourlyForecastInfo>(jsonPogoda);
                if (TimeZone == 0)
                {
                    TimeZone = Forecast1h.city.timezone;
                }

                foreach (var a in Forecast1h.list)
                {
                    a.dayTime = TimeConverters.ConvertDateTimeToHour(a.dt, TimeZone);
                }

                bool isRain = false;
                bool isNextDay = false;
                HourlyForecastItems.Clear();
                foreach (var a in Forecast1h.list)
                {
                    Bitmap icon = await GetBetterIcon(a);
                    if (a.pop > 0)
                    {
                        isRain = true;
                    }
                    else
                        isRain = false;
                    if (a.dayTime == "23")
                    {
                        isNextDay = true;

                    }
                    else
                        isNextDay = false;
                    // a.dt + 86400 -> go to next day 
                    string Currentday = _currentDate.Split('-')[0];
                    MinTemperature = (int)Math.Round(UnitsConverter.CalculateTemp(Forecast1h.GetDailyMinTemp(Currentday)));
                    MaxTemperature = (int)Math.Round(UnitsConverter.CalculateTemp(Forecast1h.GetDailyMaxTemp(Currentday)));
                    HourlyForecastItems.Add(new HourlyForecastTemplate(a.dayTime, icon, (int)Math.Round(UnitsConverter.CalculateTemp(a.main.temp)), (int)(a.pop * 100), 2.0, isRain, isNextDay, TimeConverters.ConvertDayOfWeekToShort(TimeConverters.ConvertDateTimeToDayOfWeek(a.dt + 86400, Forecast1h.city.timezone))));
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            
        }



        
        public double GetDailyMaxTemp(IForecast forecast, string date)
        {
            var ind = GetIndexesForCurrentDay(forecast, date);
            if (forecast == null || ind == null || !ind.Any() || !forecast.list.Any())
            {
                return -500; 
            }
            return ind
            .Where(index => index >= 0 && index < forecast.list.Count) 
            .Select(index => forecast.list[index].main.temp_max)
            .Max();
        }

        public double GetDailyMinTemp(IForecast forecast, string date)
        {
            var ind = GetIndexesForCurrentDay(forecast, date);
            if (forecast == null || ind == null || !ind.Any() || !forecast.list.Any())
            {
                return -500; 
            }
            return ind
            .Where(index => index >= 0 && index < forecast.list.Count) 
            .Select(index => forecast.list[index].main.temp_min)
            .Min();
        }

        private List<int> GetIndexesForCurrentDay(IForecast forecast, string targetDay)
        {
            if (forecast.list == null || forecast.list.Count == 0)
                return new List<int>();
            var list = new List<int>();
            
         
            for (int i = 0; i < forecast.list.Count; i++)
            {
                string day = ConvertDateTimeToDay(forecast.list[i].dt);
                if (day == targetDay)
                {
                    list.Add(i);
                }
                if (list.Count() > 24)
                {
                    break;
                }
            }
            return list;
        }
      


        public async Task Get3hForecast()
        {
            if (City == "")
                return;
            string url = string.Format("https://api.openweathermap.org/data/2.5/forecast?q={0}&lang=pl&units=metric&APPID={1}", City, Settings.ApiKey);

            try {
                var jsonPogoda = await web.GetStringAsync(url);
                if (jsonPogoda is null)
                {
                    return;
                }
                Forecast3h = JsonConvert.DeserializeObject<WeatherForecast.HourlyForecastInfo>(jsonPogoda);
                // ConvertDateTime(Pogoda.list[0].dt);


                //Console.WriteLine("temperatura pogoda: " + Forecast3h.list[0].main.temp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        private string ConvertDateTimeToDay(long millisec)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(millisec);
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromSeconds(TimeZone));
            string formattedDate = dateTimeOffset.Day.ToString("D2");

            return formattedDate;
        }

        private string ConvertDateTime(long millisec)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(millisec);
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromSeconds(TimeZone));
            string formattedDate = dateTimeOffset.ToString("HH:mm dd-MM-yyyy");

            return formattedDate;
        }




        public async Task<Bitmap> GetBetterIcon(IWeatherList list)
        {
            if (list.weather.Count() == 0)
                return null;
            var name = list.weather[0].icon;
            if (_iconCache.ContainsKey(name))
            {
                return _iconCache[name];
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {          
                    var imageStream = await client.GetStreamAsync($"https://openweathermap.org/img/wn/{name}@2x.png");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(ms);
                        ms.Position = 0;
                      
                        Bitmap bitmap = new Bitmap(ms);
                        _iconCache[name] = bitmap;
                    
                        return bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load image from URL: {ex.Message}");
                return null;
            }

        }




        public async Task<Bitmap> GetIcon()
        {

            var name = CurrentWeather.weather[0].icon;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var imageStream = await client.GetStreamAsync($"https://openweathermap.org/img/wn/{name}@2x.png");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(ms);
                        ms.Position = 0;
                       
                        return new Bitmap(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load image from URL: {ex.Message}");
                return null;
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

    public class HourlyForecastTemplate : ForecastBase
    {
        public string HourTime { get; }
        public int Temperature { get; set; }
        public bool IsNextDay { get; }
        public string NextDay { get; }

        public HourlyForecastTemplate(
            string hourTime,
            Bitmap icon,
            int temperature,
            int precipitationProb,
            double rainVol,
            bool isRain,
            bool isNextDay,
            string nextDay)
            : base(icon, isRain, precipitationProb, rainVol)
        {
            HourTime = hourTime;
            Temperature = temperature;
            IsNextDay = isNextDay;
            NextDay = nextDay;
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
