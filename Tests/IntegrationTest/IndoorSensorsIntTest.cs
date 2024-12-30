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
using AvaloniaTest.Helpers;

namespace Tests.IntegrationTest
{
    public class IndoorSensorsIntTest
    {
        Mock<IDataBaseService> databaseMock;
        UnitsSettings unitsSet;
        UnitsConverter unitsConv;
        public IndoorSensorsIntTest()      
        {
            databaseMock = new Mock<IDataBaseService>();
            unitsSet = new UnitsSettings();
            unitsConv = new UnitsConverter(unitsSet);
        }

        [Fact]
        public void SetLastTemp_ShouldCallDatabaseService()
        {
            // Arrange     
            databaseMock.Setup(db => db.GetValueFrom24HoursAgo<double>("innerTemperature"))
                        .Returns(15.0);

            
            var indoorSensors = new IndoorSensors(databaseMock.Object, unitsSet, unitsConv);

            // Assert
            databaseMock.Verify(db => db.GetValueFrom24HoursAgo<double>("innerTemperature"), Times.Once);
            Assert.Equal(15.0, indoorSensors.LastTemp.Value);
        }

        [Fact]
        public void OnHumidityChanged_ShouldUpdateHumidityCircle()
        {
            // Arrange


            var indoorSensors = new IndoorSensors(databaseMock.Object, unitsSet, unitsConv);

            // Act
            indoorSensors.Humidity.Value = 50.0;

            // Assert
            Assert.Equal(GraphicsGauges.GetCircleGaugeValue(50.0), indoorSensors.Humiditycircle);
        }


        [Fact]
        public void Constructor_ShouldInitializeAndCallSetMethods()
        {
            // Arrange
          
            databaseMock.Setup(db => db.GetTodayMinMaxValue<double>("innerTemperature"))
                        .Returns((10.0, 30.0));

           


            var indoorSensors = new IndoorSensors(databaseMock.Object, unitsSet, unitsConv);

            // Act
            indoorSensors.Temperature.Value = 25.0;

            // Assert
            Assert.Equal(10.0, indoorSensors.MinTemp.Value);
            Assert.Equal(30.0, indoorSensors.MaxTemp.Value);
            databaseMock.Verify(db => db.GetTodayMinMaxValue<double>("innerTemperature"), Times.Exactly(2));
        }
    }
}
