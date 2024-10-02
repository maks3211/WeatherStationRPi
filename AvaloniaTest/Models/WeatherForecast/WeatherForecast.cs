using AvaloniaTest.Helpers;
using Newtonsoft.Json;
using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AvaloniaTest.Models.WeatherForecast.WeatherForecast;
using static System.Runtime.InteropServices.JavaScript.JSType;


//IKONY POGODY:
//https://openweathermap.org/weather-conditions


namespace AvaloniaTest.Models.WeatherForecast
{
    //!! DODAĆ POLE DO DESZCZU, SNIEG
    //POLE POP - ODPOWIADA ZA PRAWDOPODOBIENSTWO OPADOW 0-1
    public class WeatherForecast
    { 
        public class coord
        {
          public  double lon { get; set; }
            public double lat { get; set; }
        }

        public class weather
        {
           public string main { get; set; }
           public string description { get; set; }
           public string icon { get; set; }
        }
        public class main
        {
          public  double temp { get; set; }
          public double feels_like { get; set; }
          public double temp_max { get; set; }
          public double temp_min { get; set; }
          public  double pressure { get; set; }
          public double humidity { get; set; }
        }

        public class wind
        {
            public double speed { get; set; }
            
        }
        public class clouds // procentowe zachmurzenie
        {
            public double all {  get; set; }    
        }

        public class sys
        {
          public  long sunrise { get; set; }
            public long sunset { get; set; }
        }

        public class WeatherInfo
        {
            public coord coord { get; set; }
            public List <weather> weather { get; set; }
            public main main { get; set; }
            public wind wind {  get; set; } 
            public sys sys { get; set; }    
            public clouds clouds { get; set; }  
            public string name { get; set; }
            public long dt { get; set; }
            public long timezone { get; set; }

        }


        public class city 
        {
            public string name { get; set;}
            public string country { get; set;}
            public long population { get; set; }
            public long timezone { get; set;}
        }

        public class Rain
        {
            [JsonProperty("1h")]
            public double OneHour { get; set; }
        }

        //PROGNOZA POGODY

        public class list : IWeatherList
        {
            public long dt { get; set; }
            public main main { get; set; }
            public List <weather> weather { get; set; }
            public clouds clouds { get; set; }
            public wind wind { get; set; }
            public Rain rain { get; set; }
            public double pop { get; set; }
            public string dayTime { get; set; }

            //interface implementation
            public long Dt => dt;
            public List<WeatherForecast.weather> Weather => weather;
            public double Pop => pop;

        }



   



        public class HourlyForecastInfo : IForecast
        {
            public List<WeatherForecast.list> list { get; set; }
            public WeatherForecast.city city { get; set; }


        
            private List<int> GetIndexesForCurrentDay(string targetDay)
            {
                if (list == null || list.Count() == 0)
                    return new List<int>();
                var indexes = new List<int>();


                for (int i = 0; i < list.Count; i++)
                {
                    // string day = ConvertDateTimeToDay(list[i].dt);
                    string day = TimeConverters.ConvertDateTimeToDay(list[i].dt,city.timezone);
                    if (day == targetDay)
                    {
                        indexes.Add(i);
                    }
                    if (indexes.Count() > 24)
                    {
                        break;
                    }
                }
                return indexes;
            }
            public double GetDailyMaxTemp(string date)
            {
                var ind = GetIndexesForCurrentDay(date);
                if (list == null || ind == null || !ind.Any() || !list.Any())
                {
                    return -500; 
                }
                return ind
                .Where(index => index >= 0 && index < list.Count) 
                .Select(index => list[index].main.temp_max)
                .Max();
            }
            public double GetDailyMinTemp(string date)
            {
                var ind = GetIndexesForCurrentDay(date);
                if (list == null || ind == null || !ind.Any() || !list.Any())
                {
                    return -500; // Lub zwróć 0, Double.MinValue, itp.
                }
                return ind
                .Where(index => index >= 0 && index < list.Count) 
                .Select(index => list[index].main.temp_min)
                .Min();
            }
        }
    }
}
