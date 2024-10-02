using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Models
{
    public partial class OutdoorVal: ObservableObject
    {
        [ObservableProperty]
        private string? napis;

        public void UpdateNapis(string v)
        {
            Napis = v;
        }
    }
}
