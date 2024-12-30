using AvaloniaTest.Helpers;
using AvaloniaTest.Models.ObservablesProperties;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitsTest
{
    public class SensorInfoTest
    {
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string expectedName = "TestSensor";
            bool expectedToInt = true;

            // Act
            var sensorInfo = new SensorInfo<int>(expectedName, expectedToInt);

            // Assert
            Assert.Equal(expectedName, sensorInfo.Name);
            Assert.True(sensorInfo.ConvertToInt);
        }

        [Fact]
        public void Value_Set_CalculatesAndFormatsValue()
        {
            // Arrange
            var sensorInfo = new SensorInfo<double>(
                name: "Test",
                toInt: false,
                getUnit: () => "X",
                calculateValue: value => value * 2);

            // Act
            sensorInfo.Value = 25.5;

            // Assert
            Assert.Equal("51X", sensorInfo.DisplayName);
        }

        [Theory]
        [InlineData(25.7, "26")]
        [InlineData(25.2, "25")]
        public void ConvertToIntegerString_ConvertsCorrectly(double input, string expected)
        {
            var sensorInfo = new SensorInfo<double>(
            name: "Test",
            toInt: true);
            sensorInfo.Value = input;
            Assert.Equal(expected, sensorInfo.DisplayName);
        }

        [Fact]
        public void Recalculate_RecomputesValue()
        {
            // Arrange
            var sensorInfo = new SensorInfo<double>(
                name: "TestSensor",
                calculateValue: value => value * 2);

            sensorInfo.Value = 10.0;

            // Act
            sensorInfo.Recalculate();

            // Assert
            Assert.Equal("20", sensorInfo.DisplayName);
        }

    }
}
