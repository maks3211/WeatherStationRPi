using Avalonia.Data;
using AvaloniaTest.Helpers;
using AvaloniaTest.Messages;
using AvaloniaTest.Models;
using AvaloniaTest.Models.AddressSearch;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Models.WeatherForecast;
using AvaloniaTest.Services.AppSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Google.Protobuf.WellKnownTypes;
using Mysqlx.Notice;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static AvaloniaTest.Models.WeatherForecast.WeatherForecast;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AvaloniaTest.ViewModels;

public partial class GeneralSettingsViewModel : ViewModelBase
{
    public GeneralSettingsViewModel(SettingsManager settings, ApperanceSettings apperanceSettings, UnitsSettings unitsSettings, WeatherForecastController weatherContoller)
    {
        CitySuggestion = new ObservableCollection<string>();
        settingsManager = settings;
        this.ApperanceSettings = apperanceSettings;
      
        UnitSettings = unitsSettings;
        WeatherController = weatherContoller;



        if (WeatherController.AutoWeatherRefresh)
        {
            WeatherUpdateInterval = WeatherController.UpdateIntervalMinuts;
        }
        else
        {
            WeatherUpdateInterval = 15;
        }


        WeakReferenceMessenger.Default.Register<UnitChangedMessage>(this, (r, m) =>
        {
            if (m.Value == true)
            {

            }

        });

        WeakReferenceMessenger.Default.Register<SettingsViewActivatedMessages>(this, (r, m) =>
        {
            if (m.Value == GetType().FullName)
            {


            }
            else
            {
                SaveSettings();
            }
        });
        var morning = ApperanceSettings.CustomLightThemeTime ?? TimeSpan.FromHours(6);
        var night = ApperanceSettings.CustomDarkThemeTime ?? TimeSpan.FromHours(20);
        CustomThemeTime = morning.ToString(@"hh\:mm") + " - " + night.ToString(@"hh\:mm");
    }



    private SettingsManager settingsManager;
    private OutdoorSensors outdoorSensors;
    [ObservableProperty]
    private WeatherForecastController _weatherController;
    public bool isPageOpen = false;



    [ObservableProperty]
    private int _spacerHeight = 310;


    [ObservableProperty]
    private int _weatherUpdateInterval;

    [ObservableProperty]
    private bool _autoWeatherRefresh;
    public ObservableCollection<CitySearchListTemplate> CityList { get; } = new();
  
   [ObservableProperty]
private CitySearchListTemplate? _selectedCity;



    partial void OnSelectedCityChanged(CitySearchListTemplate? value)
    {

        if (value is null)
        {
            //Passwordboxvis = false;
            return;
        }
        Console.WriteLine(value.City);
     //   WeatherForecastController.GetInstance().City = "Olsztyn";
        Console.WriteLine(value.Lon);
        Console.WriteLine(value.Lat);
        float longitude = 0.0F;
        float latitude = 0.0F;
        if (float.TryParse(value.Lon, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out longitude))
        {
            Console.WriteLine(longitude); // Jeśli konwersja się uda, zmienna number zawiera wartość float
        }
        else
        {
            Console.WriteLine("Nie udało się przekonwertować stringa na float.");
        }     
        if (float.TryParse(value.Lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out latitude))
        {
            Console.WriteLine(longitude); // Jeśli konwersja się uda, zmienna number zawiera wartość float
        }
        else
        {
            Console.WriteLine("Nie udało się przekonwertować stringa na float.");
        }
       // WeatherController.SetNewCity(longitude, latitude);
        WeatherController.SetNewCitys(longitude, latitude);

    }

    [ObservableProperty]
    public bool _zmiana;

    [ObservableProperty]
    private bool _autoBoxVisibility = false;

    private ObservableCollection<string> _citySuggestion;
    public ObservableCollection<string> CitySuggestion
    {
        get { return _citySuggestion; }
        set
        {
            _citySuggestion = value;
            OnPropertyChanged(nameof(CitySuggestion));
        }
    }

    // private bool _celsiusSelected = (Units.GetInstance().GetTempUnit() == "°C") ? true : false;
    // private bool _fahrenheitSelected = (Units.GetInstance().GetTempUnit() == "°F") ? true : false;


    [ObservableProperty]
    public ApperanceSettings _apperanceSettings;


    [ObservableProperty]
    public UnitsSettings _unitSettings;


    [ObservableProperty]
    public TimeSpan _lightThemeTime;

    [ObservableProperty]
    public string _customThemeTime = "";

    [ObservableProperty]
    public TimeSpan _darkThemeTime;


    [ObservableProperty]
    public bool _celsius;






    [RelayCommand]
    public void CustomThemeTimeChangedCommand()
    {
        Console.WriteLine("sssss");
    }

