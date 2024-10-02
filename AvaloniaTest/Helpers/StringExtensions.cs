using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static bool FirstCharVary(this string input, string compare)
        {         
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(compare))
            {
                throw new ArgumentException("Invalid input - null or empty");
            }
                return input[0] == compare[0];
        }

        public static bool LastNCharsDiffer(this string string1, string string2, int n)
        {
            if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2) || n < 0)
            {
                throw new ArgumentException("Invalid input - string is null or empty");
            }    
            if (n < 0)
            {
                throw new ArgumentException("Invalid input - n must be non-negative.");
            }

            int length1 = string1.Length;
            int length2 = string2.Length;
            int toCompare = Math.Min(n, Math.Min(length1, length2));
            string lastNCharsStr1 = string1.Substring(string1.Length - toCompare);
            string lastNCharsStr2 = string2.Substring(string2.Length - toCompare);
            return !lastNCharsStr1.Equals(lastNCharsStr2);
        }

        public static bool FirstNCharsDiffer(this string string1, string string2, int n)
        {

            int length1 = string1.Length;
            int length2 = string2.Length;
            int toCompare = Math.Min(n, Math.Min(length1, length2));
            string substring1 = string1.Substring(0, n);
            string substring2 = string2.Substring(0, n);
            return substring1 != substring2;
        }


        public static string AddComma(this string s)
        {
            if (s == null)
                return s;
            return s + ",";
        }
    }
}