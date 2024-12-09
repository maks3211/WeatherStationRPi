using Avalonia.Media;
using AvaloniaTest.ViewModels;
using AvaloniaTest.Views;
using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
//using System.IO.Pipelines;

using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Tracing;
using MySql.Data.MySqlClient;
using Avalonia.Controls;
//using System.Device.Gpio;


//KLASA DO USUNIECIA - TERAZ PODZIELONE JEST NA UARTCOM I INDOORSENSORS


namespace AvaloniaTest.Models
{
    public class InDoorSensor
    {
       
        public event EventHandler<double> DataUpdated;
        public event EventHandler<double> DataUpdatedTwo;
        public event EventHandler<double> IndoorTempUpdated;
        public event EventHandler<double> IndoorHumUpdated;
        
        public event EventHandler<int> IndoorAltiUpdated;
        public event EventHandler<double> IndoorLumiUpdated;
        public event EventHandler<double> IndoorCOUpdated;
        public event EventHandler<int> IndoorPreasureUpdated;

        public event EventHandler<double> WindDirectionUpdated;
        public event EventHandler<int> WindSpeedUpdated;
        public event EventHandler<int> WindGustUpdated;

        private static InDoorSensor instance;
        private bool isFirst = true;
        private bool isSecond = true;

        public double temperature = 0.1;
        public double humidity = 0.0;
        public double pressureDouble = 0.0;
        public int pressure = 0;
        public int altitude = 0;
        public double altitudeDouble = 0.0;
        public double luminance = 0.0;
        public double co = 0.0;

        public double windDirection = 0.0;
        public int windSpeed = 0;
        public int windGust = 0;

        public int preasure = 0;
        //public static OutDoorSensor Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new OutDoorSensor();
        //        }
        //        return instance;
        //    }
        //}

        DateTime currentDateTime;
        MySqlConnection con;
        MySqlCommand cmd;
        public InDoorSensor()
        {
            try
            {
                // string connString = "server=sql11.freesqldatabase.com ; uid=sql11704729 ; pwd=89jVjCtqzd ; database=sql11704729";
                string connString = "server=sql7.freesqldatabase.com ; uid=sql7733142 ; pwd=BANKMcx6Gt ; database=sql7733142";
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


        public void StopEvery()
        { 
        isFirst = false;
        }

        public async Task RunReadData()
        {

            if (!Design.IsDesignMode)
            {
                
           
           // MainWindowViewModel.mqqt.OutdoorTempUpdated += OutDoorTemp_DataUpdated;
            string portName = "/dev/ttyS0";
            int baudRate = 9600;
            SerialPort Arduino = new SerialPort(portName,baudRate);
            try
            {
                Arduino.Open();
                Console.WriteLine("UART START C#");
            } catch (Exception ex)
            { 
                Console.WriteLine($"{ex.Message}");
            }

            while (isFirst)
            {
                    
                try
                {
                    if (Arduino.BytesToRead > 0)
                    {
                        string line = Arduino.ReadLine(); // Odczyt jednej linii z portu szeregowego
                        string secondLine = line.Trim(); // Usunięcie znaków końca linii
                        string[] parts = line.Split('<');
                        if (parts.Length >= 4)
                        {
                            // Odczytanie i konwersja wartości temperatury
                            if (double.TryParse(parts[0].Replace("-C", ""), out temperature))
                            {
                                // Odczytanie i konwersja wilgotności
                                if (double.TryParse(parts[1].Replace("-%", ""), out humidity))
                                {
                                    // Odczytanie i konwersja ciśnienia
                                    if (double.TryParse(parts[2].Replace("-hPa",""), out pressureDouble))
                                    {
                                        pressure = (int)Math.Floor(pressureDouble);
                                        // Odczytanie i konwersja wysokości
                                        if (double.TryParse(parts[3].Replace("-m",""), out altitudeDouble))
                                        {
                                            altitude = (int)Math.Floor(altitudeDouble);

                                            // Wszystkie wartości zostały pomyślnie odczytane i przypisane do zmiennych
                                            Console.WriteLine("ODCZYTANO WSZYSTKO uart- InDoorSensor");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Nie udało się podzielić ciągu na wystarczającą ilość części
                            Console.WriteLine("Invalid data format.");
                        }
                        //Console.WriteLine(secondLine);
                       // string test = secondLine[0] + secondLine[1];
                    }
                }
                catch (Exception ex) //Jezeli nie ma czujnika to losowe wartosci dla testu- docelowo zwracac kod bledu
                {                
                    double j = new Random().NextDouble();
                    j += 20;

                    temperature = Math.Round(j, 1);
                    humidity = new Random().Next(101);


                    pressure = new Random().Next(960, 1060);
                    altitude = new Random().Next(200,500);
                    luminance = altitude + 5;
                    co = new Random().Next(1,20);

                }

                // Console.WriteLine("Humidity: " + humidity);
                // Console.WriteLine("Pressure: " + pressure);
                // Console.WriteLine("cisnienie: " + preasure);


                // Console.WriteLine($"PRZEROBIONA TEMPAERTURA {a}. ");
                // double moja = Convert.ToDouble(a);
                // Console.WriteLine($"PRZEROBIONA TEMPAERTURA {moja} ");
               // windDirection = new Random().NextDouble() * 360;
              ///  windSpeed = new Random().Next(0, 31);
              //  int randomGust = new Random().Next(0, 31);
              //  if (randomGust > windGust)
               // {
               //     windGust = randomGust;
               // }


                IndoorTempUpdated?.Invoke(this, temperature); // Wywołanie zdarzenia, przekazujące aktualną wartość i
                IndoorHumUpdated?.Invoke(this, humidity);
                
                IndoorAltiUpdated?.Invoke(this, altitude);
                IndoorPreasureUpdated?.Invoke(this, pressure);
                IndoorLumiUpdated?.Invoke(this, luminance);
                IndoorCOUpdated?.Invoke(this, co);


              //  WindDirectionUpdated?.Invoke(this, windDirection);
               // WindSpeedUpdated?.Invoke(this, windSpeed);
               // WindGustUpdated?.Invoke(this, windGust);

                currentDateTime = DateTime.Now;
                //InsertDataIntoTable("innerTemperature", currentDateTime, temperature);
                //InsertDataIntoTable("innerHumidity", currentDateTime, humidity);
                //InsertDataIntoTable("innerAltitude", currentDateTime, altitude);
                //InsertDataIntoTable("innerPreasure", currentDateTime, preasure);
                //InsertDataIntoTable("innerLuminance", currentDateTime, luminance);
                //InsertDataIntoTable("innerCo", currentDateTime, co);


                await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }


        private void OutDoorTemp_DataUpdated(object sender, double e)
        {
            IndoorTempUpdated?.Invoke(this, e);
            Console.WriteLine("z sensora bezposrednio Update temperatury");
        }

        public async Task RunReadDataTwo()
        {
                 
            Random rnd = new Random();
            double j = 0;
            while (isFirst)
            {
              // Console.WriteLine("j" + j);
                 j = 20.0 + rnd.NextDouble();
                  double z = Math.Round(j, 1);
                DataUpdatedTwo?.Invoke(this, z); // Wywołanie zdarzenia, przekazujące aktualną wartość i
                await Task.Delay(TimeSpan.FromSeconds(1));
            }          
    
        }

        public async Task StartMake()
        {
          //  Task task1 = RunReadData();
          //  Task task2 = RunReadDataTwo();
          //  await Task.WhenAll(task1, task2);
        }
    } 
}
