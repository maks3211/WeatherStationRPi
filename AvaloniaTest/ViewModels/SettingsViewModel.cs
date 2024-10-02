using Avalonia.Media;
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
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using AvaloniaTest.Messages;
using AvaloniaTest.Services.Factories;
namespace AvaloniaTest.ViewModels;


/// <summary>
/// View model class for the settings window.
/// </summary>
public partial class SettingsViewModel : ViewModelBase
{
    private ViewModelFactory vMf;
    private readonly Dictionary<Type, ViewModelBase> _viewModelCache = new();


    public static event EventHandler<string> CurrentSettingsOpen;
    public static string CurrentSettingsSub = "";

    [ObservableProperty]
    private ViewModelBase _currentsettingspage;// = new GeneralSettingsViewModel();
    
    [ObservableProperty]
    private SettingsListTemplate? _selectedSettings;





  

    /// <summary>
    /// Constructor for the SettingsViewModel class.
    /// </summary>
    public SettingsViewModel(ViewModelFactory factory)
    {
        vMf = factory;
        WeakReferenceMessenger.Default.Register<ViewActivatedMessages>(this, (r, m) =>
        {
            if (m.Value == GetType().FullName)
            {
                Console.WriteLine("Otwarto ustawienia");
                WeakReferenceMessenger.Default.Send(new SettingsViewActivatedMessages(CurrentSettingsSub));
            }
            else
            {
                Console.WriteLine("Zamknięto ustawienia");
                WeakReferenceMessenger.Default.Send(new SettingsViewActivatedMessages(""));
            }
        });

       if (Items.Any())
       {
           SelectedSettings ??= Items.First();
       }

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
            Console.WriteLine("-----------WSZYSKTIE USTAWIENIA---------------");
            CurrentSettingsSub = Currentsettingspage?.ToString() ?? "AvaloniaTest.ViewModels.GeneralSettingsViewModel";

           // Currentsettingspage ??= new GeneralSettingsViewModel();

            CurrentSettingsOpen?.Invoke(this, CurrentSettingsSub ?? "");

            
        }
        else
        {
            Console.WriteLine("------------ZAMKNIETO WSZYSTKIE USTAWIENIA---------------");
            CurrentSettingsSub = "AvaloniaTest.ViewModels";
             CurrentSettingsOpen?.Invoke(this, CurrentSettingsSub ?? "");
           // CurrentSettingsOpen?.Invoke(this, "");
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
        if (!_viewModelCache.TryGetValue(value.ModelType, out var instance))
        {
           // instance = (ViewModelBase)Activator.CreateInstance(value.ModelType)!;
            instance = vMf.CreateViewModel(value.ModelType);
            _viewModelCache[value.ModelType] = instance;
        }

        Console.WriteLine("ffghddfghdfgh");
       Currentsettingspage = (ViewModelBase)instance;
       CurrentSettingsSub = Currentsettingspage?.ToString() ?? "";
       WeakReferenceMessenger.Default.Send(new SettingsViewActivatedMessages(CurrentSettingsSub));
       
        // CurrentSettingsOpen?.Invoke(this, CurrentSettingsSub ?? "");



    }

    /// <summary>
    /// Collection of settings items.
    /// </summary>
    public ObservableCollection<SettingsListTemplate> Items { get; } = new()
    {
        new SettingsListTemplate(typeof(GeneralSettingsViewModel), "Ustawienia ogólne","SettingsRegular"),
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
