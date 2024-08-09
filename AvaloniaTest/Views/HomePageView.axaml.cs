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



namespace AvaloniaTest.Views
{

    public partial class HomePageView : UserControl
    {

    

    
        public HomePageView()
        {
            InitializeComponent();
            //InitGrid();
            slideMenu();

            //ZROBIONE DLA TESTA 
            var scrollRecognizer = new ScrollGestureRecognizer
            {
                CanVerticallyScroll = true,  
                CanHorizontallyScroll = true
            };

            HomePagePanel.GestureRecognizers.Add(scrollRecognizer);
            HomePagePanel.PointerReleased += Next;
            //KONIEC TESTA


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
       
    }

   
}

