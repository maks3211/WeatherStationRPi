using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Xml.Linq;
namespace AvaloniaTest.Views
{
    public partial class NetworkSettingsView : UserControl
    {
        public static bool ShiftState { get; private set; }
        public int lineNumber = 0;
        private string boxText = "";

       

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
        private void OnShowKeyboardClick(object sender, RoutedEventArgs e)
        {
            // Tworzenie instancji OnScreenKeyboard z nazw¹ panelu
            // Console.WriteLine("Ustawienifffa - siec");


          //  OnScreenKeyboard key = new OnScreenKeyboard(KeyboardPanel);
           // key.KeyPressed += OnKeyPressed;


            /* for (int i = 1; i <= 3; i++)
             {
                 var button = new Button { Content = $"Button {i}" };
                 KeyboardPanel.Children.Add(button);
             }*/

        }

        private void Passwordbox_PointerPressed(object sender, PointerPressedEventArgs args)
        {
            var ctl = sender as Control;
            if (ctl != null)
            {
                FlyoutBase.ShowAttachedFlyout(ctl);
            }
            Console.WriteLine("asd");
        }

        private void OnKeyPressed(object sender, string text)
        {
            Passwordbox.Focus();
            if (text.Length > 1)
            {
                if (text == "back")
                {
                    if (Passwordbox.Text.Length > 0)
                    {
                        if (Passwordbox.CaretIndex > 0)
                        {
                            Passwordbox.Text = Passwordbox.Text.Remove(Passwordbox.CaretIndex - 1, 1);
                            lineNumber--;
                        }
                        // Passwordbox.Text = Passwordbox.Text.Substring(0, Passwordbox.Text.Length - 1);
                       
                    }
                }
                else if (text == "right")
                {
                    Console.WriteLine("right");
                    if (Passwordbox.CaretIndex < Passwordbox.Text.Length)
                    {
                        lineNumber++;
                    }
                }
                else if (text == "left")
                {
                    if (Passwordbox.CaretIndex > 0)
                    {
                        lineNumber--;
                    }
                } 
                else if (text == "enter")
                {
                    
                    Console.WriteLine("dodac OPCJE ENTER");
                    ConnectButton.Command.Execute(null);
                }          
            }
            else 
            {
                {
                    string currentText = Passwordbox.Text;
                    if (currentText is null)
                    {
                        Passwordbox.Text = text;
                        Passwordbox.CaretIndex = 0;
                        lineNumber++;
                    }
                    else {
                        int caretIndex = Passwordbox.CaretIndex;
                        string newText = currentText.Insert(caretIndex, text);
                        Passwordbox.Text = newText;
                        Passwordbox.CaretIndex = caretIndex + text.Length;
                        lineNumber++;
                    }
                }
            }
            Passwordbox.CaretIndex = lineNumber;
        }

    }
}
