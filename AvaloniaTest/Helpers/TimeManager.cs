using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{

    //Class not used
    public partial class TimeManager : ObservableObject
    {
        private static TimeManager _instance;
        public static TimeManager Instance => _instance ??= new TimeManager();

        private DispatcherTimer _timer;

        [ObservableProperty]
        private string _currentTime;

        private TimeManager()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (sender, e) => CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            _timer.Start();
        }
    }
}
