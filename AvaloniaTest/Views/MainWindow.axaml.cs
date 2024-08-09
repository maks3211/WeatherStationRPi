using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaTest.ViewModels;
using LiveChartsCore.VisualElements;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace AvaloniaTest.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        
        private bool _isButtonHeld = false;
        private DateTime _startTime;
        public static OnScreenKeyboard keyboard;
        public MainWindow()
        {
            //Tryp pelno-ekranowy
            //this.WindowState = WindowState.FullScreen;
            //this.Topmost = true;
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            InitializeComponent();
           // this.DataContext = new MainWindowViewModel();

           
            //changeThemeButton.Content = "zmiana";
            //changeThemeButton.PointerPressed += changeThemeButton_OnPointerPressed;
            //  changeThemeButton.PointerReleased += changeThemeButton_OnPointerReleased;


          changeThemeButton.AddHandler(PointerPressedEvent, changeThemeButton_OnPointerPressed, RoutingStrategies.Tunnel);
          changeThemeButton.AddHandler(PointerReleasedEvent, changeThemeButton_OnPointerReleased, RoutingStrategies.Tunnel);

          this.AddHandler(PointerPressedEvent, OnWindowClick, handledEventsToo: true);
            keyboard = new OnScreenKeyboard(KeyboardFrame);
            OnScreenKeyboardBehavior.keyboard = keyboard;
            KeyboardFrame.Children.Add(keyboard.GetKeyBoardStackPanel());
            

        }
        private void ButtonOnClick(object? sender, RoutedEventArgs e)
        {
             this.Close();
        }

        private void changeThemeButton_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {     
            _isButtonHeld = true;
            _startTime = DateTime.Now;
            CheckButtonHold();
           
        }

   

        private void changeThemeButton_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isButtonHeld = false;
      
        }
        
        private async void CheckButtonHold()
        {
            if (_viewModel is null && DataContext is MainWindowViewModel viewModel)
            {
                _viewModel = viewModel;
            }
                Console.WriteLine("licznenie");
            while (_isButtonHeld)
            {
                await Task.Delay(100); // SprawdŸ co 100 ms
                var elapsedTime = DateTime.Now - _startTime;
                if (elapsedTime.TotalMilliseconds >= 1000) // Czy minê³o 3 sekundy?
                {
                    // Tutaj wykonaj coœ po przytrzymaniu przycisku przez 3 sekundy
                    // Na przyk³ad wywo³aj odpowiedni¹ metodê lub zmieñ stan aplikacji
                    Console.WriteLine("Przytrzymano przycisk przez 3 sekundy!");
                   // _viewModel.handleGoToSettgins();
                   // if (_viewModel is null && DataContext is MainWindowViewModel viewModel)
                  //  {
                      //  _viewModel = viewModel;
                   //     _viewModel.handleGoToSettgins();
                   // }
                    if (_viewModel is not null)
                    {
                        _viewModel.handleGoToSettgins();
                    }
                    break;
                }
                
            }
            if (_viewModel is not null && !_isButtonHeld)
            {
                _viewModel.ChangeTheme();
            }
        }




        private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }

        
        private void ThemeBtnReleased(object sender, PointerPressedEventArgs e)
        {
            // Obs³uga wciœniêcia przycisku
            Console.WriteLine("ressd");
        }


        private void OnWindowClick(object sender, PointerPressedEventArgs e)
        {
            // SprawdŸ, czy klikniêcie by³o poza obszarem StackPanel
            if (!IsClickInsideElement(KeyboardFrame, e) && keyboard.GetIsVisable())
            {
                //   Ramkadwa.IsVisible = false;
                // OnScreenKeyboard.isVisable = false;
                // klawa.UpdateVisibility();
                keyboard.Close();
            }
        }

        private bool IsClickInsideElement(Avalonia.Controls.StackPanel element, PointerPressedEventArgs e)
        {
           // if (keyboard.GetIsVisable())
          //  {
                var position = e.GetPosition(element);
                return position.X >= 0 && position.X <= element.Bounds.Width &&
                       position.Y >= 0 && position.Y <= element.Bounds.Height;
           // }
           // return true;
          

        
        }

    }
}