using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaTest.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AvaloniaTest.Models
{
    public partial class ESPnetworkData : ObservableObject
    {
        [ObservableProperty]
        public string _ssid;
        [ObservableProperty]
        public double _strength;

        [ObservableProperty]
        public StreamGeometry _strenghIcon;

        [ObservableProperty]
        public StreamGeometry _test;

        private static ESPnetworkData _instance;
        public static ESPnetworkData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ESPnetworkData();
            }
            return _instance;
        }

        private ESPnetworkData() {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(40);
            _timer.Tick += CheckConnectionStatus;
            _timer.Start();
        }

        [ObservableProperty]
        public bool _connected;

        private DateTime _lastMessageTime;
        private DispatcherTimer _timer;


        public void UpdateLastMessageTime()
        {
            _lastMessageTime = DateTime.Now;
            Connected = true;
        }

        private void CheckConnectionStatus(object? sender, EventArgs e)
        {
            // Sprawdź, czy od ostatniej wiadomości minęła 1 minuta
            if (DateTime.Now - _lastMessageTime > TimeSpan.FromSeconds(40))
            {
                Connected = false;
                OnPropertyChanged(nameof(Connected));  // Powiadomienie o zmianie statusu
            }
        }


    }
}
