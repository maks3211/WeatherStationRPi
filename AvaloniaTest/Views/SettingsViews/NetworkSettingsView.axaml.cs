using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaTest.Views
{
    public partial class NetworkSettingsView : UserControl
    {
        public NetworkSettingsView()
        {
            InitializeComponent();
            Console.WriteLine("Ustawienia - siec");
        }
    }
}
