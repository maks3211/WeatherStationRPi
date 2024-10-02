using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models.ObservablesProperties
{
    public partial class TimeProperties: ObservableObject
    {
        [ObservableProperty]
        public string _currentTime;
        [ObservableProperty]
        public string _currentDay;
        [ObservableProperty]
        public string _currentDate;

    }
}
