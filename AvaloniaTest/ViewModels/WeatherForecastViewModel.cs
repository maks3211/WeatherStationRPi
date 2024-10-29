using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;


namespace AvaloniaTest.ViewModels
{
    
    public partial class WeatherForecastViewModel : ObservableObject
    {
        //Singleton stuff
        private static WeatherForecastViewModel _instance;
        private static readonly object _lock = new object(); // Obiekt synchronizacji dla wątków

        public double WeatherWidth { get; private set; } = 243.5;
        public double WeatherHeight { get; private set; } = 293.0;
        public double TopPos { get; private set; } = 166.5;
        public double LeftPos { get; private set; } = 10;

        public bool IsOpen { get; private set; } = false;
        public bool Ready { get; private set; } = true;

        private Border border;
        private StackPanel panel;

        private WeatherForecastViewModel(Border border, StackPanel panel)
        {
            this.border = border;
            this.panel = panel;
        }

        public static WeatherForecastViewModel GetInstance(Border border, StackPanel panel)
        {
            lock (_lock) // Synchronizacja dla wielowątkowości
            {
                if (_instance == null)
                {
                    _instance = new WeatherForecastViewModel(border, panel);
                }
                return _instance;
            }
        }

        public void UpdateUIElements(Border newBorder, StackPanel newPanel)
        {
            // Możliwość aktualizacji border i panel, jeśli jest taka potrzeba
            this.border = newBorder;
            this.panel = newPanel;
        }

        public void Open()
        {
            SetBigSize();
            OpenAnimation();
        }
        public void Close() {
            SetDefaultPos();
            SetDefaultSize();
            CloseAnimation();
        }

        public void SetBigSize()
        {
            WeatherWidth = 800;
            WeatherHeight = 450;
            TopPos = 10;
            LeftPos = 100;
           
         }
        public void SetDefaultPos()
        {
            TopPos = 166.5;
            LeftPos = 10;
        } 
        public void SetDefaultSize()
        {
            WeatherWidth = 243.5;
            WeatherHeight = 293.0;
        }

        private async void OpenAnimation()
        {
            if (IsOpen) return;
            if (!Ready) return;
            Ready = false;
            var animation = new Avalonia.Animation.Animation
            {
                FillMode = FillMode.Forward,
                Duration = TimeSpan.FromMilliseconds(300),
                Easing = new SineEaseInOut(),
                Children =
            {
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(StackPanel.WidthProperty, WeatherWidth) },
                    Cue = new Cue(1d)
                } ,
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(StackPanel.HeightProperty, WeatherHeight) },
                    Cue = new Cue(1d)
                },
                 new Avalonia.Animation.KeyFrame
            {
                Setters = { new Setter(Canvas.TopProperty, TopPos) }, // Docelowa pozycja Canvas.Top
                Cue = new Cue(1d)
            },
                    new Avalonia.Animation.KeyFrame
            {
                Setters = { new Setter(Canvas.LeftProperty, LeftPos) }, // Docelowa pozycja Canvas.Top
                Cue = new Cue(1d)
            }
            }
            };

            var tasks = new[]
   {
        animation.RunAsync(panel),
        animation.RunAsync(border)
    };

            await Task.WhenAll(tasks);
            IsOpen = true;
            Ready = true;
        }        
        private async void CloseAnimation()
        {

             if (!IsOpen) return;
            if (!Ready) return;
            Ready = false;
            var animation = new Avalonia.Animation.Animation
            {
                FillMode = FillMode.Forward,
                Duration = TimeSpan.FromMilliseconds(300),
                Easing = new SineEaseInOut(),
                Children =
            {
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(StackPanel.WidthProperty, WeatherWidth) },
                    Cue = new Cue(1d)
                } ,
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(StackPanel.HeightProperty, WeatherHeight) },
                    Cue = new Cue(1d)
                },
                 new Avalonia.Animation.KeyFrame
            {
                Setters = { new Setter(Canvas.TopProperty, TopPos) }, // Docelowa pozycja Canvas.Top
                Cue = new Cue(1d)
            },
                    new Avalonia.Animation.KeyFrame
            {
                Setters = { new Setter(Canvas.LeftProperty, LeftPos) }, // Docelowa pozycja Canvas.Top
                Cue = new Cue(1d)
            }
            }
            };

            var tasks = new[]
   {
        animation.RunAsync(panel),
        animation.RunAsync(border)
    };

            await Task.WhenAll(tasks);
            IsOpen = false;
            Ready = true;
        }
    }
}
