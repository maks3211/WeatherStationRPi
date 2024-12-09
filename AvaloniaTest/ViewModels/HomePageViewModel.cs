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
        /// <summary>
        /// Forecast controller to fetch data used in UI
        /// </summary>
        [ObservableProperty]
        public WeatherForecastController _weatherController;
        [ObservableProperty]
        TimeProperties timeProp;
        [ObservableProperty]
        private OutdoorSensors _outdoorSensorsProp;
        [ObservableProperty]
        private IndoorSensors _indoorSensorsProp;
        /// <summary>
        /// Initialize a new instance of <see cref="HomePageViewModel"/> class,
        /// sets up the necessary dependencies for the HomePage view model.
        /// </summary>
        /// <param name="timeProperties"></param>
        /// <param name="outdoorSensors"></param>
        /// <param name="weatherController"></param>
        /// <param name="indoorSensors"></param>
        public HomePageViewModel(TimeProperties timeProperties, OutdoorSensors outdoorSensors, WeatherForecastController weatherController, IndoorSensors indoorSensors)
        {
            OutdoorSensorsProp = outdoorSensors;
            IndoorSensorsProp = indoorSensors;
            TimeProp = timeProperties;
            WeatherController = weatherController;
      
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


       //Wydaje sie byc nie potrzebne, nie ma uzycia RaiseProperty
       /* 
       public event PropertyChangedEventHandler PropertyChanged;
       private void RaisePropertyChanged(string propertyName)
        {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
       }
       */

        /// <summary>
        /// Handles the click event of the "Refresh Forecast" button.
        /// Calls the GetAllData method from the WeatherController to fetch new weather forecast data.
        /// </summary>
        [RelayCommand]
        public void OnRefreshForecastClicked()
        {
            WeatherController.GetAllData();
        }
    }
}