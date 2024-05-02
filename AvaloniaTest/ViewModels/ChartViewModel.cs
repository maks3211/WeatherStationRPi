using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;


namespace AvaloniaTest.ViewModels
{

    public record WeatherForecast(DateOnly date, int temp)
    {
        public static IEnumerable<WeatherForecast> TestData() =>
            [
            new WeatherForecast(new DateOnly(2024,4,21),14 ),
             new WeatherForecast(new DateOnly(2024,4,22),7 ),
             new WeatherForecast(new DateOnly(2024,4,23),8 ),
             new WeatherForecast(new DateOnly(2024,4,24),10 ),
            ];
    }


    


    public partial class ChartViewModel : ViewModelBase
    {

        public ISeries[] Series { get; set; } =
    {
        new LineSeries<WeatherForecast>
        {
            Values = WeatherForecast.TestData(),
            Mapping = (sample,_) => new Coordinate(sample.date.Day, sample.temp),
            Fill = null
        }
    };

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "My chart title",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
    }
}
