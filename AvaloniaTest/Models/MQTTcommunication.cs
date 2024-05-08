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



namespace AvaloniaTest.Models
{
    public class MQTTcommunication
    {
        public static bool IsConnected = false;
        public static string OutDoorTEMPERATURE = "";


        public event EventHandler<double> OutdoorTempUpdated;
        public event EventHandler<double> OutdoorPresUpdated;
        public event EventHandler<int> OutdoorAltiUpdated;
        public event EventHandler<double> OutdoorHumiUpdated;
        public event EventHandler<double> OutdoorLumiUpdated;
        public event EventHandler<double> OutdoorNO2Updated;
        public event EventHandler<double> OutdoorCOUpdated;
        public event EventHandler<double> OutdoorNH3Updated;

        public double OutDoorTemp = -999.0;
        public double OutDoorPres = -999.0;
        public int    OutDoorAlti = -999;
        public double OutDoorHumi = -999.0;
       
        public double OutDoorLumi = -99.0;
        public double OutDoorNO2 = -99.0;
        public double OutDoorCO = -99.0;
        public double OutDoorNH3 = -99.0;


        DateTime currentDateTime;
        MySqlConnection con;
        MySqlCommand cmd;
        public MQTTcommunication()
        {
            try
            {
                string connString = "server=sql11.freesqldatabase.com ; uid=sql11704729 ; pwd=89jVjCtqzd ; database=sql11704729";
                con = new MySqlConnection();
                con.ConnectionString = connString;
                con.Open();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to the database: " + ex.Message);
            }
        }


        private void InsertDataIntoTable(string tableName, DateTime date, double value)
        {
            try
            {
                string insertQuery = $"INSERT INTO {tableName} (date, {tableName}) VALUES (@date, @value)";
                cmd = new MySqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while inserting data into the database: " + ex.Message);
            }
        }
        //public static
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
            

            Console.WriteLine(mqttClient.IsConnected.ToString());
            Console.WriteLine("PRZED  ");
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
                mqttClient.ConnectAsync(options);
                Console.WriteLine($"stan mqtt: {mqttClient.IsConnected}");
                await Task.Delay(2000);
                IsConnected = mqttClient.IsConnected;
                
            }
            if (IsConnected)
            {
                Console.WriteLine($"POLOCZONO: {mqttClient.IsConnected}");
            }
            var oTemp = await mqttClient.SubscribeAsync("outdoortemperature");
            var OPres = await mqttClient.SubscribeAsync("outdoorpreasure");
            var oAlti = await mqttClient.SubscribeAsync("outdooraltitude");
            var oHumi = await mqttClient.SubscribeAsync("outdoornhumidity");
            var oIlumi = await mqttClient.SubscribeAsync("outdooriluminance");

            var oNo = await mqttClient.SubscribeAsync("outdoorno2");
            var oNh = await mqttClient.SubscribeAsync("outdoornh3");
            var oCo = await mqttClient.SubscribeAsync("outdoorco");


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
                        mqttClient.ConnectAsync(options);
                        Console.WriteLine($"stan: {mqttClient.IsConnected}");
                        await Task.Delay(2000);
                        IsConnected = mqttClient.IsConnected;

                    }
                    if (IsConnected)
                    {
                        Console.WriteLine("Polaczono z mqtt");
                        var oTemp = await mqttClient.SubscribeAsync("outdoortemperature");
                        var OPres = await mqttClient.SubscribeAsync("outdoorpreasure");
                        var oAlti = await mqttClient.SubscribeAsync("outdooraltitude");
                        var oHumi = await mqttClient.SubscribeAsync("outdoornhumidity");
                        var oIlumi = await mqttClient.SubscribeAsync("outdooriluminance");

                        var oNo = await mqttClient.SubscribeAsync("outdoorno2");
                        var oNh = await mqttClient.SubscribeAsync("outdoornh3");
                        var oCo = await mqttClient.SubscribeAsync("outdoorco");
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
                // Console.WriteLine("Received application message.");
                // Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                // Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                // Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

                currentDateTime = DateTime.Now;
                switch (e.ApplicationMessage.Topic)
                {
                    case "outdoortemperature":
                        Console.WriteLine($"+ Temperatura = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        // OutDoorTEMPERATURE = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                        OutDoorTemp = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorTempUpdated?.Invoke(this, OutDoorTemp);
                        InsertDataIntoTable("outerTemperature", currentDateTime, OutDoorTemp);
                        break;
                    case "outdoorpreasure":
                           Console.WriteLine($"+ Cisnienie = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        OutDoorPres = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorPresUpdated?.Invoke(this, OutDoorPres);
                        InsertDataIntoTable("outerPreasure", currentDateTime, OutDoorPres);
                        break;
                    case "outdooraltitude":
                        string result = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                         Console.WriteLine($"+ Wysokosc = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        // OutDoorAlti = ConvertToInt(e.ApplicationMessage.PayloadSegment);

                        if (result.Contains(".") || result.Contains(","))
                        {
                            string[] parts = result.Split('.', ',');
                            result = parts[0];      
                        }
                         OutDoorAlti = int.Parse(result);
                         OutdoorAltiUpdated?.Invoke(this, OutDoorAlti);
                        InsertDataIntoTable("outerAltitude", currentDateTime, OutDoorAlti);
                        break;
                    case "outdoornhumidity":
                        // Console.WriteLine($"+ Wilgotnosc = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        OutDoorHumi= ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorHumiUpdated?.Invoke(this, OutDoorHumi);
                        InsertDataIntoTable("outerHumidity", currentDateTime, OutDoorHumi);
                        break;
                    case "outdooriluminance":
                        OutDoorLumi = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorLumiUpdated?.Invoke(this, OutDoorLumi);
                        //Console.WriteLine($"+ Swiatlo = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        InsertDataIntoTable("outerLuminance", currentDateTime, OutDoorLumi);
                        break;
                    case "outdoorno2":
                        OutDoorNO2 = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorNO2Updated?.Invoke(this, OutDoorNO2);
                        // Console.WriteLine($"+ NO2 = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        InsertDataIntoTable("outerNo2", currentDateTime, OutDoorNO2);
                        break;
                    case "outdoorco":
                        OutDoorCO = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorCOUpdated?.Invoke(this, OutDoorCO);
                        // Console.WriteLine($"+ CO = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        InsertDataIntoTable("outerCo", currentDateTime, OutDoorCO);
                        break;
                    case "outdoornh3":
                        OutDoorNH3 = ConvertToDouble(e.ApplicationMessage.PayloadSegment);
                        OutdoorNH3Updated?.Invoke(this, OutDoorNH3);
                        // Console.WriteLine($"+ NH3 = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        InsertDataIntoTable("outerNh3", currentDateTime, OutDoorNH3);
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
            double result = -99.99;
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
