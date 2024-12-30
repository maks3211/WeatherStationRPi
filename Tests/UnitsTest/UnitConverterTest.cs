using AvaloniaTest;
using AvaloniaTest.Services.AppSettings;
using System;
namespace Tests.UnitsTest
{
    public class UnitConverterTest
    {
        UnitsSettings settings;
        UnitsConverter converter;
        public UnitConverterTest()
        {

            settings = new UnitsSettings();
            converter = new UnitsConverter(settings);
        }
        [Fact]
        public void CalculateTemp_ShouldConvertToFahrenheit_WhenUnitIsFahrenheit()
        {
            //Arrange
            settings.Temp = Unit.Fahrenheit;
            double inputTemp = 25.0;
            // Act
            double result = converter.CalculateTemp(inputTemp);
            // Assert
            Assert.Equal(77.0, result); // 25 * 1.8 + 32 = 77
        }


        [Fact]
        public void CalculateTemp_ShouldReturnCelsius_WhenUnitIsCelsius()
        {
            //Arrange
            settings.Temp = Unit.Celsius;
            double inputTemp = 25.0;
            // Act
            double result = converter.CalculateTemp(inputTemp);
            // Assert
            Assert.Equal(25.0, result);
        }
        [Fact]
        public void CalculateWind_ShouldReturnMetersPerSecond_WhenUnitIsMS()
        {
            // Arrange
            var settings = new UnitsSettings { Wind = Unit.MS };
            var converter = new UnitsConverter(settings);
            double inputSpeed = 10.0;

            // Act
            double result = converter.CalculateWind(inputSpeed);

            // Assert
            Assert.Equal(10.0, result); // No conversion
        }

        [Fact]
        public void CalculateWind_ShouldConvertToKilometersPerHour_WhenUnitIsKMH()
        {
            // Arrange
            var settings = new UnitsSettings { Wind = Unit.KMH };
            var converter = new UnitsConverter(settings);
            double inputSpeed = 10.0;

            // Act
            double result = converter.CalculateWind(inputSpeed);

            // Assert
            Assert.Equal(36.0, result); // 10 * 3.6 = 36
        }

        [Fact]
        public void CalculateWind_ShouldConvertToKnots_WhenUnitIsKnots()
        {
            // Arrange
            var settings = new UnitsSettings { Wind = Unit.KT };
            var converter = new UnitsConverter(settings);
            double inputSpeed = 10.0;

            // Act
            double result = converter.CalculateWind(inputSpeed);

            // Assert
            Assert.Equal(5.14, result); // 10 * 0.514 = 5.14
        }


    }
}