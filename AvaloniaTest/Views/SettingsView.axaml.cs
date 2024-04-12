using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaTest.Views
{

    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            Console.WriteLine("nowy");
        }
        private void PointerPressedHandler(object sender, PointerPressedEventArgs args)
        {
            var point = args.GetCurrentPoint(sender as Control);
            var x = point.Position.X;
            var y = point.Position.Y;
            var msg = $"Pointer press at {x}, {y} relative to sender.";
            if (point.Properties.IsLeftButtonPressed)
            {
                msg += " Left button pressed.";
            }
            if (point.Properties.IsRightButtonPressed)
            {
                msg += " Right button pressed.";
            }
            Console.WriteLine(msg);
        }

    }
}