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



namespace AvaloniaTest.ViewModels;

public partial class NetworkSettingsViewModel : ViewModelBase
{
    //Do czego ta zmienna - chyba nie potrzebne to jest 
    // [ObservableProperty]
    // private ViewModelBase _currentwifilistselected = new GeneralSettingsViewModel();


    [ObservableProperty]
    private WifiListTemplate? _selectedwifilist;

    [ObservableProperty]
    private WifiListTemplate? _espWifiList;

    private bool connectingToEsp = false;

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
    private string _connectedssid = "";
    [ObservableProperty]
    private string _toConnectssid = "";


    [ObservableProperty]
    private bool _connectbuttonvis = false;
    [ObservableProperty]
    private bool _passwordboxvis = false;
    [ObservableProperty]
    private bool _keyboardvis = false;



    private static Network network = new Network();
    private bool isOpen;
    public NetworkSettingsViewModel() {
        Console.WriteLine("-=-=-=-=-network SETTING KONSTRUKTOR-=-=-=-=-");
        WeakReferenceMessenger.Default.Register<SettingsViewActivatedMessages>(this, (r, m) =>
        {

            if (m.Value == GetType().FullName)
            {
                Console.WriteLine("Otwarto ustawienia- siec");
                ViewModelOpened();
            }
            else
            {
                ViewModelClosed();
                Console.WriteLine("Zamknięto ustawienia- siec");
            }
        });
        //   isOpen = true;
        // SettingsViewModel.CurrentSettingsOpen += ViewModel_Activated;
        //  StartWifiReading();
        //  CheckWifiStatus();

        network.WifiListUpdated += (sender, e) =>
        {
            // Aktualizacja listy Items
            UpdateWifiList();
        };



        //rozpocznij skanowanie sieci i dodaj do listy 



    }

    private void ViewModelClosed()
    {
        isOpen = false;
    }
    private void ViewModelOpened()
    {
        isOpen = true;
        StartWifiReading();
        CheckWifiStatus();
    }

    private void UpdateWifiList()
    {
        Items.Clear();
        foreach (var e in Network.wifiList)
        {
            Items.Add(new WifiListTemplate(e.Ssid, e.PowerLevel, false, false));
        }
    }

    public ObservableCollection<WifiListTemplate> Items { get; } = new()
    {

        new WifiListTemplate( "siec1",2,false,false),
        new WifiListTemplate( "siec2",4,false,true),

    };

    public ObservableCollection<WifiListTemplate> EspwifiList { get; } = new()
    {
    };

    [RelayCommand]
    public async Task GetEspwifi()
    {
        ReadESPwifiAsync();
    }


    private async Task ReadESPwifiAsync()
        {
        string esp32Url = "http://192.168.4.1/getData";
        using (HttpClient client = new HttpClient())
        {
            Console.WriteLine("Czytanie siecii");
            var response = await client.GetStringAsync($"{esp32Url}");

            var jsonResponseArray = JArray.Parse(response);
            string name;
            int power;
            Items.Clear();
            foreach (var item in jsonResponseArray)
            {
                Console.WriteLine($"Nr {item["nr"]}");
                Console.WriteLine($"SSID: {item["ssid"]}");
                Console.WriteLine($"RRSI: {item["rssi"]}%");
                Console.WriteLine($"CHANEL: {item["channel"]}");
                Console.WriteLine("------");
                name = (string)item["ssid"];
                power = (int)item["rssi"];
                Items.Add(new WifiListTemplate(name, power, false, false));
            }
        }
        connectingToEsp = true;
    }

    private async Task ConnectESPtoWifiAsync()
    {
        string esp32Url = "http://192.168.4.1/postData";  // Adres URL do ESP32
        // Przygotowanie danych JSON

        string h = Selectedwifi;
        string b = Passwordbox;
        // string jsonData = "{\"wifi_ssid\":\"TP-Link_0E21\",\"wifi_pass\":\"66275022\"}";
        string jsonData = $"{{\"wifi_ssid\":\"{Selectedwifi}\",\"wifi_pass\":\"{Passwordbox}\"}}";
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Tworzymy zawartość zapytania POST (JSON)
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Wysłanie zapytania POST
                HttpResponseMessage response = await client.PostAsync(esp32Url, content);

                // Sprawdzenie, czy odpowiedź jest poprawna
                response.EnsureSuccessStatusCode();

                // Odczytanie odpowiedzi jako tekst
                string responseData = await response.Content.ReadAsStringAsync();

                // Wyświetlenie odpowiedzi w konsoli
                Console.WriteLine("Odpowiedź z ESP32: " + responseData);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nBłąd: " + e.Message);
            }

        }

    }

    [RelayCommand]
    public void ConnectButton()
    {
        
        Console.WriteLine($"Polacz z: {Selectedwifi}");
        if (connectingToEsp)
        {
            ConnectESPtoWifiAsync();
        }
        else
        {
            network.ConnectToWifi(Selectedwifi, Passwordbox);
            Passwordbox = "";
            Connectedssid = "";
            ToConnectssid = "Łączenie z siecią";
        }
       


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
                //Tutaj sprawdzenie czy poloczona siec to esp jezeli tak to wywolaj ReadESPwifiAsync
             
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
        Keyboardvis = true; 
        ToConnectssid = "Połącz się z:" +  value.Label;
        Console.WriteLine("wybrno sieć:");
        Console.WriteLine(Selectedwifi);
    }


    public async Task StartWifiReading()
    {
        while (isOpen)
        {
            //TODO 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
    public int Power { get; set; }
    public bool IsConnected { get; set; }
    public bool NeedPassword { get; }


}




