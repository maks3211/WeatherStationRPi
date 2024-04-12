using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Xml.Linq;


//DOMYŚLNE NA KOŃCU !!!!
namespace AvaloniaTest.Views
{
    internal class Widget : Border
    {    
      //Cała reszta jest dziedziczona 
        private StackPanel _cointeiner;
        private int childsNum = 0;
        public StackPanel Cointeiner
        {
            get { return _cointeiner; }
            set { _cointeiner = value; }
        }


        public Widget(Avalonia.Media.IBrush color, int width = 200, int height = 0) //trzeba podać kolor podczas tworzenia + wywołanie metody (wewnątrz widoku) gdy jest zmiana koloru 
        {

            var boxShadow = new BoxShadow
            {
                Spread = 2,
                OffsetX = 5,             
                OffsetY = 5,             
                Blur = 8,
                Color = Avalonia.Media.Colors.Gray    
            };
            var boxShadows = new BoxShadows(boxShadow);
            Width = width;
            Height = height;
            Margin = new Thickness(10);
            BorderBrush = color;
            BorderThickness = new Thickness(3);
            BoxShadow = boxShadows;
            CornerRadius = new CornerRadius(8);
            _cointeiner = new StackPanel { 
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            };    
            Child = _cointeiner;

        }
        /// <summary>
        /// <c>AddElement</c> adds new element into Widget
        /// </summary>
        /// <param name="child">is StackPanel with other elemnts e.g. Text, icons etc.</param>
        public void AddElement(StackPanel child)
        {
            if (childsNum == 1) // po pierwszym dodaj linie
            {
                var separatorLine = new Avalonia.Controls.Shapes.Rectangle
                {
                    Width = 200, // Szerokość linii oddzielającej (możesz dostosować)
                    Height = 3, // Wysokość linii oddzielającej
                    Fill = BorderBrush // Kolor linii oddzielającej (możesz dostosować)
                };
                _cointeiner.Children.Add(separatorLine);
            }
            _cointeiner.Children.Add(child);
            childsNum++;
            Height += child.Height;
        }

    }

     //pojedyńczy wiersz -> Element wchodzi w skład Widget który jest całością 
    internal class Element : StackPanel
    {

        //Tablica z elementami które beda dodawane z metody 
        public List<object> elements = new List<object>();
        /// <summary>
        /// <c>Element</c> Creates new Element
        /// </summary>
        /// <param name="type">is used to set Height: 'Small'->60, 'Medium'->140 (defualt) ,'Large'->200, bad sets to 140</param>
        public Element(string type = "Medium")
        {
            Orientation = Orientation.Horizontal;
            switch (type)
            {
                case "Small":
                    Height =60;
                    break;
                case "Medium":
                    Height = 140;
                    break;
                case "Large":
                    Height = 200;
                    break;
                default:
                    try
                    {
                        Height = Int32.Parse(type);
                    }
                    catch (FormatException e) { 
                    Console.WriteLine(e.Message + "height is set to 140");
                    Height = 140;
                    }
                    break;
            }
          

            Application.Current!.ActualThemeVariantChanged += (sender, newTheme) =>
            {
                ThemeVariant theme = Application.Current!.ActualThemeVariant;
                Application.Current!.Resources.TryGetResource("FontBlue", theme, out var kolorek);
                foreach (var element in elements)
                {
                    if (element is TextBlock textBlock) // tylko dla textBlock
                    {
                        textBlock.Foreground = (IBrush)kolorek!; 
                    }
                }
              
            };


        }

        public void addItem(Control obj)
        {
            Console.WriteLine("Dodanie");
        elements.Add(obj);
        Children.Add((Control)elements.Last());
        }


    }
}
