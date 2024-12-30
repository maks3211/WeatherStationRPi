using AvaloniaTest.Helpers;
using AvaloniaTest.Models;
using AvaloniaTest.Services.AppSettings;
using AvaloniaTest.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaTest.Interfaces;
using Avalonia.Controls;
using System.ComponentModel;

namespace Tests.UnitsTest
{
    public class IndoorSensorsTest
    {
        Mock<IDataBaseService> databaseMock;
        UnitsSettings unitsSet;
        UnitsConverter unitsConv;

        

        public IndoorSensorsTest()
        {
            databaseMock = new Mock<IDataBaseService>();
            unitsSet =new UnitsSettings();
            unitsConv =new UnitsConverter(unitsSet);
           
        }


        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {    
           
            // Act
            var indoorSensors = new IndoorSensors(databaseMock.Object, unitsSet, unitsConv);

            // Assert
            Assert.NotNull(indoorSensors.Temperature);
            Assert.Equal(ErrorValues.DoubleError, indoorSensors.Temperature.Value);
            Assert.NotNull(indoorSensors.Pressure);
            Assert.NotNull(indoorSensors.Humidity);
            Assert.NotNull(indoorSensors.CO);
            Assert.NotNull(indoorSensors.MinTemp);
            Assert.NotNull(indoorSensors.MaxTemp);
            Assert.NotNull(indoorSensors.LastTemp);
        }

        [Fact]
        public void OnTemperatureChanged_ShouldUpdateMinMaxAndLastTemp()
        {
      

            databaseMock.Setup(db => db.GetValueFrom24HoursAgo<double>(It.IsAny<string>()))
                        .Returns(20.0);

            var indoorSensors = new IndoorSensors(databaseMock.Object, unitsSet, unitsConv);

            // Act
            indoorSensors.Temperature.Value = 25.0;

            // Assert
            Assert.Equal(25.0, indoorSensors.MaxTemp.Value);
            Assert.Equal(20.0, indoorSensors.LastTemp.Value);
        }

        [Fact]
        public void Unit_PropertyChanged_ShouldRecalculateTemperatureValues()
        {
            // Arrange
            //  var unitsSettingsw = new UnitsSettings(); // Rzeczywista instancja
            unitsSet.Temp = Unit.Celsius;
            //var unitsConverterw = new UnitsConverter(unitsSettingsw); // Rzeczywista instancja


            var indoorSensors = new IndoorSensors(databaseMock.Object, unitsSet, unitsConv);
            double tempInCelsius = 10;
            indoorSensors.Temperature.Value = tempInCelsius;
            // unitsSettingsw.Temp = Unit.Fahrenheit;
            unitsSet.Temp = Unit.Fahrenheit;

            double expectedFahrenheit = Math.Round(10 * 1.8 + 32, 2);
            Assert.Equal(expectedFahrenheit.ToString() + "°F", indoorSensors.Temperature.DisplayName);


        }
    }
}
