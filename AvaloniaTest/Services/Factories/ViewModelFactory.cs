using AvaloniaTest.Models;
using AvaloniaTest.Models.ObservablesProperties;
using AvaloniaTest.Models.WeatherForecast;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Services.Factories
{
    public class ViewModelFactory
    {
        public SettingsManager Settings { get; set; }
        public ApperanceSettings ApperanceSettings { get; set; }

        public UnitsSettings UnitSettings { get; set; }
        public TimeProperties TimeProperties { get; set; }
        public OutdoorSensors OutdoorSensors { get; set; }
        public WeatherForecastController WeatherController { get; set; }
        public DataBaseService DataBaseService { get; set; }
     
        public ViewModelBase CreateViewModel(Type viewModelType)
        {        
            if (viewModelType == typeof(HomePageViewModel))
            {
                return new HomePageViewModel(TimeProperties, OutdoorSensors, WeatherController);
            }
            else if (viewModelType == typeof(SettingsViewModel))
            {
                return new SettingsViewModel(this);
            }
            else if (viewModelType == typeof(ChartViewModel))
            {
                return new ChartViewModel();
            }
            else if (viewModelType == typeof(GeneralSettingsViewModel))
            {
                return new GeneralSettingsViewModel(Settings, ApperanceSettings, UnitSettings, OutdoorSensors, WeatherController);
            }
            else if (viewModelType == typeof(NetworkSettingsViewModel))
            {
                return new NetworkSettingsViewModel();
            }
            throw new ArgumentException("Unknown ViewModel type", nameof(viewModelType));
        }


    }
}
