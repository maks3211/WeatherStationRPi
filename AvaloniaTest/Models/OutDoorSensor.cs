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
//using System.Device.Gpio;



namespace AvaloniaTest.Models
{
    public class OutDoorSensor
    {
       
        public event EventHandler<double> DataUpdated;
        public event EventHandler<double> DataUpdatedTwo;
        public event EventHandler<double> IndoorTempUpdated;
        public event EventHandler<double> IndoorHumUpdated;
        public event EventHandler<double> IndoorPresUpdated;
        public event EventHandler<double> IndoorAltiUpdated;

        public event EventHandler<int> IndoorPreasureUpdated;

        public event EventHandler<double> WindDirectionUpdated;
        public event EventHandler<int> WindSpeedUpdated;
        public event EventHandler<int> WindGustUpdated;

        private static OutDoorSensor instance;
        private bool isFirst = true;
        private bool isSecond = true;

        public double temperature = 0.1;
        public double humidity = 0.0;
        public double pressure = 0.0;
        public double altitude = 0.0;

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

        public void StopEvery()
        { 
        isFirst = false;
        }

        public async Task RunReadData()
        {
            MainWindowViewModel.mqqt.OutdoorTempUpdated += OutDoorTemp_DataUpdated;
            Console.WriteLine("TUTAJ");
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
                                    if (double.TryParse(parts[2].Replace("-hPa",""), out pressure))
                                    {
                                        // Odczytanie i konwersja wysokości
                                        if (double.TryParse(parts[3].Replace("-m",""), out altitude))
                                        {
                                            // Wszystkie wartości zostały pomyślnie odczytane i przypisane do zmiennych
                                          
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

                    windDirection = new Random().NextDouble() * 360;
                    windSpeed = new Random().Next(0, 31);
                    int randomGust= new Random().Next(0, 31);
                    if (randomGust > windGust)
                    {
                        windGust = randomGust;
                    }

                    preasure = new Random().Next(960, 1060);
                    altitude = new Random().Next(200,500);
                    luminance = altitude + 5;
                    co = new Random().NextDouble() * 200;
                    Console.WriteLine(co);


                }

                // Console.WriteLine("Humidity: " + humidity);
                // Console.WriteLine("Pressure: " + pressure);
                // Console.WriteLine("cisnienie: " + preasure);


                // Console.WriteLine($"PRZEROBIONA TEMPAERTURA {a}. ");
                // double moja = Convert.ToDouble(a);
                // Console.WriteLine($"PRZEROBIONA TEMPAERTURA {moja} ");
  

                //IndoorTempUpdated?.Invoke(this, te); // Wywołanie zdarzenia, przekazujące aktualną wartość i
                IndoorHumUpdated?.Invoke(this, humidity);
                IndoorPresUpdated?.Invoke(this, pressure);
                IndoorAltiUpdated?.Invoke(this, altitude);
                WindDirectionUpdated?.Invoke(this, windDirection);
                WindSpeedUpdated?.Invoke(this, windSpeed);
                WindGustUpdated?.Invoke(this, windGust);

                currentDateTime = DateTime.Now;
                InsertDataIntoTable("innerTemperature", currentDateTime, temperature);
                InsertDataIntoTable("innerHumidity", currentDateTime, humidity);
                InsertDataIntoTable("innerAltitude", currentDateTime, altitude);
                InsertDataIntoTable("innerPreasure", currentDateTime, preasure);
                InsertDataIntoTable("innerLuminance", currentDateTime, luminance);
                InsertDataIntoTable("innerCo", currentDateTime, co);


                await Task.Delay(TimeSpan.FromSeconds(5));   
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
            Task task1 = RunReadData();
            Task task2 = RunReadDataTwo();
            await Task.WhenAll(task1, task2);
        }
    } 
}
