using Avalonia.Controls;
using Avalonia.Dialogs.Internal;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Layout;
using Avalonia.Interactivity;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Asn1.Cmp;
using System.Collections.ObjectModel;
namespace AvaloniaTest.Views
{
    public class OnScreenKeyboard : Window
    {
        private Grid keyboardGrid;

        private Panel keyboard;
        private const string qwertyKeyboard = "`1234567890-=qwertyuiop[]\\asdfghjkl;'zxcvbnm,./";
        private const string shifKeyboard = "as";
        private string[] fnKeysList =[ "Shift", "Space", "Backspace", "Alt"];

        private KeyboardKey mojGuzik;
        private bool shiftState = false;
        private bool altState = false;

        private Dictionary<string, Button> FnKeys = new Dictionary<string, Button>();

        public event EventHandler<string> KeyPressed;

        private StackPanel mainPanel;


        public ObservableCollection<string> fourthRowDefault { get; } = new()
        {
            "z", "x", "c", "v", "b", "n", "m", ",", ".", "/"
        };


        private List<KeyboardKey> firsRowButtons = new List<KeyboardKey>();
        private StackPanel firstRow;
        private string[] firstRowDefault = [ "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" ];
        private string[] firstRowShift = [ "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+" ];

        private StackPanel secondRow;
        private List<KeyboardKey> secondRowButtons = new List<KeyboardKey>();
        private string[] secondRowDefault = ["q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\"];
        private string[] secondRowShift = ["", "", "ę", "", "","", "€", "", "ó", "", "", "", "",];

        private StackPanel thirdRow;
        private List<KeyboardKey> thirdRowButtons = new List<KeyboardKey>();
        private string[] thirdRowDefault = ["a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'"];
        private string[] thirdRowShift = ["ą", "ś", "", "", "", "", "", "", "ł", "", ""];

        private StackPanel fourthRow;
       // private string[] fourthRowDefault = ["z", "x", "c", "v", "b", "n", "m", ",", ".", "/"];
        // private string[] fourthRowShift = ["Z", "X", "C", "V", "B", "N", "M", "<", ">", "?"];
        private string[] fourthRowShift = ["ż", "ź", "ć", "", "", "ń", "", "", "", ""];       
        private List<KeyboardKey> fourthRowButtons = new List<KeyboardKey>();

        private StackPanel fifthRow;



        public OnScreenKeyboard(Panel child)
        {
            InitializeGrid();
            if (child is Panel panel)
            {
                //panel.Children.Add(keyboardGrid);
                panel.Children.Add(mainPanel);
            }

           
          //  CreateButtons();
            //InitializeBackspace();
            InitFirstRow();
            InitSecondRow();
            InitThirdRow();
            InitFourthRow(); 
            InitFifthRow();
            foreach (var a in firsRowButtons)
            {
                a.UpdateText(shiftState);
            }
            foreach (var a in secondRowButtons)
            {
                a.UpdateText(shiftState);
            }
            foreach (var a in thirdRowButtons)
            {
                a.UpdateText(shiftState);
            }
            foreach (var a in fourthRowButtons)
            {
                a.UpdateText(shiftState);
            }
        }

        private void InitializeGrid()
        {
            mainPanel = new StackPanel();
            firstRow = new StackPanel {
                Margin = new Avalonia.Thickness(2, 2, 2, 2),
            Orientation = Orientation.Horizontal,
            };
            secondRow = new StackPanel { 
            Orientation = Orientation.Horizontal,
            Margin = new Avalonia.Thickness(30, 2, 2, 2)
            };
            thirdRow = new StackPanel { 
            Orientation = Orientation.Horizontal,
            Margin = new Avalonia.Thickness(60, 2, 2, 2)
            };
            fourthRow = new StackPanel { 
            Orientation = Orientation.Horizontal,
            Margin = new Avalonia.Thickness(2, 2, 2, 2)
            };
           fifthRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Avalonia.Thickness(2, 2, 2, 2)
            };

            mainPanel.Children.Add(firstRow);
            mainPanel.Children.Add(secondRow);
            mainPanel.Children.Add(thirdRow);
            mainPanel.Children.Add(fourthRow);
            mainPanel.Children.Add(fifthRow);
            
        }

