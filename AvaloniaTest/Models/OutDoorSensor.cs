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
        private static OutDoorSensor instance;
        private bool isFirst = true;
        private bool isSecond = true;

        

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
            double temperature = 0.1;
            double humidity = 0.0;
            double pressure = 0.0;
            double altitude = 0.0;
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
                        Console.WriteLine(secondLine);
                       // string test = secondLine[0] + secondLine[1];
                    }
                }
                catch (Exception ex) //Jezeli nie ma czujnika to losowe wartosci dla testu- docelowo zwracac kod bledu
                {                
                    double j = new Random().NextDouble();
                    j += 20;
                    temperature = Math.Round(j, 1);
                    IndoorTempUpdated?.Invoke(this, temperature); 
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Temperature: " + temperature);
                Console.WriteLine("Humidity: " + humidity);
                Console.WriteLine("Pressure: " + pressure);
                Console.WriteLine("Altitude: " + altitude);

                IndoorTempUpdated?.Invoke(this, temperature); // Wywołanie zdarzenia, przekazujące aktualną wartość i
                IndoorHumUpdated?.Invoke(this, humidity);
                IndoorPresUpdated?.Invoke(this, pressure);
                IndoorAltiUpdated?.Invoke(this, altitude);
                //DataUpdatedTwo?.Invoke(this, humidity);
                await Task.Delay(TimeSpan.FromSeconds(5));   
        }
    }

        public async Task RunReadDataTwo()
        {
                 
            Random rnd = new Random();
            double j = 0;
            while (isFirst)
            {
               Console.WriteLine("j" + j);
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
