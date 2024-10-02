using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models.ObservablesProperties
{
    public partial class SettingsProperites : ObservableObject
    {
        [ObservableProperty]
        private bool? themeBtnVis;

      
    }
}
