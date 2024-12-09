using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Avalonia.Controls.Converters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using MySql.Data.MySqlClient;
using Google.Protobuf.WellKnownTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AvaloniaTest.Models.ObservablesProperties;
using Newtonsoft.Json.Linq;
using AvaloniaTest.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Mysqlx.Notice;
using AvaloniaTest.Services.Enums;
using AvaloniaTest.Helpers;
using AvaloniaTest.Services;



namespace AvaloniaTest.Models
{
    public class MQTTcommunication
    {

      

   

        private OutdoorSensors sensors;

        public void AddSensros(OutdoorSensors s)
        {
            sensors = s;
        }

        public static bool IsConnected = false;
        public static string OutDoorTEMPERATURE = "";



        public double OutDoorTemp = -ErrorValues.GetErrorValue<double>();
        public double OutDoorPres = -ErrorValues.GetErrorValue<double>();
        public int    OutDoorAlti = -ErrorValues.GetErrorValue<int>();
        public double OutDoorHumi = -ErrorValues.GetErrorValue<double>();

        public double OutDoorLumi = ErrorValues.GetErrorValue<double>();
        public double OutDoorNO2 = ErrorValues.GetErrorValue<double>();
        public double OutDoorCO =  ErrorValues.GetErrorValue<double>();
        public double OutDoorNH3 = ErrorValues.GetErrorValue<double>();

        public double OutDoorWind = ErrorValues.GetErrorValue<double>();
        public double OutDoorWindAngle = ErrorValues.GetErrorValue<double>();

        DateTime currentDateTime;
        MySqlConnection con;
        MySqlCommand cmd;

        private DataBaseService database;
        public MQTTcommunication(Services.DataBaseService database)
        {
            this.database = database;
        }
     
        public async Task Start_Server()
        {
            //string topic = "test";
            Console.WriteLine("serwer start");
            //string broker = "192.168.0.91"; //192.168.0.91
           // int port = 1883;
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("7b21c793398043ab8fbde110f0ebc243.s1.eu.hivemq.cloud", 8883) //testuseR1
            .WithCredentials("testuser1", "testuseR1")
            .WithTls()
            .Build();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            

            Console.WriteLine($"mqqt Connected: {mqttClient.IsConnected.ToString()}");

            var timeoutToken =  new CancellationTokenSource(TimeSpan.FromSeconds(10));
          //  try
          // {
           //     //moze wywalic await wtedy przechodzi dalej i w petli sprawdzac czy jest connected - analognicznie jak na esp
          //      mqttClient.ConnectAsync(options, timeoutToken.Token);
          //  }
          //  catch (OperationCanceledException) {
          //      Console.WriteLine("TIMEOUIT po laczeniu");
          //  }


            //mqttClient.ConnectAsync(options, timeoutToken.Token);

            //  Console.WriteLine(a.ResultCode);

          
            while (!IsConnected)
            {
                await  mqttClient.ConnectAsync(options);
                Console.WriteLine($"stan mqtt: {mqttClient.IsConnected}");
                await Task.Delay(2000);
                IsConnected = mqttClient.IsConnected;
                
            }
            if (IsConnected)
            {
                Console.WriteLine($"stan mqqt: {mqttClient.IsConnected}");
            }
            var oTemp = await mqttClient.SubscribeAsync("outdoortemperature");
            var OPres = await mqttClient.SubscribeAsync("outdoorpreasure");
            var oAlti = await mqttClient.SubscribeAsync("outdooraltitude");
            var oHumi = await mqttClient.SubscribeAsync("outdoornhumidity");
            var oIlumi = await mqttClient.SubscribeAsync("outdooriluminance");

            var oNo = await mqttClient.SubscribeAsync("outdoorno2");
            var oNh = await mqttClient.SubscribeAsync("outdoornh3");
            var oCo = await mqttClient.SubscribeAsync("outdoorco");
            var wind = await mqttClient.SubscribeAsync("outdoorwind");
            var windangle = await mqttClient.SubscribeAsync("outdoorwindangle");
            await mqttClient.SubscribeAsync("outdoorrain");
            await mqttClient.SubscribeAsync("espSsid");
            await mqttClient.SubscribeAsync("espStrength");


            //WYSYLANIE WIADOMOSCI - NIE TESTOWANE
            //  var message = new MqttApplicationMessageBuilder()
            //  .WithTopic("test")
            //  .WithPayload("Hello World")          
            //   .WithRetainFlag()     
            //  .Build();
            // await mqttClient.PublishAsync(message, CancellationToken.None);

            mqttClient.DisconnectedAsync += async e =>
            {
                if (e.ClientWasConnected)
                {
                    // Use the current options as the new options.
                    Console.WriteLine("rozolczone");
                    IsConnected = false;
                    while (!IsConnected)
                    {
                        Console.WriteLine("proba ponownego laczenia");
                        await mqttClient.ConnectAsync(options);
                        Console.WriteLine($"stan: {mqttClient.IsConnected}");
                        await Task.Delay(2000);
                        IsConnected = mqttClient.IsConnected;

                    }
                    if (IsConnected)
                    {
                        Console.WriteLine("Polaczono z mqtt");
                      //  var oTemp = await mqttClient.SubscribeAsync("outdoortemperature");
                      //  
                      //  var OPres = await mqttClient.SubscribeAsync("outdoorpreasure");
                      //  var oAlti = await mqttClient.SubscribeAsync("outdooraltitude");
                      //  var oHumi = await mqttClient.SubscribeAsync("outdoornhumidity");
                      //  var oIlumi = await mqttClient.SubscribeAsync("outdooriluminance");
                      //
                      //  var oNo = await mqttClient.SubscribeAsync("outdoorno2");
                      //  var oNh = await mqttClient.SubscribeAsync("outdoornh3");
                      //  var oCo = await mqttClient.SubscribeAsync("outdoorco");
                      //  var wind = await mqttClient.SubscribeAsync("outdoorwind");
                      //  var windangle = await mqttClient.SubscribeAsync("outdoorwindangle");
                      //  await mqttClient.SubscribeAsync("espSsid");
                      //  await mqttClient.SubscribeAsync("espStrength");
                    }
                    //await mqttClient.ConnectAsync(mqttClient.Options, cancellationToken);
                }
            };

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
               // Console.WriteLine($"tu jest:  {e.ReasonCode}");
            };


            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                 Console.WriteLine("Received application message.");
                 Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                // Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                // Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

