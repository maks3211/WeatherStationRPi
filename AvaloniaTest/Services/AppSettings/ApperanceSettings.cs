using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaTest.Helpers;
namespace AvaloniaTest.Services.AppSettings
{
    public partial class ApperanceSettings: ObservableObject
    {
        [ObservableProperty]
        public bool lightTheme;

        [ObservableProperty]
        public bool themeButtonVis;

        //Zmiana na podstawie pory dnia (wybranej recznie lub zgodnie z pora dnia)

        [ObservableProperty]
        public bool useSchduleThemeChange;
        //na podstawie recznie wybranego czasu
        [ObservableProperty]
        public bool customTimeThemeChange;
        [ObservableProperty]
        public TimeSpan? customLightThemeTime;
        [ObservableProperty]
        public TimeSpan? customDarkThemeTime;

        [IgnoreDuringSerialization(true)]  // Użycie atrybutu
        public bool prevLightTheme;



    }
}