    private async void LoadSettings()
    {
        Console.WriteLine( $"TO WCZYTANO: {ApperanceSettings.CustomDarkThemeTime} od godzinei:  {ApperanceSettings.CustomLightThemeTime}");
        await settingsManager.LoadSettingsAsync("Appearance", ApperanceSettings);
        // await settingsManager.SaveSettingsAsync("Appearance", ApperanceSettings.Instance);
    }







    [ObservableProperty]
    public bool _autoThemeSwitchState;

    [RelayCommand]
    public async void SaveThemeScheduleCommand()
    {

     //   ApperanceSettings Apperance = ApperanceSettings;
       // Apperance.UseSchduleThemeChange = AutoThemeSwitchState;
      //  Apperance.CustomTimeThemeChange = CustomThemeSelected;
      //  Apperance.CustomLightThemeTime = LightThemeTime;
     //   Apperance.CustomDarkThemeTime = DarkThemeTime;
      //  Console.WriteLine($"Zapisane info po kliknięciu zapisz:\n Od zmroku do świtu {AutoThemeSelected}" +
     //       $"\n Reczny wybór czasu: {CustomThemeSelected}" +
      //      $"\n Czasy {LightThemeTime}  {DarkThemeTime}");

      //  await settingsManager.SaveSettingsAsync("Appearance", Apperance);

       // CustomThemeTime = LightThemeTime.ToString(@"hh\:mm") + " - " + DarkThemeTime.ToString(@"hh\:mm");
       // WeakReferenceMessenger.Default.Send(new AutoThemeMessage(true, LightThemeTime, DarkThemeTime)); 
    }

    [RelayCommand]
    public void AutoThemeSwitchCommand()
    {
        if (!AutoThemeSwitchState) {
          //  WeakReferenceMessenger.Default.Send(new AutoThemeMessage(false, LightThemeTime, DarkThemeTime));

        }
    }

    [RelayCommand]
    public void TemperatureChoiceCommand()
    {
       // Units.GetInstance().SetCelsius(UnitSettings.Celsius);
    }

    [RelayCommand]
    public async Task SearchCityButtonCommand()
    {
        if (CitySearchText.Length > 2)
        {
            CityList.Clear();
            SelectedCity = null;
            var resuts = await AddressSearchController.Search(CitySearchText);
            if (resuts != null)
            {
                if (resuts.Count < 2)
                    SpacerHeight = 210;
                else if (resuts.Count < 3)
                    SpacerHeight = 150;
                else
                    SpacerHeight = 90;


                foreach (var a in resuts)
                {
                    Console.WriteLine("no cos jest!!!!");
                    CityList.Add(new CitySearchListTemplate(
        a.address.name.AddComma(),
        a.address.city.AddComma(),
        a.address.county.AddComma(),
        a.address.state.AddComma(),
        a.address.postcode.AddComma(),
        a.address.country,
        a.lat, a.lon)
    );

                }
                CitySearchText = "";
            }
            else
            {
                Console.WriteLine("NULLLL");
                SpacerHeight = 310;
            }
        }

    }



    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private bool _mSelected = (Units.GetInstance().GetWindUnit() == "m/s") ? true : false;
    private bool _kmSelected = (Units.GetInstance().GetWindUnit() == "km/h") ? true : false;
    private bool _ktSelected = (Units.GetInstance().GetWindUnit() == "kt") ? true : false;

   // private bool _celsiusSelected = (Units.GetInstance().GetTempUnit() == "°C") ? true : false;
   // private bool _fahrenheitSelected = (Units.GetInstance().GetTempUnit() == "°F") ? true : false;


    [ObservableProperty]
    public bool _autoThemeSelected;

    [RelayCommand]
    public async void AutoThemeCommand()
    {
        Console.WriteLine($"zapis od zmorku do świetu {AutoThemeSelected}  reczny wybor: {CustomThemeSelected}");
        var set = WeatherController.GetDailyForecastdwa().list[0].sunset;
        var rise = WeatherController.GetDailyForecastdwa().list[0].sunrise;
        var timeZone = WeatherController.TimeZone;

        var morning = TimeConverters.ConvertDateTimeToHourMinute(rise, timeZone);
        ApperanceSettings.CustomLightThemeTime = morning;
        var night = TimeConverters.ConvertDateTimeToHourMinute(set, timeZone);
        ApperanceSettings.CustomDarkThemeTime = night; // ?? TimeSpan.FromHours(18) + TimeSpan.FromMinutes(30);


       // WeakReferenceMessenger.Default.Send(new AutoThemeMessage(true, morning, night));
        CustomThemeTime = morning.ToString(@"hh\:mm") + " - " + night.ToString(@"hh\:mm");

      
       
        //zapisanie ze auto 
    }

    [ObservableProperty]
    public bool _customThemeSelected;

    [RelayCommand]
    public async void CustomThemeCommand()
    {
     

    }

