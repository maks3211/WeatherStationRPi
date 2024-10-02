using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Services.AppSettings
{
    public partial class WeatherSettings : ObservableObject
    {
        [ObservableProperty]
        public int _refreshInterval;
        [ObservableProperty]
        public float _longitude;
        [ObservableProperty]
        public float _latitude;
        [ObservableProperty]
        public string _apiKey;
    }
}