        private void InitFirstRow()
        {
            for (int i = 0; i< firstRowDefault.Length; i++)
            {
                var button = new KeyboardKey
                {
                   PrimaryText = firstRowDefault[i],
                   SecondaryText = firstRowShift[i],
                   Margin = new Avalonia.Thickness(2, 2, 2, 2) 
                };
                button.KeyPressed += (sender, text) =>
                {
                    // Forward event to parent control
                    KeyPressed?.Invoke(this, text);
                };
                firstRow.Children.Add(button);
                firsRowButtons.Add(button);
            }

            var backBtn = new Button
            {
                Content = "<---",
                Width = 100,
                Height = 60,
                Margin = new Avalonia.Thickness(2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            backBtn.Click += (sender, e) =>
            {
                KeyPressed?.Invoke(this, "back");
            };
            firstRow.Children.Add(backBtn);
        } 
        private void InitSecondRow()
        {
            for (int i = 0; i < secondRowDefault.Length; i++)
            {
                var button = new KeyboardKey
                {
                    PrimaryText = secondRowDefault[i],
                    SecondaryText = secondRowShift[i],
                    Margin = new Avalonia.Thickness(2, 2, 2, 2),
                };
                button.KeyPressed += (sender, text) =>
                {
                    // Forward event to parent control
                    KeyPressed?.Invoke(this, text);
                };
                secondRow.Children.Add(button);
                secondRowButtons.Add(button);
            }
        } 
        private void InitThirdRow()
        {
            for (int i = 0; i < thirdRowDefault.Length; i++)
            {
                var button = new KeyboardKey
                {
                    PrimaryText = thirdRowDefault[i],
                    SecondaryText = thirdRowShift[i],
                    Margin = new Avalonia.Thickness(2, 2, 2, 2)
                };
                button.KeyPressed += (sender, text) =>
                {
                    // Forward event to parent control
                    KeyPressed?.Invoke(this, text);
                };
                thirdRow.Children.Add(button);
                thirdRowButtons.Add(button);
            }
            var enterBtn = new Button
            {
                Content = "Enter",
                Width = 180,
                Height = 60,
                Margin = new Avalonia.Thickness(2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            enterBtn.Click += (sender, e) =>
            {
                KeyPressed?.Invoke(this, "enter");
            };
            thirdRow.Children.Add(enterBtn);
        } 
        private void InitFourthRow()
        {
            var shiftBtn = new Button
            {
                Content = "Shift",
                Width = 90,
                Height = 60,
                Margin = new Avalonia.Thickness(2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            fourthRow.Children.Add(shiftBtn);

            shiftBtn.Click += Button_Shift;

            for (int i = 0; i < fourthRowDefault.Count; i++)
            {
                var button = new KeyboardKey
                {
                    PrimaryText = fourthRowDefault[i],
                    SecondaryText = fourthRowShift[i],
                    Margin = new Avalonia.Thickness(2, 2, 2, 2)
                };
                button.KeyPressed += (sender, text) =>
                {
                    // Forward event to parent control
                    KeyPressed?.Invoke(this, text);
                };
                fourthRow.Children.Add(button);
                fourthRowButtons.Add(button);
               
            }
        }
        private void InitFifthRow()
        {
            var altBtn = new Button
            {
                Content = "Alt",
                Width = 110,
                Height = 60,
                Margin = new Avalonia.Thickness(2, 2,2,2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
            };       
            fifthRow.Children.Add(altBtn);
            altBtn.Click += Button_Alt;


            var spaceBtn = new Button
            {
                Content = "Space",
                Width = 300,
                Height = 60,
                Margin = new Avalonia.Thickness(186, 2,2,2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
               
            var leftBtn = new Button
            {
                Content = "<",
                Width = 60,
                Height = 60,
                Margin = new Avalonia.Thickness(130, 2, 2, 2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            var rightBtn = new Button
            {
                Content = ">",
                Width = 60,
                Height = 60,
                Margin = new Avalonia.Thickness(10, 2, 2, 2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            spaceBtn.Click += (sender, e) =>
            {
                KeyPressed?.Invoke(this, " ");
            };
            leftBtn.Click += (sender, e) =>
            {
                KeyPressed?.Invoke(this, "left");
            };
            rightBtn.Click += (sender, e) =>
            {
                KeyPressed?.Invoke(this, "right");
            };
            fifthRow.Children.Add(spaceBtn);
            fifthRow.Children.Add(leftBtn);
            fifthRow.Children.Add(rightBtn);
   
        }

        private void OnKeyPressed(object sender, string text)
        {
            Console.WriteLine($"Key Pressed: {text}");
            // Tutaj możesz zrobić coś z tekstem, np. dodać go do pola tekstowego
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Console.WriteLine($"Kliknięto przycisk: {button.Content}");
        } 
        private void Button_Alt(object sender, RoutedEventArgs e)
        {
            altState = !altState;
            foreach (var a in secondRowButtons)
            {
                a.UpdateText(altState);
            }
            foreach (var a in thirdRowButtons)
            {
                a.UpdateText(altState);
            }
            foreach (var a in fourthRowButtons)
            {
                a.UpdateText(altState);
            }

        }

        private void Button_Shift(object sender, RoutedEventArgs e)
        {
            shiftState = !shiftState;
            foreach (var a in firsRowButtons)
            {
                a.UpdateText(shiftState);
            }
            foreach (var a in secondRowButtons)
            {
                a.ToggleFontSize();
            }
            foreach (var a in thirdRowButtons)
            {
                a.ToggleFontSize();
            }

            foreach (var a in fourthRowButtons)
            {
                a.ToggleFontSize();
            }

        }

        private void Button_Space(object sender, RoutedEventArgs e)
        {
            
        }

        private void InitializeBackspace()
        {
            var button = new Button
            {
                Content = "<---",
                Width = 90,
                Height = 60,
                Margin = new Avalonia.Thickness(2, 2, 2, 2)
            };
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, 13);
            keyboardGrid.Children.Add(button);
            button.Click += Backspace_Click;
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("BACKSPACE");
        }


    }
}