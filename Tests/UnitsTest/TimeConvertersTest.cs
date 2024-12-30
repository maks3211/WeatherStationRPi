using AvaloniaTest.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitsTest
{
    public class TimeConvertersTest
    {

        [Theory]
        [InlineData(1734433200, -46800, "16")] // 2024-12-16, UTC-1
        [InlineData(1734433200, 0, "17")] //  2024-12-17, UTC
        [InlineData(1734433200, 46800, "18")] // 2024-12-18, UTC+1
        public void ConvertDateTimeToDay_ReturnsCorrectDay(long millisec, long timezone, string expectedDay)
        {
            // Act
            var result = TimeConverters.ConvertDateTimeToDay(millisec, timezone);

            // Assert
            Assert.Equal(expectedDay, result);
        }


        [Theory]
        [InlineData(1734433200, -3600, "10")]
        [InlineData(1734433200, 0, "11")] 
        [InlineData(1734433200, 3600, "12")] 
        public void ConvertDateTimeToHour_ReturnsCorrectHour(long millisec, long timezone, string expectedHour)
        {
            // Act
            var result = TimeConverters.ConvertDateTimeToHour(millisec, timezone);

            // Assert
            Assert.Equal(expectedHour, result);
        }

        [Theory]
        [InlineData(1734433200, -46800, DayOfWeek.Monday)] 
        [InlineData(1734433200, 0, DayOfWeek.Tuesday)]
        [InlineData(1734433200, 46800, DayOfWeek.Wednesday)] 
        public void ConvertDateTimeToDayOfWeek_ReturnsCorrectDayOfWeek(long millisec, long timezone, DayOfWeek expectedDayOfWeek)
        {
            // Act
            var result = TimeConverters.ConvertDateTimeToDayOfWeek(millisec, timezone);

            // Assert
            Assert.Equal(expectedDayOfWeek, result);
        }

        [Theory]
        [InlineData(DayOfWeek.Monday, "PN")]
        [InlineData(DayOfWeek.Tuesday, "WT")]
        [InlineData(DayOfWeek.Wednesday, "ŚR")]
        [InlineData(DayOfWeek.Thursday, "CZ")]
        [InlineData(DayOfWeek.Friday, "PT")]
        [InlineData(DayOfWeek.Saturday, "SB")]
        [InlineData(DayOfWeek.Sunday, "ND")]
        public void ConvertDayOfWeekToShort_ReturnsCorrectShortDay(DayOfWeek day, string expectedShortDay)
        {
            // Act
            var result = TimeConverters.ConvertDayOfWeekToShort(day);

            // Assert
            Assert.Equal(expectedShortDay, result);
        }
    }
}
