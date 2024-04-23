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

        private void Binding(object? sender, Avalonia.Controls.TextChangedEventArgs e)
        {
        }

        private void Binding(object? sender, Avalonia.Input.KeyEventArgs e)
        {
        }
    }
}
