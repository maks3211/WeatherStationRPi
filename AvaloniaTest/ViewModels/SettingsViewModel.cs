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
namespace AvaloniaTest.ViewModels;

public partial class SettingsViewModel : ViewModelBase
    {

    [ObservableProperty]
    private ViewModelBase _currentsettingspage = new GeneralSettingsViewModel();
    [ObservableProperty]
    private SettingsListTemplate? _selectedSettings;

    public SettingsViewModel()
    {
        Console.WriteLine("TYPE OF: ");
        Console.WriteLine(typeof(GeneralSettingsViewModel));
        Console.WriteLine("- ");
    }
    partial void OnSelectedSettingsChanged(SettingsListTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if(instance is null) return ;
        Currentsettingspage = (ViewModelBase)instance;
    }

    public ObservableCollection<SettingsListTemplate> Items { get; } = new()
    {
       
        new SettingsListTemplate(typeof(GeneralSettingsViewModel), "Ustawienia ogolne","SettingsRegular"),
        new SettingsListTemplate(typeof(NetworkSettingsViewModel), "Ustawienia sieci", "NetworkSettingsEmpty"),
        
    };


    }


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
