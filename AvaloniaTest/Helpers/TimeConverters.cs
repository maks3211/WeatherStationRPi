using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AvaloniaTest.Models.WeatherForecast.WeatherForecast;

namespace AvaloniaTest.Helpers
{
    public static class TimeConverters
    {
        public static string ConvertDateTimeToDay(long millisec, long timezone)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(millisec);
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromSeconds(timezone));
            string formattedDate = dateTimeOffset.Day.ToString("D2");
            return formattedDate;
        }

        public static string ConvertDateTimeToHour(long millisec, long timezone)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(millisec);
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromSeconds(timezone));
            int hour = dateTimeOffset.Hour;
            return hour.ToString("D2");
        }


        public static TimeSpan ConvertDateTimeToHourMinute(long millisec, long timezone)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(millisec);
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromSeconds(timezone));
            return new TimeSpan(dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second);
        }


        public static DayOfWeek ConvertDateTimeToDayOfWeek(long millisec, long timezone)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(millisec);
            dateTimeOffset = dateTimeOffset.ToOffset(TimeSpan.FromSeconds(timezone));
          
           // string dayOfWeek = dateTimeOffset.ToString("dddd", new CultureInfo("pl-PL"));
            return dateTimeOffset.DayOfWeek;
        }



        public static string ConvertDayOfWeekToShort(DayOfWeek day)
        {
            string shortDayOfWeek = day switch
            {
                DayOfWeek.Monday => "PN",
                DayOfWeek.Tuesday => "WT",
                DayOfWeek.Wednesday => "ŚR",
                DayOfWeek.Thursday => "CZ",
                DayOfWeek.Friday => "PT",
                DayOfWeek.Saturday => "SB",
                DayOfWeek.Sunday => "ND",
                _ => "UNKNOWN"
            };

            return shortDayOfWeek;
        }
    }
}
