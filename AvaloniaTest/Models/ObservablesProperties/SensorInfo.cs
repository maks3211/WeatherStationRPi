using AvaloniaTest.Helpers;
using AvaloniaTest.Messages;
using AvaloniaTest.Services.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models.ObservablesProperties
{
    public partial class SensorInfo<T> : ObservableObject
    {
        public Func<T, T>? CalculateValue { get; set; } // przelicza 

        public Func<string> GetUnit { get; set; }


        public string Name { get; set; }
        public bool ConvertToInt;

        private bool DisplayUnit;

        [ObservableProperty]
        public string _displayName = "-";

        [ObservableProperty]
        public string _unit = "";

        private bool IsValueSet = false;
        private T _unitValue { get; set; }

        private T _value;
        public T Value
         {
            get => _value;

            set
            {
                
             //   _value = CalculateValue != null ? CalculateValue(value) : value; //przelicz jezli podano funkcje 
             _value = value; //value przechowuje wartosc w domyslnej jednostece

                if (ErrorChecker.IsError(value))
                {
                    return;
                }

              _unitValue = CalculateValue != null ? CalculateValue(value) : value;  //to zawiera juz przeliczonoa wartosc

                var format = _unitValue?.ToString();
                    
                if (format != null)
                {
                    if (GetUnit != null)
                    {
                        if (DisplayUnit)
                        {
                            format = format + GetUnit();
                        } 
                        Unit = GetUnit();
                    }
                    // Zamiana przecinka na kropkę
                    DisplayName = format.Replace(',', '.');
                    if (ConvertToInt)
                    {
                        DisplayName = ConvertToIntegerString(DisplayName);
                    }
                }
                else
                {
                    DisplayName = "-";
                }
                OnPropertyChanged(nameof(Value));
                IsValueSet = true;
            }
        }

        public void Recalculate()
        {
            if(IsValueSet)
            Value = Value;
        }

        private string ConvertToIntegerString(string input)
        {
            // Zamiana przecinka na kropkę, jeśli jest obecny
            string normalizedInput = input.Replace(',', '.');

            // Próba konwersji na double
            if (double.TryParse(normalizedInput, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                // Obcięcie części dziesiętnej i konwersja na string
                value += 0.5;
                int integerValue = (int)value;
                return integerValue.ToString();
            }
            // W przypadku niepowodzenia zwróć pusty string lub obsłuż błąd
            return "";
        }
        public SensorInfo(string name, bool toInt = false ,Func<string> getUnit = null, Func<T, T> calculateValue = null, bool displayUnit = true)
        {
            Name = name;
            ConvertToInt = toInt;
            GetUnit = getUnit;
            CalculateValue = calculateValue;
            DisplayUnit = displayUnit;

            if (getUnit == null)
            {
                DisplayUnit = false;
            }
   
        }

    }
}
