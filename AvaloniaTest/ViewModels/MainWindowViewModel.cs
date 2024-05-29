using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaTest.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AvaloniaTest.ViewModels
{
    /// <summary>
    /// View model class for the main window.
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {

        public static event EventHandler<string> CurrentPageOpened;
        public static string CurrentPageSub = "";
        public  static InDoorSensor inDoorSens = new InDoorSensor();
        public static MQTTcommunication mqqt = new MQTTcommunication();
        public static string lastPage = "";
        public static Network siec = new Network();

        [ObservableProperty]
        public ViewModelBase _currentPage = new HomePageViewModel();
        
        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;

        /// <summary>
        /// List of navigation items.
        /// </summary>
        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        {
            new ListItemTemplate(typeof(HomePageViewModel),"HomeRegular", "Strona Główna"),
            new ListItemTemplate(typeof(SettingsViewModel),"SettingsRegular","Ustawienia"),
            new ListItemTemplate(typeof(ChartViewModel),"ChartRegular","Wykresy"),
        };

        /// <summary>
        /// Constructor for the MainWindowViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {
            
            CurrentPageSub = "AvaloniaTest.ViewModels.HomePageViewModel";
            CurrentPageOpened?.Invoke(this, CurrentPageSub ?? "");

            StartDataReading();
            mqqt.Start_Server();
            
        }

        /// <summary>
        /// Method to start server.
        /// </summary>
        static async Task StartSERWER()
        {
            await mqqt.Start_Server();
        }

        /// <summary>
        /// Method to start reading data.
        /// </summary>
        public async Task StartDataReading()
        {            
            Task task1 = inDoorSens.RunReadData();
            await Task.WhenAll(task1);
        }

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value == null) return;
            var istance = Activator.CreateInstance(value.ModelType);
            if (istance == null) return;
            CurrentPage = (ViewModelBase)istance;
            CurrentPageSub = CurrentPage.ToString();
           // Console.WriteLine("TUTAJ JEST TA STREONA:" + CurrentPageSub);
          //  Console.WriteLine("TUTAJ JEST TA STREONA druga:" + Items.FirstOrDefault().ModelType.Name.ToString());        
            CurrentPageOpened?.Invoke(this, CurrentPageSub ?? "");
           
            
           
        }

        /// <summary>
        /// Method to start reading WIFI data.
        /// </summary>
        public async Task StartWifiReading() {
            Task t1 = siec.GetWifiList();      
            await Task.WhenAll(t1);
        }

        /// <summary>
        /// Method to change the application theme.
        /// </summary>
        [RelayCommand]
        public void ChangeTheme()
        {
            App app = (App)Application.Current;
            app.ChangeTheme();
        }

        /// <summary>
        /// Method to set the default list item.
        /// </summary>
        public void SetDefaultItem()
        {
            SelectedListItem = Items[0];
            if (SelectedListItem is not null)
            {
                OnSelectedListItemChanged(SelectedListItem);
            }
           
        }

        /// <summary>
        /// Method to navigate to the settings view.
        /// </summary>
        public void handleGoToSettgins()
        {        
            var settingsItem = Items.FirstOrDefault(item => item.ModelType == typeof(SettingsViewModel));
            if (settingsItem != null)
            {
                int index = Items.IndexOf(settingsItem);
                if (SelectedListItem != Items[index])
                {
                    SelectedListItem = Items[index];
                    OnSelectedListItemChanged(SelectedListItem);
                }
                else {
                    Console.WriteLine("juz jest");
                }
            }
        }
    }

    /// <summary>
    /// Class representing a template for a list item.
    /// </summary>
    public class ListItemTemplate
    {

        public ListItemTemplate(Type type, string iconKey, string name) 
        {
            ModelType = type;
            //Label = type.Name.Replace("ViewModel", "");
            Label = name;
            Application.Current!.TryFindResource(iconKey, out var res);
            ListItemIcon = (StreamGeometry)res!;

        }
        public string Label {get;}
        public Type ModelType {get;}
        public StreamGeometry ListItemIcon {get;}
    }

}