using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaTest.ViewModels;
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

        public MainWindow()
        {
            
            //this.WindowState = WindowState.FullScreen;
           // this.Topmost = true;
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            InitializeComponent();
           // this.DataContext = new MainWindowViewModel();

           
            //changeThemeButton.Content = "zmiana";
            //changeThemeButton.PointerPressed += changeThemeButton_OnPointerPressed;
            //  changeThemeButton.PointerReleased += changeThemeButton_OnPointerReleased;


          changeThemeButton.AddHandler(PointerPressedEvent, changeThemeButton_OnPointerPressed, RoutingStrategies.Tunnel);
          changeThemeButton.AddHandler(PointerReleasedEvent, changeThemeButton_OnPointerReleased, RoutingStrategies.Tunnel);


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
                await Task.Delay(100); // Sprawd� co 100 ms
                var elapsedTime = DateTime.Now - _startTime;
                if (elapsedTime.TotalMilliseconds >= 1000) // Czy min�o 3 sekundy?
                {
                    // Tutaj wykonaj co� po przytrzymaniu przycisku przez 3 sekundy
                    // Na przyk�ad wywo�aj odpowiedni� metod� lub zmie� stan aplikacji
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
            // Obs�uga wci�ni�cia przycisku
            Console.WriteLine("ressd");
        }

    }
}