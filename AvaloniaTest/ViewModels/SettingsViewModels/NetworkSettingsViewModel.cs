using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AvaloniaTest.Models;
using Avalonia.Media;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Input;
using System.ComponentModel;



namespace AvaloniaTest.ViewModels;

public partial class NetworkSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentwifilistselected = new GeneralSettingsViewModel();
    [ObservableProperty]
    private WifiListTemplate? _selectedwifilist;

    [ObservableProperty]
    private string _selectedwifi = "";

    //[ObservableProperty]
    //private string _passwordbox = "";

    public event PropertyChangedEventHandler PropertyChanged;


    private string _passwordbox;
    public string Passwordbox
    {
        get => _passwordbox;
        set
        {
            if (_passwordbox != value)
            {
                _passwordbox = value;
                RaisePropertyChanged(nameof(Passwordbox));
                // Wywołaj swoją metodę tutaj za każdym razem, gdy zmienia się wartość _passwordbox
                if (value == "")
                {
                    Connectbuttonvis = false;
                }
                else
                {
                    Connectbuttonvis = true;    
                }

            }
        }
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    [ObservableProperty]
    private string _connectedssid="";
    [ObservableProperty]
    private string _toConnectssid = "";


    [ObservableProperty]
    private bool _connectbuttonvis = false;
    [ObservableProperty]
    private bool _passwordboxvis = false;



    private static Network network = new Network();
    private bool isOpen;
    public NetworkSettingsViewModel() {
        isOpen = true;
        SettingsViewModel.CurrentSettingsOpen += ViewModel_Activated;
        Console.WriteLine("Otwarto ustawienia sieci");
        StartWifiReading();
        CheckWifiStatus();
        network.WifiListUpdated += (sender, e) =>
        {
            // Aktualizacja listy Items
            UpdateWifiList();
        };
       
        //rozpocznij skanowanie sieci i dodaj do listy 
    }

    private void ViewModel_Activated(object sender, string e)
    {
        Console.WriteLine($"To{e}");
        if (SettingsViewModel.CurrentSettingsSub == "AvaloniaTest.ViewModels.NetworkSettingsViewModel")
        {
            Console.WriteLine("------------otwarto zmiane siecii---------------");
            isOpen = true;
        }
        else
        {
            Console.WriteLine("-------------------zamknieto siec----------------------");
            isOpen = false;
            SettingsViewModel.CurrentSettingsOpen -= ViewModel_Activated;
        }

    }

    private void UpdateWifiList()
    {
        Items.Clear();
        foreach (var e in Network.wifiList)
        {
            Items.Add(new WifiListTemplate(e.Ssid, e.PowerLevel,false,false));
        }
       
        
       
    }

    public ObservableCollection<WifiListTemplate> Items {get; } = new()
    {
        
        new WifiListTemplate( "siec1",2,false,false),
        new WifiListTemplate( "siec2",4,false,true),

    };

    [RelayCommand]
    public void DeleteButton()
    {
        Selectedwifilist = null;
        Items.Add(new WifiListTemplate("siec33", 4,true,true));
        Items[0].Label = "zmiana";
        
    }

    [RelayCommand]
    public void ConnectButton()
    {
        
        Console.WriteLine($"Polacz z: {Selectedwifi}");
        
        network.ConnectToWifi(Selectedwifi, Passwordbox);
        Passwordbox = "";
        Connectedssid = "";
        ToConnectssid = "Łączenie z siecią";
        //Task t1 = CheckWifiStatus();

        //Task.WhenAll(t1);
        //Console.WriteLine(Passwordbox);

    }



    public async Task CheckWifiStatus()
    {    
        while (isOpen)
        {
            try
            {
                string net = await network.IsConnected();
                if (net == "")
                {
                    Connectedssid = "Brak polaczenia z siecia";
                   // ToConnectssid = "Wybierz sieć z listy";
                }
                else if (net == Selectedwifi)
                {
                    Connectedssid = "Polaczono z: " + net;
                    ToConnectssid = "";
                }
                else
                {
                    Connectedssid = "Polaczono z: " + net;
                   // Selectedwifi = "";
                   // Connectedssid = "Wprowadź hasło dla: " + Selectedwifi;

                }

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nTasks cancelled: timed out.\n");
            }

            //await network.GetWifiList();
            await Task.Delay(1000);

        }

    }


    partial void OnSelectedwifilistChanged(WifiListTemplate? value)
    {
        if (value is null)
        {
            //Passwordboxvis = false;
            return;
        }
        Console.WriteLine(value.Label);
        Selectedwifi = value.Label;
        Passwordboxvis = true;
        ToConnectssid = "Połącz się z:" +  value.Label;

    }


    public async Task StartWifiReading()
    {
        while (isOpen)
        {
            //w network zrobic token na 500ms i nich sama siebie zabija a tu wywoływanie tej metody co np 3sek 
            //network trwa 500 ms wywoływana w petli co 3 sek
            Console.WriteLine("Petla lecii");
            try
            {
                await network.GetWifiList();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nTasks cancelled: timed out.\n");
            }
            
            //await network.GetWifiList();
            await Task.Delay(3000);
           // await Task.WhenAll(t1);
        }
      
    }
}






public class WifiListTemplate
{

    public WifiListTemplate(string label, int power, bool isConnected, bool needPassword)
    {
        Label = label;
        Power = power;
        IsConnected = isConnected;
        NeedPassword = needPassword;

    }

    public string Label { set; get; }
    public int Power { get; }
    public bool IsConnected { get; set; }
    public bool NeedPassword { get; }


}




