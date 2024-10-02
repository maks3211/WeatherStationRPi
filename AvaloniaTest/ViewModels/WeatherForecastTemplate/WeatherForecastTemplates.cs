using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.ViewModels.WeatherForecastTemplate
{
    public class HourlyForecastTemplate
    {

        public string HourTime {get;}
      //  public Bitmap Icon {get;}
        public double Temperature {get;}
        public double PrecipitationProb {get;}
        public double RainVol {get;}
       
        public HourlyForecastTemplate(string hourTime,double temp, double precipitationProb, double rainVol)
        {
            HourTime = hourTime;
            //Icon = icon; , Bitmap icon,
            Temperature = temp;
            PrecipitationProb = precipitationProb;
            RainVol = rainVol;
        }
    }
}
