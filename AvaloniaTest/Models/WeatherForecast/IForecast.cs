using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models.WeatherForecast
{
    public interface IForecast
    {
        List<WeatherForecast.list> list { get; set; }
        WeatherForecast.city city { get; set; }

      
    }

    public interface IWeatherList
    {
        long Dt { get; }
        List<WeatherForecast.weather> weather { get; }
        double Pop { get; }
    }
}
