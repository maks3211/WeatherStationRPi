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
using Org.BouncyCastle.Asn1.Ocsp;
using Avalonia.Media;
using Avalonia.Animation;
using System.ComponentModel.DataAnnotations;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

using Avalonia;
using System.Xml.Linq;
using System.Diagnostics;

namespace AvaloniaTest.Views
{
    public class OnScreenKeyboard : UserControl
    {
        private  bool isVisable = false;
        private TextBox _associatedTextBox;
        private Button _associatedEnterButton;

        private Button enterBtn;
        private Button backBtn;
        private Button leftBtn;
        private Button rightBtn;
        private Button spaceBtn;
        private Button altBtn;
        private Button shiftBtn;

      //  private TextBox Ramka;
      //  private TextBlock textBoxName;

        private bool shiftState = false;
        private bool altState = false;


        private event EventHandler<string> KeyPressed;

        private StackPanel mainPanel;

        private readonly StackPanel framePanel;
        private bool ready = true;

        private ObservableCollection<string> FourthRowDefault { get; } = new()
        {
            "z", "x", "c", "v", "b", "n", "m", ",", ".", "/"
        };


        private List<KeyboardKey> firsRowButtons = new List<KeyboardKey>();
        private StackPanel firstRow;
        private readonly string[]  firstRowDefault = [ "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" ];
        private readonly string[]  firstRowShift = [ "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+" ];

