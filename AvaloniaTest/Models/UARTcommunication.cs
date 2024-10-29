﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models
{
    public class UARTcommunication
    {

        private IndoorSensors sensors;
        private SerialPort serialPort;
        private const int baudRate = 9600;
        private bool isOpen = false;

        public UARTcommunication(IndoorSensors indoorSensors, string portName)
        {
            sensors = indoorSensors;
            serialPort = new SerialPort(portName, baudRate);
        }


        public void Open()
        {
            try
            {
                serialPort.Open();
                isOpen = true;
            }
            catch (Exception ex) 
            { 

                Console.WriteLine(ex.Message);
                isOpen = false;
            }
        }

        public void Close()
        {
            serialPort?.Close();
            isOpen = false;
        }


        public async Task ReadDataAsync()
        {

            try
            {
               Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            while (isOpen)
            {
                try
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        string line = serialPort.ReadLine(); // Odczyt jednej linii z portu szeregowego
                        string secondLine = line.Trim(); // Usunięcie znaków końca linii
                        string[] parts = line.Split('<');
                        if (parts.Length >= 5)
                        {
                            if (double.TryParse(parts[0].Replace("-C", ""), out double temperature))
                            {
                                sensors.Temperature.Value = temperature;
                            }

                            // Odczytanie i aktualizacja wilgotności
                            if (double.TryParse(parts[1].Replace("-%", ""), out double humidity))
                            {
                                sensors.Humidity.Value = humidity;
                            }

                            // Odczytanie i aktualizacja ciśnienia
                            if (double.TryParse(parts[2].Replace("-hPa", ""), out double pressure))
                            {
                                sensors.Pressure.Value = pressure;
                            }

                            // Odczytanie i aktualizacja wysokości
                            if (int.TryParse(parts[3].Replace("-m", ""), out int altitude))
                            {
                                sensors.Altitude.Value = altitude;
                            }
                            if (int.TryParse(parts[4].Replace("-ppm", ""), out int co))
                            {
                                sensors.Co.Value = co;
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid data format.");
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid data format.");
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

        }


    }
}
