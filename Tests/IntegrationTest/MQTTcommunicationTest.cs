using AvaloniaTest.Interfaces;
using AvaloniaTest.Models;
using AvaloniaTest.Services;
using AvaloniaTest.Services.AppSettings;
using Moq;
using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using MQTTnet.Server;
using MQTTnet.Adapter;


namespace Tests.IntegrationTest
{
    public class MQTTcommunicationTest
    {

        Mock<IDataBaseService> databaseMock;
        OutdoorSensors outdoorSensors;
        UnitsSettings unitsSet;
        UnitsConverter unitsConv;
        private readonly ITestOutputHelper output;
        public MQTTcommunicationTest(ITestOutputHelper output)
        {
            this.output = output;
            databaseMock = new Mock<IDataBaseService>();
            unitsSet = new UnitsSettings();
            unitsConv = new UnitsConverter(unitsSet);
            outdoorSensors = new OutdoorSensors(databaseMock.Object, unitsSet, unitsConv);
        }

        [Fact]
        public async Task ConnectToBroker_ShouldEstablishConnection()
        {
            // Arrange
            var mqttCommunication = new MQTTcommunication(databaseMock.Object);
            mqttCommunication.AddSensros(outdoorSensors);

            // Act
            var connectTask = mqttCommunication.Start_Server();
            await Task.Delay(5000); // Wait for connection

            // Assert
            Assert.True(MQTTcommunication.IsConnected, "MQTT client should be connected.");
        }

        [Fact]
        public async Task SubscribeToTopics_ShouldReceiveMessages()
        {
            // Arrange
            var mqttCommunication = new MQTTcommunication(databaseMock.Object);
            mqttCommunication.AddSensros(outdoorSensors);

            await mqttCommunication.Start_Server();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("7b21c793398043ab8fbde110f0ebc243.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("testuser1", "testuseR1")
                .WithTls()
                .Build();
            await mqttClient.ConnectAsync(options);

            // Act
            var payload = Encoding.UTF8.GetBytes("25.5");
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("outdoortemperature")
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);

            // Wait for the message to be processed
            await Task.Delay(2000);

            Assert.Equal(25.5, outdoorSensors.Temperature.Value);
            databaseMock.Verify(d => d.InsertDataIntoTable("outerTemperature", It.IsAny<DateTime>(), 25.5), Times.Once);
        }

      


    }
}