                currentDateTime = DateTime.Now;
                switch (e.ApplicationMessage.Topic)
                {
                    case "outdoortemperature":                 
                        OutDoorTemp = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.Temperature.Value= OutDoorTemp;
                        //InsertDataIntoTable("outerTemperature", currentDateTime, OutDoorTemp);
                        database.InsertDataIntoTable("outerTemperature", currentDateTime, OutDoorTemp);
                        break;
                    case "outdoorpreasure":
                           Console.WriteLine($"+ Cisnienie = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        OutDoorPres = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.Pressure.Value = OutDoorPres;
                        // InsertDataIntoTable("outerPreasure", currentDateTime, (int)OutDoorPres);
                        database.InsertDataIntoTable("outerPreasure", currentDateTime, (int)OutDoorPres);
                       
                        break;
                    case "outdooraltitude":
                        string result = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                         Console.WriteLine($"+ Wysokosc = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        if (result.Contains(".") || result.Contains(","))
                        {
                            string[] parts = result.Split('.', ',');
                            result = parts[0];      
                        }
                         OutDoorAlti = int.Parse(result);
                        sensors.Altitude.Value = OutDoorAlti;
                        // InsertDataIntoTable("outerAltitude", currentDateTime, OutDoorAlti);
                        database.InsertDataIntoTable("outerAltitude", currentDateTime, OutDoorAlti);
                        break;
                    case "outdoornhumidity":
                        OutDoorHumi = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        // OutdoorHumiUpdated?.Invoke(this, OutDoorHumi);
                        sensors.Humidity.Value = OutDoorHumi;
                        // InsertDataIntoTable("outerHumidity", currentDateTime, OutDoorHumi);
                        database.InsertDataIntoTable("outerHumidity", currentDateTime, OutDoorHumi);
                        break;
                    case "outdooriluminance":
                        OutDoorLumi = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.Illuminance.Value = OutDoorLumi;
                        //  InsertDataIntoTable("outerLuminance", currentDateTime, OutDoorLumi);
                        database.InsertDataIntoTable("outerLuminance", currentDateTime, OutDoorLumi);
                        break;
                    case "outdoorno2":
                        OutDoorNO2 = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.NO2.Value = OutDoorNO2;
                        // InsertDataIntoTable("outerNo2", currentDateTime, OutDoorNO2);
                        database.InsertDataIntoTable("outerNo2", currentDateTime, OutDoorNO2);
                        break;
                    case "outdoorco":
                        OutDoorCO = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.CO.Value = OutDoorCO;
                        database.InsertDataIntoTable("outerCo", currentDateTime, OutDoorCO);
                       // InsertDataIntoTable("outerCo", currentDateTime, OutDoorCO);
                        break;
                    case "outdoornh3":
                        OutDoorNH3 = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.NH3.Value = OutDoorNH3;
                        // InsertDataIntoTable("outerNh3", currentDateTime, OutDoorNH3);
                        database.InsertDataIntoTable("outerNh3", currentDateTime, OutDoorNH3);
                        break;
                    case "outdoorwind":
                        OutDoorWind = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.Wind.WindSpeed.Value = OutDoorWind;
                        break;
                    case "outdoorwindangle":
                        OutDoorWindAngle = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        sensors.Wind.Angle = OutDoorWindAngle;
                        break;
                    case "outdoorrain":
                        sensors.Rain.Value = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        break;

                    case "espSsid":
                        ESPnetworkData.GetInstance().Ssid = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                        ESPnetworkData.GetInstance().UpdateLastMessageTime();
                        break;
                    case "espStrength":
                        ESPnetworkData.GetInstance().Strength = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        ESPnetworkData.GetInstance().UpdateLastMessageTime();
                        break;
                }

                
                return Task.CompletedTask;
            };

          //  await mqttClient.ConnectAsync(options, CancellationToken.None);

         

            //await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());

            // Ustaw funkcję zwrotną dla otrzymanych wiadomości
            //mqttClient.ApplicationMessageReceived += (s, e) =>
            // {
            //    Console.WriteLine($"Otrzymano wiadomość w temacie '{e.ApplicationMessage.Topic}': {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //};

        }

        private double ConvertToDouble(ArraySegment<byte> value)
        {
            string i = Encoding.UTF8.GetString(value);
            double result = -ErrorValues.GetErrorValue<double>();
            try
            {
                result = double.Parse(i);
            }
            catch (FormatException)
            {
                string a = i.Replace(".", ",");
                result = double.Parse(a);
            }
            return result;    
        }
    }

  
}
