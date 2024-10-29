using AvaloniaTest.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Services.AppSettings
{


    public static class Unit
    {
        public const string Celsius = "°C";
        public const string Fahrenheit = "°F";
        public const string MS = "m/s";
        public const string KMH = "km/h";
        public const string KT = "kt";
        public const string mm = "mm";
        public const string liter = "l";

    }

    public partial class UnitsSettings : ObservableObject
    {


        [ObservableProperty]
        public string _temp;

        [ObservableProperty]
        public string _wind;

        [ObservableProperty]
        public string _rain;
    }
}