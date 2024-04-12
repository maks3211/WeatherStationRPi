﻿using Avalonia;
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
    public partial class MainWindowViewModel : ViewModelBase
    {
        //Np w Text umieszczam Text="{Binding Tekst}" i sie zmienia po zmianie Tekst = "abc"
        //wywoływanie metod np po guziku to Command="{Binding btnClickCommand}" + defincja btnClick

        //OTWIERANIE SPLIT VIEW
        //  [ObservableProperty]
        //  private bool _isMainPaneOpen = true;

        public static event EventHandler<string> CurrentPageOpened;
        public static string CurrentPageSub = "";
        public  static OutDoorSensor outDoorSens = new OutDoorSensor();
        public MainWindowViewModel()
        {
              Console.WriteLine("-------------NOwy MainWindowViewModel--------------");
            CurrentPageSub = "AvaloniaTest.ViewModels.HomePageViewModel";
            CurrentPageOpened?.Invoke(this, CurrentPageSub ?? "");
            // Ustaw pierwszy element jako domyślnie wybrany
            //    SelectedListItem = Items.FirstOrDefault();
            //outDoorSens.StartMake();

            StartDataReading();
           // outDoorSens.DataUpdatedTwo += Poka;
           // outDoorSens.DataUpdated += Pokadwa;
        }

     //   private void Poka(object sender, double e)
      //  {
      //      Console.WriteLine("JJJJJJ:" + e);
      //  }
      // private void Pokadwa(object sender, double e)
       // {
       //     Console.WriteLine("IIIIIIII:" + e);
       // }

        public async Task StartDataReading()
        {
            
            Task task1 = outDoorSens.RunReadData();
            //Task task2 = outDoorSens.RunReadDataTwo();
            await Task.WhenAll(task1);
        }
        [ObservableProperty]
        public ViewModelBase _currentPage = new HomePageViewModel();
        [ObservableProperty]
        private ListItemTemplate? _selectedListItem;

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value == null) return;
            var istance = Activator.CreateInstance(value.ModelType);
            if (istance == null) return;
            CurrentPage = (ViewModelBase)istance;
            CurrentPageSub = CurrentPage.ToString();
            Console.WriteLine("TUTAJ JEST TA STREONA:" + CurrentPageSub);
            CurrentPageOpened?.Invoke(this, CurrentPageSub ?? "");
        }
        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        { 
            new ListItemTemplate(typeof(HomePageViewModel),"HomeRegular", "Strona Główna"),
            new ListItemTemplate(typeof(SettingsViewModel),"SettingsRegular","Ustawienia"),
        };


        [RelayCommand]
        public void ChangeTheme()
        {
            App app = (App)Application.Current;
            app.ChangeTheme();
       
        }

        public void setDefaultItem()
        {
            SelectedListItem = Items[0];
            if (SelectedListItem is not null)
            {
                OnSelectedListItemChanged(SelectedListItem);
            }
           
        }



        //[RelayCommand]
        //private void BtnClick()
        //{
        //
        //    if (IsMainPaneOpen)
        //    {
        //    IsMainPaneOpen = false;
        //        //CurrentPage = new SettingsViewModel();
        //    }
        //    else
        //    {
        //        IsMainPaneOpen = true;
        //    }
        //
        //
        //}

        //!!! Narazie przechodzi ogólnie do ustawień Zrobić żeby przechodziło od razu do ustawień motywu :) 
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