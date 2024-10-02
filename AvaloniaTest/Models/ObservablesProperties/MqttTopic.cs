using AvaloniaTest.Messages;
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
    public partial class MqttTopic<T> : ObservableObject
    {
        public Func<T, T>? CalculateValue { get; set; } // przelicza 

        public Func<string> GetUnit { get; set; }


        public string Name { get; set; }
        public bool ConvertToInt;


        [ObservableProperty]
        public string _displayName = "-";

        private bool IsValueSet = false;
        private T _unitValue { get; set; }

        private T _value;
        public T Value
         {
            get => _value;

            set
            {
                Console.WriteLine("jest setter w mqqt topic");
                
             //   _value = CalculateValue != null ? CalculateValue(value) : value; //przelicz jezli podano funkcje 
             _value = value; //value przechowuje wartosc w domyslnej jednostece
              _unitValue = CalculateValue != null ? CalculateValue(value) : value;  //to zawiera juz przeliczonoa wartosc

                var format = _unitValue?.ToString();

                if (format != null)
                {
                    if (GetUnit != null)
                    {
                        format = format + GetUnit();
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
        public MqttTopic(string name, bool toInt = false ,Func<string> getUnit = null, Func<T, T> calculateValue = null)
        {
            Name = name;
            ConvertToInt = toInt;
            //  FormatDisplayName = formatDisplayName;
            GetUnit = getUnit;
            CalculateValue = calculateValue;
            if (calculateValue != null)
            {
                WeakReferenceMessenger.Default.Register<UnitChangedMessage>(this, (r, m) =>
                {
                    if (m.Value == true)
                    {
                        Console.WriteLine("otrzymano wiodmosc w mqqt topic");
                        Recalculate();
                    }

                });
            }

        }

    }
}
