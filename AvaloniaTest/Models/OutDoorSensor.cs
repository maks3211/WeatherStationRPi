using Avalonia.Media;
using AvaloniaTest.ViewModels;
using AvaloniaTest.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AvaloniaTest.Models
{
    public class OutDoorSensor
    {

        public event EventHandler<double> DataUpdated;
        public event EventHandler<double> DataUpdatedTwo;

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

          
            Random rnd = new Random();
            double i = 0;

            while (isFirst)
            {
                        i = 30.1 + rnd.NextDouble();
                      double zs = Math.Round(i, 1);
                  //  Console.WriteLine("while " + i);
                    
                DataUpdated?.Invoke(this, zs); // Wywołanie zdarzenia, przekazujące aktualną wartość i
                await Task.Delay(TimeSpan.FromSeconds(1));   
        }
    }

        public async Task RunReadDataTwo()
        {
                 
            Random rnd = new Random();
            double j = 0;
            while (isFirst)
            {
             //  Console.WriteLine("j" + j);
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
