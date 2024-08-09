using Avalonia.Controls;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using AvaloniaTest.Models;
using System.Threading.Tasks;

namespace AvaloniaTest.Views
{
    public class OnScreenKeyboardBehavior : AvaloniaObject
    {
        public static OnScreenKeyboard keyboard;
        private static TextBox _textBox;
       // private static int lineNumber = 0;
        public static readonly AttachedProperty<bool> IsKeyboardEnabledProperty =
             AvaloniaProperty.RegisterAttached<TextBox, bool>(
                 "IsKeyboardEnabled",
                 typeof(OnScreenKeyboardBehavior),
                 defaultValue: false);

        public static readonly AttachedProperty<Button> EnterButtonProperty =
              AvaloniaProperty.RegisterAttached<TextBox, Button>(
                  "EnterButton",
                  typeof(OnScreenKeyboardBehavior));


        static OnScreenKeyboardBehavior()
        {   
            IsKeyboardEnabledProperty.Changed.AddClassHandler<TextBox>(OnIsKeyboardEnabledChanged);
        }

        public static Button GetEnterButton(AvaloniaObject element)
        {
            return element.GetValue(EnterButtonProperty);
        }
        public static void SetEnterButton(AvaloniaObject element, Button value)
        {
            element.SetValue(EnterButtonProperty, value);
        }

        public static bool GetIsKeyboardEnabled(AvaloniaObject element)
        {
            return element.GetValue(IsKeyboardEnabledProperty);
        }

        public static void SetIsKeyboardEnabled(AvaloniaObject element, bool value)
        {
            element.SetValue(IsKeyboardEnabledProperty, value);
        }

        private static void OnIsKeyboardEnabledChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is true)
            {
                //  textBox.GotFocus += TextBox_GotFocus;
                textBox.PointerReleased += TextBox_Clicked;
            }
            else
            {
              //  textBox.GotFocus -= TextBox_GotFocus;
                textBox.PointerReleased -= TextBox_Clicked;
               
            }
        }



        private static   void TextBox_Clicked(object? sender, PointerReleasedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _textBox = textBox;

                if (keyboard is not null)
                {
                    keyboard.SetAssociatedTextBox(textBox);
                    keyboard.SetAssociatedEnterButton(GetEnterButton(textBox));
                    if (!keyboard.GetIsVisable())
                    {
                        keyboard.Open();
                    }
                    

                   
                }
             
            }
        }  
    }
}

