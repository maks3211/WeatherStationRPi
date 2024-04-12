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


namespace AvaloniaTest.Views
{

    public partial class HomePageView : UserControl
    {

        private int rows = 5;
        private int columns = 5;    
        public HomePageView()
        {
            InitializeComponent();
            //InitGrid();
            slideMenuTest();
        }
       
        private void slideMenuTest()
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
           

            var myCanvas = new Canvas();
            myCanvas.HorizontalAlignment = HorizontalAlignment.Center;
            myCanvas.Width = 1024;
            myCanvas.Height = 470;

            var border = new Avalonia.Controls.Shapes.Rectangle
            {
                Width = myCanvas.Width,
                Height = myCanvas.Height,
               // Stroke = Brushes.Black, // Kolor obramowania
               // StrokeThickness = 1, // Gruboœæ obramowania
            };

            // Dodajemy prostok¹t (ramkê) do Canvas
            myCanvas.Children.Add(border);
            


            Application.Current!.TryFindResource("ClockRegular", out var res);


           

            DateTime currentTime = DateTime.Now;

            // Teraz mo¿emy uzyskaæ godzinê, minutê i sekundê z currentTime

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");
            string dayName = currentDate.ToString("dddd");
            string dzien = char.ToUpper(dayName[0]) + dayName.Substring(1);
            var napis = new TextBlock
            {
                Text = dzien,
                FontSize = 30,
                Foreground = Brushes.Navy,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

 

            // Uruchamiamy timer
            timer.Start();

        
            var clockIcon = new PathIcon {
            Data = (StreamGeometry)res!,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10),
            };


            var firstRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height =60,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
               // Background = new SolidColorBrush(Colors.Goldenrod),

            };
            var secondRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height =140,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
               // Background = new SolidColorBrush(Colors.Gray),

            };
            ThemeVariant theme = Application.Current!.ActualThemeVariant;
           
            Application.Current!.Resources.TryGetResource("FontBlue", theme, out var kolorek);
            var napis2 = new TextBlock
            {
                Text = "",
                FontSize = 40,
                Foreground = (IBrush)kolorek!,
                FontWeight = FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
            };
            firstRow.Children.Add(clockIcon);
            firstRow.Children.Add(napis);
            Console.WriteLine(Application.Current!.ActualThemeVariant.ToString());
           
            
            Application.Current!.ActualThemeVariantChanged += (sender, newTheme) =>
            {             
                // Pobierz aktualny motyw
                var currentTheme = Application.Current!.ActualThemeVariant;
                // SprawdŸ, czy uda³o siê pobraæ zasób dla aktualnego motywu
                if (Application.Current!.Resources.TryGetResource("FontBlue", currentTheme, out var kolorek))
                {
                    // Tutaj dokonaj aktualizacji koloru czcionki dla odpowiednich elementów interfejsu u¿ytkownika
                    // Na przyk³ad TextBox.Foreground = kolorek;
                    napis2.Foreground = (IBrush)kolorek;
                }
            };
            Console.WriteLine(theme.ToString());
            

           

            var uri = "avares://AvaloniaTest/Assets/Images/sun.png"; 

            var bitmap = new Bitmap(AssetLoader.Open(new Uri(uri)));
           
            var zdj = new Image {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Source = bitmap,
                Width =40,
                Height =40,
              
            };


            timer.Tick += (sender, e) =>
            {
                // Pobieramy aktualn¹ godzinê z systemu
                DateTime currentTime = DateTime.Now; ;
                string formattedHour = currentTime.Hour.ToString("D2");
                string formattedMinute = currentTime.Minute.ToString("D2");
                string formattedSeconds = currentTime.Second.ToString("D2");
                // Aktualizujemy tekst w TextBlock
                napis2.Text = $"{formattedHour}:{formattedMinute}:{formattedSeconds}";
            };

            secondRow.Children.Add(napis2);
            secondRow.Children.Add(zdj);

            var cos = new TextBlock {
                
            Text = $"{formattedDate}",
            VerticalAlignment = VerticalAlignment.Bottom,
           HorizontalAlignment = HorizontalAlignment.Center,
              
            };

            var thirdRow = new StackPanel {
                Height = 50,
              

                 // Background = new SolidColorBrush(Colors.MediumSlateBlue),
            };
            thirdRow.Children.Add(cos);
            


            var mainPanel = new StackPanel {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            int a = 200;
            var border2 = new Border
            {
                Margin = new Avalonia.Thickness(10),
                //VerticalAlignment = VerticalAlignment.Top,  //nie ma znaczenia odnosi sie do pozycji wzgledem parenta
                //HorizontalAlignment = HorizontalAlignment.Left,
                Width = a,
                Height = 250,
                // Ustawienia dla ramki
                BorderBrush = Avalonia.Media.Brushes.Yellow,
                BorderThickness = new Avalonia.Thickness(3), // Gruboœæ ramki (mo¿esz dostosowaæ)
                CornerRadius = new Avalonia.CornerRadius(8), // Zaokr¹glenie rogów ramki (opcjonalnie)
               // Background = new SolidColorBrush(Avalonia.Media.Colors.Brown),
                Child = mainPanel // Dodanie StackPanel jako dziecko ramki
                
            };
            
        
            var separatorLine = new Avalonia.Controls.Shapes.Rectangle
            {
                Width = 200, // Szerokoœæ linii oddzielaj¹cej (mo¿esz dostosowaæ)
                Height = 3, // Wysokoœæ linii oddzielaj¹cej
                Fill = Brushes.Black // Kolor linii oddzielaj¹cej (mo¿esz dostosowaæ)
            };

            mainPanel.Children.Add(firstRow);
            mainPanel.Children.Add(separatorLine);
            mainPanel.Children.Add(secondRow);
            mainPanel.Children.Add(thirdRow);
            Widget widget = new Widget(Brushes.Yellow);
            //widget.addElement
            Element element1 = new Element("Small");
            var napis23 = new TextBlock
            {
                Text = "ABC",
                FontSize = 40,
                Foreground = (IBrush)kolorek!,
                FontWeight = FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
            };
            var napis3 = new TextBlock
            {
                Text = "dwa",
                FontSize = 40,
                Foreground = (IBrush)kolorek!,
                FontWeight = FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
            };

            element1.addItem(napis23);
            element1.addItem(napis3);
            ((TextBlock)element1.elements[0]).Text = "abccc";
            Element elment2 = new Element();
            var napis4 = new TextBlock
            {
                Text = "trzy",
                FontSize = 40,
                Foreground = (IBrush)kolorek!,
                FontWeight = FontWeight.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
            };

            elment2.addItem(napis4);
            widget.AddElement(element1);
            widget.AddElement(elment2);

            myCanvas.Children.Add(border2);
    
            Canvas.SetTop(border2, 0);
            Canvas.SetLeft(border2, 300);

            myCanvas.Children.Add(widget);
            Canvas. SetTop(widget, 0);
            Canvas.SetLeft(widget, 0);

            //SlideMenu.Items.Add(widget);
            SlideMenu.Items.Add(myCanvas);
       
           
           
           

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

