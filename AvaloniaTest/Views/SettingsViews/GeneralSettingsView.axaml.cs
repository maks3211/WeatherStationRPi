using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using System;

namespace AvaloniaTest.Views
{

    public partial class GeneralSettingsView : UserControl
    {



        public GeneralSettingsView()
        {
            InitializeComponent();        
        }



        private void AutoCompleteBox_GotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
        {
        }
       
        private void Binding_1(object? sender, Avalonia.Input.GotFocusEventArgs e)
        {
        }

        private void CityInputBox_GotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
        {
            var position = CityInputBox.TranslatePoint(new Avalonia.Point(0, 0), MainScroll);

            if (position.HasValue)
            {
                // Pozycja Y elementu "Napis_box" wzglêdem "ScrollViewer"
                double yPosition = position.Value.Y;
            }
            MainScroll.Offset = new Point(0, 1900);
        }

       
    }
}