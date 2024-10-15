using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    public static class ErrorValues
    {
        public static readonly int IntError = -9999; // Błędna wartość dla int
        public static readonly double DoubleError = -9999.0; // Błędna wartość dla double
        public static readonly string StringError = "Error"; // Błędna wartość dla string



        public static T GetErrorValue<T>()
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)IntError;
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)(object)DoubleError;
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)StringError;
            }

            // Dla innych typów
            throw new InvalidOperationException($"Brak zdefiniowanej wartości błędnej dla typu {typeof(T)}");
        }

    }
}