    private async void SaveSettings()
    {
        WeakReferenceMessenger.Default.Send(new AutoThemeMessage(ApperanceSettings.UseSchduleThemeChange));
        await settingsManager.SaveSettingsAsync("Appearance", ApperanceSettings);
        if (!WeatherController.AutoWeatherRefresh)
        {
            WeatherController.UpdateIntervalMinuts = 0;
        }
        else
        {
            WeatherController.UpdateIntervalMinuts = WeatherUpdateInterval;
        }

        await settingsManager.SaveSettingsAsync("Units", UnitSettings);

    }
   



    [ObservableProperty]
    public bool _themeButtonVis;

    [RelayCommand]
    public async void ThemeButtonVisSwitch()
    {
     //   Console.WriteLine(ThemeButtonVis);
     //Tutaj inna zmienna do wyslania
        WeakReferenceMessenger.Default.Send(new ThemeBtnVisMessage(ApperanceSettings.ThemeButtonVis));

     //   ApperanceSettings Apperance = ApperanceSettings.Instance;
     //   Apperance.ThemeButtonVis = ThemeButtonVis;
        //Console.WriteLine($"Zapis widocznosci guzika {ThemeButtonVis}");
       // await settingsManager.SaveSettingsAsync("Appearance", Apperance);
    }



    




   

    private bool MSelected
    {
        get => _mSelected;
        set
        {
            if (_mSelected != value)
            {
                _mSelected = value;
                OnPropertyChanged(nameof(MSelected));          
                if (value == true)
                {
                    Console.WriteLine("m");
                    Units.GetInstance().ChangeWindUnit("m");
                }  
            }
        }
    }

    public bool KmSelected
    {
        get => _kmSelected;
        set
        {
            if (_kmSelected != value)
            {
                _kmSelected = value;
                OnPropertyChanged(nameof(KmSelected));              
                if (value == true)
                {
                    Console.WriteLine("km");
                    Units.GetInstance().ChangeWindUnit("km");
                }
               
            }
        }
    }
    public bool KtSelected
    {
        get => _ktSelected;
        set
        {
            if (_ktSelected != value)
            {
                _ktSelected = value;
                OnPropertyChanged(nameof(KtSelected));            
                if (value == true)
                {
                    Console.WriteLine("kt");
                    Units.GetInstance().ChangeWindUnit("kt");
                }

            }
        }
    }

    private bool _celsiusSelected = true;

    private bool _fahrenheitSelected = false;
    public bool CelsiusSelected
    {
        get => _celsiusSelected;
        set
        {
            if (_celsiusSelected != value)
            {
                _celsiusSelected = value;
                OnPropertyChanged(nameof(CelsiusSelected));

                if (_celsiusSelected)
                {
                    FahrenheitSelected = false; // Upewnij się, że druga opcja jest odznaczona
                    Console.WriteLine("celek");
                    Units.GetInstance().ChangeTempUnit("C");
                }
            }
        }
    }

    public bool FahrenheitSelected
    {
        get => _fahrenheitSelected;
        set
        {
            if (_fahrenheitSelected != value)
            {
                _fahrenheitSelected = value;
                OnPropertyChanged(nameof(FahrenheitSelected));

                if (_fahrenheitSelected)
                {
                    CelsiusSelected = false; // Upewnij się, że pierwsza opcja jest odznaczona
                    Console.WriteLine("farek");
                    Units.GetInstance().ChangeTempUnit("F");
                }
            }
        }
    }





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

    [ObservableProperty]
    public string _citySearchText;

    AddressSearchController AddressSearchController = new AddressSearchController();
    public async Task UpdateCitySuggestions()
    {
       
        Console.WriteLine($"Ostatni  input: {lastCityImput}");
        Console.WriteLine($"Aktualny input: {CityInput}");


        if (CityInput.Length < 3)
        {
            Console.WriteLine("Mniejsze od 3 ");
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
            Console.WriteLine("OK roznica ");
            lastCityImput = CityInput;
        }



        lastCityImput = CityInput;

        Console.WriteLine("Szukam!!!!");
        var resuts = await AddressSearchController.Search(CityInput);
        if (resuts != null)
        {
           
            Console.WriteLine(resuts.Count);
            CitySuggestion.Clear();

            foreach (var a in resuts)
            {
                Console.WriteLine("no cos jest!!!!");
                CitySuggestion.Add(a.display_address);
                Console.WriteLine(a.display_name);
            }
        }
        else
        {
            Console.WriteLine("NULLLL");
        }


    }


    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

 

}
public class CitySearchListTemplate
{
    public CitySearchListTemplate(string? name, string? city, string? county, string? state, string? postcode, string? country, string? lat, string? lon)
    {
        Name = name;
        City = city;
        County = county;
        State = state;
        Postcode = postcode;
        Country = country;
        Lat = lat;
        Lon = lon;
    }

    public string? Name { set; get; }
    public string? City { set; get; }
    public string? County { set; get; }

    public string? State { set; get; }
    public string? Postcode { set; get; }
    public string? Country { set; get; }

    public string? Lat {  set; get; }
    public string? Lon { set; get; }
}