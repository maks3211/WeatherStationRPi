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
//using Newtonsoft.Json.Linq;
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
    private SettingsManager settingsManager;
    [ObservableProperty]
    public ApperanceSettings _apperanceSettings;
    [ObservableProperty]
    public UnitsSettings _unitSettings;
    [ObservableProperty]
    private WeatherForecastController _weatherController;
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

        //Rejestrowanie otwarca/ zamkniecia widoku
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
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    /*private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }*/


    AddressSearchController AddressSearchController = new AddressSearchController();
    [ObservableProperty]
    private int _spacerHeight = 310;
    [ObservableProperty]
    private int _weatherUpdateInterval;
    [ObservableProperty]
    private CitySearchListTemplate? _selectedCity;
    [ObservableProperty]
    public string _citySearchText;

    public ObservableCollection<CitySearchListTemplate> CityList { get; } = new();
  
    partial void OnSelectedCityChanged(CitySearchListTemplate? value)
    {

        if (value is null)
        {
            return;
        }
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
        WeatherController.SetNewerCitys(longitude, latitude);

    }



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
                    CityList.Add(new CitySearchListTemplate(
                    a.address.name.AddComma(),
                    a.address.city.AddComma(),
                    a.address.county.AddComma(),
                    a.address.state.AddComma(),
                    a.address.postcode.AddComma(),
                    a.address.country,
                    a.lat, a.lon));
                }
                CitySearchText = "";
            }
            else
            {
                SpacerHeight = 310;
            }
        }
    }


    [RelayCommand]
    public async void AutoThemeCommand()
    {
        var set = WeatherController.GetDailyForecastdwa().list[0].sunset;
        var rise = WeatherController.GetDailyForecastdwa().list[0].sunrise;
        var timeZone = WeatherController.TimeZone;

        var morning = TimeConverters.ConvertDateTimeToHourMinute(rise, timeZone);
        ApperanceSettings.CustomLightThemeTime = morning;
        var night = TimeConverters.ConvertDateTimeToHourMinute(set, timeZone);
        ApperanceSettings.CustomDarkThemeTime = night; // ?? TimeSpan.FromHours(18) + TimeSpan.FromMinutes(30);


       // WeakReferenceMessenger.Default.Send(new AutoThemeMessage(true, morning, night));
        //CustomThemeTime = morning.ToString(@"hh\:mm") + " - " + night.ToString(@"hh\:mm"); 
        //zapisanie ze auto 
    }

    [RelayCommand]
    public async void ThemeButtonVisSwitch()
    {
        WeakReferenceMessenger.Default.Send(new ThemeBtnVisMessage(ApperanceSettings.ThemeButtonVis));
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

    private async void SaveSettings()
    {
        WeakReferenceMessenger.Default.Send(new AutoThemeMessage(ApperanceSettings.UseSchduleThemeChange));
        await settingsManager.SaveSettingsAsync("Appearance", ApperanceSettings);
        Console.WriteLine($"W pogodzie: {WeatherController.UpdateIntervalMinuts}");
        Console.WriteLine($"W ustawieniach: {WeatherUpdateInterval}");
        Console.WriteLine(WeatherController.AutoWeatherRefresh);
        if (!WeatherController.AutoWeatherRefresh)
        {
            WeatherController.UpdateIntervalMinuts = 0;
        }
        else
        {
            WeatherController.UpdateIntervalMinuts = WeatherUpdateInterval;
        }
        Console.WriteLine(WeatherController.UpdateIntervalMinuts);
        Console.WriteLine(!WeatherController.AutoWeatherRefresh);
        await settingsManager.SaveSettingsAsync("Units", UnitSettings);
        await WeatherController.SaveSettings();
    }



    public async Task UpdateCitySuggestions()
    {    
        if (CityInput.Length < 3)
        {
            return;
        }
        if (CityInput.Length - lastCityImput.Length < 5)
        {
            if (CityInput.Length - lastCityImput.Length < -5)
            {
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
        var resuts = await AddressSearchController.Search(CityInput);
        if (resuts != null)
        {
           
            Console.WriteLine(resuts.Count);
            CitySuggestion.Clear();

            foreach (var a in resuts)
            {
                CitySuggestion.Add(a.display_address);
            }
        }
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