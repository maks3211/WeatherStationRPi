using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using System;
using System.Drawing;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Animation;
using Avalonia.Layout;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using AvaloniaTest.Models.WeatherForecast;
using AvaloniaTest.ViewModels;
using LiveChartsCore.VisualElements;



namespace AvaloniaTest.Views
{

    public partial class HomePageView : UserControl
    {

        private HomePageViewModel ViewModel => DataContext as HomePageViewModel;
      //  public WeatherForecastViewModel weatherModel;
        //private HomePageViewModel viewModel;


        public HomePageView()
        {
            InitializeComponent();
            //InitGrid();
        
          //  DataContext = new HomePageViewModel(); -> TO NIE MOZE BYC BO SIE DWA RAZY TWORZY VIEWMODEL !!
            slideMenu();
            
         //   weatherModel = new WeatherForecastViewModel(WeatherBorder, WeatherStackPanel);

            


        }
       
        private void OnBorderPointerPressed(object sender, PointerPressedEventArgs e)
        {
         
           // weatherModel.Open();
           // e.Handled = true; //oznacza ze zdarzenie zostalo obsluzone i dalej nie ma byc juz brane pod uwage 

        }


        private void PagePressed(object sender, PointerPressedEventArgs e)
        {
           // Console.WriteLine("Klikniêto w POLE");
          //  if (e.Source is Border border && border.Name == "WeatherBorder")
          //  {
   
             //   return;
            //}
           
           // weatherModel.Close();
        }



        private void slideMenu()
        {
            var compisitePageTransition = new CompositePageTransition();
            var pageSlide = new PageSlide
            {
               Duration = TimeSpan.FromSeconds(0.2), // Ustawiamy czas trwania na 1.5 sekundy
               Orientation = Avalonia.Animation.PageSlide.SlideAxis.Horizontal // Ustawiamy orientacjê na poziom¹

            };
            compisitePageTransition.PageTransitions.Add(pageSlide);

            // Ustawiamy w³aœciwoœæ PageTransition dla Carousel
            SlideMenu.PageTransition = compisitePageTransition;    
        }


 

        public void Next(object source, RoutedEventArgs args)
        {
            SlideMenu.Next();
            
        }

        public void Previous(object source, RoutedEventArgs args)
        {
            SlideMenu.Previous();
            
        }

        private void Dotyk(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            // SprawdŸ, czy klikniêto w obszarze Border
            var border = sender as StackPanel;
            if (border != null)
            {
                // Twoja logika, która ma byæ wykonana po klikniêciu
                Console.WriteLine("Klikniêto w Border");
            }
        }


    }

   
}

