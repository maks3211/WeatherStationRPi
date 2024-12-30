using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    public class Clock: ObservableObject
    {
        private static readonly Lazy<Clock> _instance = new Lazy<Clock>(()=> new Clock());
        private DateTime _currentTime;
        private DispatcherTimer _timer;

        private readonly Dictionary<string, (TimeSpan Interval, Action Task)> _tasks = new();
        public Dictionary<string, (TimeSpan Interval, Action Task)> Tasks => _tasks;

        private readonly Dictionary<string, DateTime> _lastExecutionTimes = new();

        private readonly Dictionary<string, (List<TimeOnly> times, Action Task) > _specificTimeTasks = new();

        public event Action? TimeElapsed;

        private string _currentDate;
        public string CurrentDate
        {
            get => _currentDate;
            private set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    OnPropertyChanged(nameof(CurrentDate));
                }
            }
        }
      

        private TimeSpan _interval = TimeSpan.FromSeconds(1);
        public  TimeSpan Interval
        {
            get => _interval;
            set
                {
                if (_interval != value) { 
                _interval = value;
                UpdateInterval();
                }


            }
        }

        public static Clock Instance => _instance.Value;

        public DateTime CurrentTime
        { 
            get => _currentTime;
            private set {
                if (_currentTime != value)
                { 
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }

        private Clock()
        {
            _currentTime = DateTime.Now;
            _timer = new DispatcherTimer();
            _timer.Interval = _interval;
            _timer.Tick += OnTimerTick;
            _timer.Start();
            //_timer = new Timer(1000);
        }


        public void AddSpecificTimeTask(string name,Action task ,List<TimeOnly> times)
        {
                _specificTimeTasks[name] = (times, task);
        }
        public void RemoveSpecificTimeTask(string name) {
            if (_specificTimeTasks.ContainsKey(name))
            {
                _specificTimeTasks.Remove(name);   
            }
        }

        public void AddTask(string name,Action task,TimeSpan interval)
        {
            if (_tasks.ContainsKey(name))
            {
                _tasks[name] = (interval, task);
            }
            else
            {
                _tasks[name] = (interval, task);
                _lastExecutionTimes[name] = DateTime.Now;
            }
        }

  

        public void UpdateTaskInterval(string name, TimeSpan newInterval)
        {
            if (_tasks.ContainsKey(name))
            {
                var (currentInterval, task) = _tasks[name];
                _tasks[name] = (newInterval, task);
                _lastExecutionTimes[name] = DateTime.Now;
            }
        }

        public void RemoveTask(string name)
        {
            if (_tasks.ContainsKey(name))
            {
                _tasks.Remove(name);
                _lastExecutionTimes.Remove(name);
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
           
            _currentTime = DateTime.Now;

            foreach (var (name, (interval, task)) in _tasks)
            {
               
                var lastExecutionTime = _lastExecutionTimes[name];

                if (interval == TimeSpan.Zero)
                {
                    continue; 
                }
                if (_currentTime - lastExecutionTime >= interval)
                {
                    task();
                    _lastExecutionTimes[name] = _currentTime;
                }
            }

            foreach (var (_,(times, task)) in _specificTimeTasks)
            {
                foreach (var t in times)
                {
                    var diff = _currentTime.Second - _interval.Seconds;
                    if (t.Second > diff) { 
                    if (_currentTime.Hour == t.Hour && _currentTime.Minute == t.Minute)
                    {
                        task();
                    }
                    }
                }
            }
        }

        private void UpdateInterval()
        {
            if (_timer != null)
            {
                _timer.Interval = _interval;
            }
        }

    }
}