        private StackPanel secondRow;
        private readonly List<KeyboardKey> secondRowButtons = new List<KeyboardKey>();
        private readonly string[] secondRowDefault = ["q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\"];
        private readonly string[] secondRowShift = ["", "", "ę", "", "","", "€", "", "ó", "", "", "", "",];

        private StackPanel thirdRow;
        private readonly List<KeyboardKey> thirdRowButtons = new List<KeyboardKey>();
        private readonly string[] thirdRowDefault = ["a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'"];
        private readonly string[] thirdRowShift = ["ą", "ś", "", "", "", "", "", "", "ł", "", ""];

        private StackPanel fourthRow;
        private readonly string[]  fourthRowShift = ["ż", "ź", "ć", "", "", "ń", "", "", "", ""];       
        private readonly List<KeyboardKey> fourthRowButtons = new List<KeyboardKey>();

        private StackPanel fifthRow;

        private StackPanel textBoxPanel;

        public OnScreenKeyboard(StackPanel frame)
        {
            Application.Current.ActualThemeVariantChanged += Current_ActualThemeVariantChanged;
            framePanel = frame;
           
            InitializeGrid();

            // Ramka = new TextBox();
            
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
            if (framePanel is not null)
            {
                framePanel.IsVisible = isVisable;
            }

            SetColors();
        }


        private void SetColors()
        {
        if (Application.Current.ActualThemeVariant == ThemeVariant.Light)
            {
                var color = new SolidColorBrush(Color.Parse("#D3D3D3"));
                enterBtn.Background = color;
                leftBtn.Background = color;
                rightBtn.Background = color;
                spaceBtn.Background = color;
                altBtn.Background = color;
                shiftBtn.Background = color;
                backBtn.Background = color;

            }
            else 
            {
               var color = new SolidColorBrush(Color.Parse("#3A3A3E"));
                enterBtn.Background = color;
                leftBtn.Background = color;
                rightBtn.Background = color;
                spaceBtn.Background = color;
                altBtn.Background = color;
                shiftBtn.Background = color;
                backBtn.Background = color;
            }
        }
        private void Current_ActualThemeVariantChanged(object? sender, EventArgs e)
        {      
                SetColors(); 
        }

        public void SetAssociatedTextBox(TextBox textBox)
        {
            _associatedTextBox = textBox;
        }
         public void SetAssociatedEnterButton(Button button)
        {
            _associatedEnterButton = button;
            if (_associatedEnterButton is not null)
            {
              
                var napius = _associatedEnterButton.Content;
                enterBtn.Content = napius;
            }
            else {
                Console.WriteLine("Keybaord - enter btn not set");
                enterBtn.Content = "Enter";
            }

            //  var args = new RoutedEventArgs(Button.ClickEvent);
            // _associatedEnterButton.RaiseEvent(args);
            //  _associatedEnterButton.Command.Execute(null);
        }

        public void HandleKeyPress(string text)
        {

           

            if (_associatedTextBox == null) return;
            _associatedTextBox.Focus();
            int lineNumber = _associatedTextBox.CaretIndex;

            if (text.Length > 1)
            {
                if (text == "back")
                {
                    if (_associatedTextBox.Text is not null && _associatedTextBox.Text.Length > 0 && _associatedTextBox.CaretIndex > 0)
                    {
                        _associatedTextBox.Text = _associatedTextBox.Text.Remove(_associatedTextBox.CaretIndex - 1, 1);
                        lineNumber--;
                    }
                }
                else if (text == "right")
                {
                    if (_associatedTextBox.CaretIndex < _associatedTextBox.Text.Length)
                    {
                        lineNumber++;
                    }
                }
                else if (text == "left")
                {
                    if (_associatedTextBox.CaretIndex > 0)
                    {
                        lineNumber--;
                    }
                }
                else if (text == "enter")
                {
                    if (_associatedEnterButton is not null)
                    {
                        _associatedEnterButton?.Command?.Execute(null);
                        Close();
                    }
                    
                }
            }
            else
            {
                string currentText = _associatedTextBox.Text ?? string.Empty;
                int caretIndex = _associatedTextBox.CaretIndex;
                string newText = currentText.Insert(caretIndex, text);
                _associatedTextBox.Text = newText;
                _associatedTextBox.CaretIndex = caretIndex + text.Length;
                lineNumber++;
            }

            _associatedTextBox.CaretIndex = lineNumber;




           

            // Ramka.Text = _associatedTextBox.Text;
        }

        public StackPanel GetKeyBoardStackPanel()
        {
            return mainPanel;
        }

        public async  void Close()
        {
            if (!isVisable) return;
            if (!ready) return;
            ready = false;
            if (_associatedTextBox != null)
            {
                _associatedTextBox.IsVisible = true;
            }
           

            var animation = new Avalonia.Animation.Animation
            {
                FillMode = FillMode.Forward,
                Duration = TimeSpan.FromMilliseconds(300), 
                Easing = new SineEaseInOut(), 
                Children =
            {
               new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Visual.OpacityProperty, 0.0) }, // Docelowa wartość Opacity
                    Cue = new Cue(1d) // Na końcu animacji (1.0) ustaw Opacity na 1.0
                },
                 new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Control.MarginProperty, new Thickness(40, 0, 0, 0)) }, 
                    Cue = new Cue(0.65d) 
                },
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Control.MarginProperty, new Thickness(40, 180, 0, 0)) }, 
                    Cue = new Cue(1d) 
                }
            }
            };
          
            await animation.RunAsync(framePanel);           
            isVisable = false;
            framePanel.IsVisible = isVisable;
            ready = true;
            
        }
        public async void Open()
        {

            if (isVisable) return;
            if (!ready) return;
            ready = false;
            isVisable = true;
            framePanel.IsVisible = isVisable;
  
         
            //PRZESUWANIE POLA TEKSOTWEGO:



            // _associatedTextBox.Margin = new Thickness(_associatedTextBoxMargin.Left + 10, _associatedTextBoxMargin.Top, _associatedTextBoxMargin.Right, _associatedTextBoxMargin.Bottom);




            //  Console.WriteLine($"MOJA pozycja: {_associatedTextBox.RenderTransform}");
            //  var translateTransform = new TranslateTransform();
            //  _associatedTextBox.RenderTransform = translateTransform;
            // translateTransform.X = 100;
            // translateTransform.Y = 0;

            // var position = _associatedTextBox.PointToScreen(new Point(0d, 0d));




            var animation = new Avalonia.Animation.Animation
            {
                FillMode = FillMode.Forward,
                Duration = TimeSpan.FromMilliseconds(300), 
                Easing = new SineEaseInOut(), 
                Children =
            {
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Visual.OpacityProperty, 1.0) }, 
                    Cue = new Cue(1d) 
                },
                  new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Control.MarginProperty, new Thickness(40, 0, 0, 0)) }, 
                    Cue = new Cue(0.65d) 
                },
                    new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Control.MarginProperty, new Thickness(40, -220, 0, 0)) }, 
                    Cue = new Cue(1d) 
                }
            }
            };

            await animation.RunAsync(framePanel);
            ready = true;
        }
        public bool GetIsVisable()
        {
            return isVisable;
        }
       
        private void InitializeGrid()
        {
            mainPanel = new StackPanel();
            mainPanel.Opacity = 1;
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
                    HandleKeyPress(text);
                };
                firstRow.Children.Add(button);
                firsRowButtons.Add(button);
            }

            backBtn = new Button
            {
                Content = "<---",
                Width = 100,
                Height = 60,
                Margin = new Avalonia.Thickness(2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#D3D3D3"))
            };

            backBtn.Click += (sender, e) =>
            {
                HandleKeyPress("back");
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
                    HandleKeyPress(text);
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
                    HandleKeyPress(text);
                };
                thirdRow.Children.Add(button);
                thirdRowButtons.Add(button);
            }

            enterBtn = new Button
            {
                Content = "Enter",
                Width = 180,
                Height = 60,
                Margin = new Avalonia.Thickness(2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                
             };

          
       


            enterBtn.Click += (sender, e) =>
            {
                HandleKeyPress("enter");
            };
            thirdRow.Children.Add(enterBtn);
        } 
        private void InitFourthRow()
        {
            shiftBtn = new Button
            {
                Content = "Shift",
                Width = 90,
                Height = 60,
                Margin = new Avalonia.Thickness(2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#D3D3D3"))
            };
            fourthRow.Children.Add(shiftBtn);

            shiftBtn.Click += Button_Shift;

            for (int i = 0; i < FourthRowDefault.Count; i++)
            {
                var button = new KeyboardKey
                {
                    PrimaryText = FourthRowDefault[i],
                    SecondaryText = fourthRowShift[i],
                    Margin = new Avalonia.Thickness(2, 2, 2, 2),            
                };
                button.KeyPressed += (sender, text) =>
                {
                    // Forward event to parent control
                    HandleKeyPress(text);
                };
                fourthRow.Children.Add(button);
                fourthRowButtons.Add(button);
               
            }
        }
        private void InitFifthRow()
        {
            altBtn = new Button
            {
                Content = "Alt",
                Width = 110,
                Height = 60,
                Margin = new Avalonia.Thickness(2, 2,2,2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#D3D3D3"))
            };       
            fifthRow.Children.Add(altBtn);
            altBtn.Click += Button_Alt;


             spaceBtn = new Button
            {
                Content = "Space",
                Width = 300,
                Height = 60,
                Margin = new Avalonia.Thickness(186, 2,2,2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#D3D3D3")),
            };
               
             leftBtn = new Button
            {
                Content = "<",
                Width = 60,
                Height = 60,
                Margin = new Avalonia.Thickness(130, 2, 2, 2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#D3D3D3")),
            };
             rightBtn = new Button
            {
                Content = ">",
                Width = 60,
                Height = 60,
                Margin = new Avalonia.Thickness(10, 2, 2, 2),
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#D3D3D3")),
            };
            spaceBtn.Click += (sender, e) =>
            {
                KeyPressed?.Invoke(this, " ");
                HandleKeyPress(" ");
            };
            leftBtn.Click += (sender, e) =>
            {
              //  KeyPressed?.Invoke(this, "left");
                HandleKeyPress("left");
            };
            rightBtn.Click += (sender, e) =>
            {
              //  KeyPressed?.Invoke(this, "right");
                HandleKeyPress("right");
            };
            fifthRow.Children.Add(spaceBtn);
            fifthRow.Children.Add(leftBtn);
            fifthRow.Children.Add(rightBtn);
   
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

    }
}