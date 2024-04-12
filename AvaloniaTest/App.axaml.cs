using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaTest.ViewModels;
using AvaloniaTest.Views;
using System;
using System.Globalization;

namespace AvaloniaTest
{
    public partial class App : Application
    {
        public event EventHandler<string> CurrentPageOpend;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine("WINDOWS");
                desktop.MainWindow = new MainWindow
                {
                    
                    DataContext = new MainWindowViewModel(),
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                Console.WriteLine("Linux");
                singleView.MainView = new MainWindow //
               {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
        public void ChangeTheme()
        {
            if (RequestedThemeVariant == ThemeVariant.Dark)
            {
                RequestedThemeVariant = ThemeVariant.Light;
            }
            else {
                RequestedThemeVariant = ThemeVariant.Dark;
            }
          
            
        }
    }

    
}