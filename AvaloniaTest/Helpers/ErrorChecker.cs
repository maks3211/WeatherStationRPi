using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    public static class ErrorChecker
    {
        public static bool IsError<T>(T value)
        {
            if (typeof(T) == typeof(int))
            {
                return EqualityComparer<T>.Default.Equals(value, (T)(object)ErrorValues.IntError);
            }
            else if (typeof(T) == typeof(double))
            {
                return EqualityComparer<T>.Default.Equals(value, (T)(object)ErrorValues.DoubleError);
            }
            else if (typeof(T) == typeof(string))
            {
                return EqualityComparer<T>.Default.Equals(value, (T)(object)ErrorValues.StringError);
            }

            // Dla innych typów
            return false;
        }
    }
}
