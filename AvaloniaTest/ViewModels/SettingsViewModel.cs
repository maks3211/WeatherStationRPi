﻿using Avalonia.Media;
using AvaloniaTest.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
namespace AvaloniaTest.ViewModels;


/// <summary>
/// View model class for the settings window.
/// </summary>
public partial class SettingsViewModel : ViewModelBase
{

    public static event EventHandler<string> CurrentSettingsOpen;
    public static string CurrentSettingsSub = "";

    [ObservableProperty]
    private ViewModelBase _currentsettingspage = new GeneralSettingsViewModel();
    
    [ObservableProperty]
    private SettingsListTemplate? _selectedSettings;

    /// <summary>
    /// Constructor for the SettingsViewModel class.
    /// </summary>
    public SettingsViewModel()
    {
        MainWindowViewModel.CurrentPageOpened += ViewModel_Activated;
        CurrentSettingsSub = "AvaloniaTest.ViewModels.GeneralSettingsViewModel";
        CurrentSettingsOpen?.Invoke(this, CurrentSettingsSub ?? "");

    }
    /// <summary>
    /// Method called when the main view model is activated.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewModel_Activated(object sender, string e)
    {
        if (MainWindowViewModel.CurrentPageSub == "AvaloniaTest.ViewModels.SettingsViewModel")
        {
           // Console.WriteLine("------------otwarto Settings---------------");
        }
        else
        {
            CurrentSettingsSub = "AvaloniaTest.ViewModels";
            CurrentSettingsOpen?.Invoke(this, CurrentSettingsSub ?? "");
            MainWindowViewModel.CurrentPageOpened -= ViewModel_Activated;
        }

    }


    /// <summary>
    /// Method called when the selected settings item is changed.
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedSettingsChanged(SettingsListTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if(instance is null) return ;
        Currentsettingspage = (ViewModelBase)instance;
        CurrentSettingsSub = Currentsettingspage.ToString();
        Console.WriteLine($"JAKA JEST STORNS: {CurrentSettingsSub}");
        CurrentSettingsOpen?.Invoke(this, CurrentSettingsSub ?? "");
    }

    /// <summary>
    /// Collection of settings items.
    /// </summary>
    public ObservableCollection<SettingsListTemplate> Items { get; } = new()
    {
        new SettingsListTemplate(typeof(GeneralSettingsViewModel), "Ustawienia ogolne","SettingsRegular"),
        new SettingsListTemplate(typeof(NetworkSettingsViewModel), "Ustawienia sieci", "NetworkSettingsEmpty"),
    };


}

/// <summary>
/// Class representing a template for a settings list item.
/// </summary>
public class SettingsListTemplate {
    
    public SettingsListTemplate(Type type, string label, string icon) {
    ModelType = type;
    Label = label;
        Application.Current!.TryFindResource(icon, out var res);
        ItemIcon = (StreamGeometry)res!;
    }

    public string Label { get;}
    public Type ModelType { get;}
    public StreamGeometry ItemIcon { get;}

}
