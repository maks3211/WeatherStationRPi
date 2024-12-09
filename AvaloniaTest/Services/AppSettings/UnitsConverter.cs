using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Services.AppSettings
{
    public partial class UnitsConverter : ObservableObject
    {
        [ObservableProperty]
        private UnitsSettings _settings;

        public UnitsConverter(UnitsSettings settings)
        {
            Settings = settings;
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("jakas zmiana w unitsConverter - ale nie musze tutaj o tym wiedziec: ");
            Console.WriteLine(e.PropertyName);
        }


        public double CalculateTemp(double temp)
        {
            if (Settings.Temp == Unit.Fahrenheit)
            {
                return Math.Round(temp * 1.8 + 32, 2);
            }
            return Math.Round(temp, 2);
        }

        public double CalculateWind(double speed)
        {
            if (Settings.Wind == Unit.MS)
            {

                return Math.Round(speed, 2);
            }
            if (Settings.Wind == Unit.KMH)
            {
   
                return Math.Round(speed * 3.6, 2);
            }

            return Math.Round(speed * 0.514, 2);
        }



    }
}
