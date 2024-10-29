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
using AvaloniaTest.Views;
using AvaloniaTest.Helpers;
using AvaloniaTest.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using AvaloniaTest.Services;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Google.Protobuf.WellKnownTypes;



namespace AvaloniaTest.ViewModels;

public partial class NetworkSettingsViewModel : ViewModelBase
{
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

    [ObservableProperty]
    private string _selectedwifi = "";
    [ObservableProperty]
    private string _toConnectssid = "";
    [ObservableProperty]
    private bool _connectbuttonvis = false;
    [ObservableProperty]
    private bool _passwordboxvis = false;
    [ObservableProperty]
    private bool _keyboardvis = false;
    [ObservableProperty]
    private NetworkManager _netMan;
    private bool isOpen;


    public NetworkSettingsViewModel(NetworkManager networkManager)
    {

        NetMan = networkManager;
        NetMan.PropertyChanged += NetMan_PropertyChanged;
        WeakReferenceMessenger.Default.Register<SettingsViewActivatedMessages>(this, (r, m) =>
        {

            if (m.Value == GetType().FullName)
            {
                ViewModelOpened();
            }
            else
            {
                ViewModelClosed();
            }
        });
    }
    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void NetMan_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Console.WriteLine("jakas zmina w sieci ");
        Console.WriteLine(e.PropertyName);
        if (e.PropertyName == nameof(NetMan.SelectedWifiFromList) && NetMan.SelectedWifiFromList is not null)
        {
            Console.WriteLine(NetMan.SelectedWifiFromList.Label);
            Selectedwifi = NetMan.SelectedWifiFromList.Label;
            Passwordboxvis = true;
            Keyboardvis = true;
            ToConnectssid = "Połącz się z:" + NetMan.SelectedWifiFromList.Label;
        }
        
    }

  
    public async void GetWifiList()
    {
        Console.WriteLine("Cykliczna metoda odczytywanie listy siecii w ustawieniach");
        await NetMan.GetNetworkList();
    }
    private void ViewModelClosed()
    {
        isOpen = false;
        Clock.Instance.RemoveTask("GetWifiList");
        NetMan.MessageActivator = false;

    }
    private void ViewModelOpened()
    {
        isOpen = true;
        Clock.Instance.AddTask("GetWifiList", GetWifiList, TimeSpan.FromSeconds(30));
        GetWifiList();
    }
    [RelayCommand]
    public async void ConnectButton()
    {
        await NetMan.ConnectToWifi(Selectedwifi, Passwordbox);
    }
}



