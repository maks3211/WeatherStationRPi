using Avalonia.Media;
using AvaloniaTest.Models;
using AvaloniaTest.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaTest.Services
{
    public partial class NetworkManager: ObservableObject, INetworkStatus
    {

        public bool isConnected { get; private set; }

        public event EventHandler NetworkConnected;
        public event EventHandler NetworkDisconnected;


        //zmienia sie ssid - jezeli jest pusty lub esp to nie ma dostepu do sieci 
        partial void OnCurrentSsidChanged(string value)
        {
            Console.WriteLine("Zmiana stanu sieci");
            if (!value.Contains("ESP") && value != "")
            {
                Console.WriteLine("JEST INTERNET");
                isConnected = true;
                NetworkConnected?.Invoke(this, EventArgs.Empty);
            }
            else {
                Console.WriteLine("NIE MA INTERENTU");
                isConnected = false;
                NetworkDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }


        [ObservableProperty]
        public string _currentSsid = "-";   
        [ObservableProperty]
        public string _currentSsidDisplay;

        [ObservableProperty]
        public double _signalStrength; //pozniej na zdj

        [ObservableProperty]
        public StreamGeometry _strenghIcon;

        [ObservableProperty]
        public string _message;
        [ObservableProperty]
        public bool _messageActivator = false;


        [ObservableProperty]
        public string _currentNetwork;

        [ObservableProperty]
        public string _lastSsid;
        private string LastPassword;


        private bool ESPconnecting = false;

        public ObservableCollection<WifiListTemplate> WifiList { get; set; } = new();

        [ObservableProperty]
        private WifiListTemplate? _selectedWifiFromList;




 

        partial void OnSelectedWifiFromListChanged(WifiListTemplate? value)
        {
            MessageActivator = false;
            Console.WriteLine("Wybrano siec: ");
            Console.WriteLine(value?.Label);
            if (value is not null)
            {
                if (ESPconnecting)
                {
                    Message = $"Wybrana sieć dla ESP:";
                }
                else {
                    Message = $"Wybrana sieć dla RPI:";
                }
            
            }
            MessageActivator = true;
        }


    

        private void DetermineMessage()
        {
            
        }


        /// <summary>
        /// Returns saved wifi password stored on device
        /// </summary>
        public async Task<string> GetWifiPassword(string ssid)
        {
            if (string.IsNullOrEmpty(ssid))
                return null;

            string wpaFilePath = "/etc/wpa_supplicant/wpa_supplicant.conf";
            string password = null;

            // Sprawdź, czy plik istnieje
            if (File.Exists(wpaFilePath))
            {
                string[] lines = await File.ReadAllLinesAsync(wpaFilePath);
                bool inNetworkBlock = false;

                foreach (string line in lines)
                {
                    // Użycie wyrażenia regularnego do dopasowania dokładnego SSID
                    if (Regex.IsMatch(line, $@"ssid=""{Regex.Escape(ssid)}"""))
                    {
                        inNetworkBlock = true; // Zaczynamy blok sieci
                    }
                    else if (inNetworkBlock && line.Trim().StartsWith("psk="))
                    {
                        password = line.Substring(line.IndexOf('=') + 1).Trim('\"');
                        break; // Zakończono szukanie hasła
                    }
                    else if (inNetworkBlock && line.Trim().StartsWith("}"))
                    {
                        inNetworkBlock = false; // Zakończono blok sieci
                    }
                }
            }

            return password;
        }
        /// <summary>
        /// Returns current RPI ssid
        /// </summary>
        public string GetCurrentSsidRPI()
        {
            // Tworzymy nowy proces
            Process process = new Process();
            process.StartInfo.FileName = "bash"; // Uruchamiamy powłokę bash
            process.StartInfo.Arguments = "-c \"iwgetid -r\""; // Argumenty do wykonania
            process.StartInfo.UseShellExecute = false; // Nie używamy powłoki
            process.StartInfo.RedirectStandardOutput = true; // Przekierowujemy standardowe wyjście
            process.StartInfo.RedirectStandardError = true; // Przekierowujemy standardowy błąd
            process.StartInfo.CreateNoWindow = true; // Nie tworzymy nowego okna

            try
            {
                // Uruchamiamy proces
                process.Start();
                // Odczytujemy wyjście
                string output = process.StandardOutput.ReadToEnd();
                // Czekamy na zakończenie procesu
                process.WaitForExit();

                // Zwracamy wynik, usuwając ewentualne białe znaki
                Console.WriteLine($"Aktualna sieć wifi: {output}    Metoda GetSsidRPI");
              
                
                return output.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                return string.Empty; // Zwracamy pusty string w przypadku błędu
            }
        }


        public async Task ConnectToWifi(string ssid, string password)
        {
            if (ESPconnecting)
            {
                await ConnectESPtoWifiAsync(ssid,password);
            }
            else
            {
                await ConnectRPI(ssid, password);
            }
        }

        public async Task<(string ssid, string signalStrength)> GetCurrentNetworkInfo()
        {
            string ssid = string.Empty;
            string signalStrength = string.Empty;
            try
            {
           

            // Odczyt SSID
            using (Process ssidProcess = new Process())
            {
                ssidProcess.StartInfo.FileName = "bash"; // Użyj bash do wykonania polecenia
                ssidProcess.StartInfo.Arguments = "-c \"iwgetid -r\""; // Odczytaj SSID
                ssidProcess.StartInfo.UseShellExecute = false;
                ssidProcess.StartInfo.RedirectStandardOutput = true;
                ssidProcess.StartInfo.RedirectStandardError = true;
                ssidProcess.StartInfo.CreateNoWindow = true;

                ssidProcess.Start();

                ssid = await ssidProcess.StandardOutput.ReadToEndAsync();
                string error = await ssidProcess.StandardError.ReadToEndAsync();

                ssidProcess.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Błąd odczytu SSID: {error}");
                    return (null, null);
                }
                    CurrentSsid = ssid.Trim();
                }

            // Odczyt siły sygnału
            using (Process signalProcess = new Process())
            {
                signalProcess.StartInfo.FileName = "bash"; // Użyj bash do wykonania polecenia
                signalProcess.StartInfo.Arguments = "-c \"iw dev wlan0 link | grep 'signal'\""; // Odczytaj siłę sygnału
                signalProcess.StartInfo.UseShellExecute = false;
                signalProcess.StartInfo.RedirectStandardOutput = true;
                signalProcess.StartInfo.RedirectStandardError = true;
                signalProcess.StartInfo.CreateNoWindow = true;

                signalProcess.Start();

                signalStrength = await signalProcess.StandardOutput.ReadToEndAsync();
                string signalError = await signalProcess.StandardError.ReadToEndAsync();

                signalProcess.WaitForExit();

                if (!string.IsNullOrEmpty(signalError))
                {
                    Console.WriteLine($"Błąd odczytu siły sygnału: {signalError}");
                    return (ssid, null);
                }
            }

            // Przykład wyniku dla siły sygnału: "signal: -50 dBm"
            signalStrength = signalStrength.Trim(); // Zwróć wynik siły sygnału
           
            }
            catch (Exception ex){
                Console.WriteLine($"GetCurrentNetworkInfo error: {ex.Message}");
            }
            string[] parts = signalStrength.Split(' ');

            // Przekonwertuj liczbę na typ double
            if (parts.Length >= 2 && double.TryParse(parts[1], out double signalValue))
            {
                Console.WriteLine($"Wartość sygnału: {signalValue}");
                SignalStrength = signalValue;
            }
            else
            {
                Console.WriteLine("Nie udało się wyodrębnić wartości sygnału.");
                SignalStrength = 0;
            }


           
            Console.WriteLine("Odczytano nazwe siecii");



            if (CurrentSsid.Contains("ESP"))
            {
                MessageActivator = false;
                Console.WriteLine("Polaczono z esp");
                Message = "Połączono się z esp. Wybierz sieć dla czujników bezprzewodowych";
                ESPconnecting = true;
            }
            else { 
            ESPconnecting = false;
            }
            MessageActivator = true;
            return (ssid.Trim(), signalStrength);
        }



        public async Task GetNetworkList()
        {
            if (ESPconnecting)
            {
                 await GetListESPAsync();
            }
            else {
                await GetListRPI();
            }
        }
        public async Task GetListRPI()
        {
            try
            {
                WifiList.Clear();
                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "sudo";
                    process.StartInfo.Arguments = "wpa_cli -i wlan0";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    // Zmienna do przechowywania wyników
                    List<string> results = new List<string>();

                    DataReceivedEventHandler outputDataReceivedHandler = (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            string ssid = ExtractSSID(e.Data);
                            string level = ExtractSignalLevel(e.Data);
                            bool requiresPassword = ExtractRequiresPassword(e.Data); // Nowa funkcja

                            if (!string.IsNullOrEmpty(ssid) && int.TryParse(level, out int signalStrength)) // Próbujemy sparsować siłę sygnału
                            {                           
                                results.Add($"Nazwa sieci: {ssid}, sygnał: {signalStrength}, wymaga hasła: {requiresPassword}");
                                WifiList.Add(new WifiListTemplate(ssid, signalStrength,false, requiresPassword)); // Dodajemy do listy
                            }
                        }
                    };

                    process.OutputDataReceived += outputDataReceivedHandler;

                    // Uruchom proces
                    process.Start();
                    process.BeginOutputReadLine();

                    // Wykonaj polecenia asynchronicznie
                    await Task.Run(() =>
                    {
                        ExecuteCommand(process, "scan");
                        ExecuteCommand(process, "scan_results");
                    });

                    // limit czasu 
                    cts.CancelAfter(5000);

                    // Czekaj na zakończenie procesu, ale sprawdzaj także, czy anulowano
                    while (!process.HasExited)
                    {
                        if (cts.Token.IsCancellationRequested)
                        {
                            Console.WriteLine("Operacja została anulowana.");

                            // Wysyłamy polecenie quit do procesu
                            using (var writer = process.StandardInput)
                            {
                                if (writer.BaseStream.CanWrite)
                                {
                                    writer.WriteLine("quit");
                                }
                            }
                            break;
                        }
                        await Task.Delay(100); // Małe opóźnienie
                    }

                    // Czekaj na zakończenie procesu w sposób asynchroniczny
                    await process.WaitForExitAsync();

                    // Usuń nasłuchiwacza
                    process.OutputDataReceived -= outputDataReceivedHandler;

                    // Wyświetl wyniki po zakończeniu procesu
                    if (results.Count == 0)
                    {
                        Console.WriteLine("Brak dostępnych sieci.");
                    }

                    Console.WriteLine("xxxxxaaaaaKONIEC czytania dostępnych sieci");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetWifiListRPI error: {ex.Message}");
            }
           
        }

        private async Task GetListESPAsync()
        {
            Console.WriteLine("Czytanie danych z esp");
            Message = "Pobieranie danych o dostępnych sieciach z ESP";
            MessageActivator = true;
            WifiList.Clear();
            string esp32Url = "http://192.168.4.1/getData";
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Czytanie siecii");
                var response = await client.GetStringAsync($"{esp32Url}");

                var jsonResponseArray = JArray.Parse(response);
                string name;
                int power;

                foreach (var item in jsonResponseArray)
                {
                    Console.WriteLine($"Nr {item["nr"]}");
                    Console.WriteLine($"SSID: {item["ssid"]}");
                    Console.WriteLine($"RRSI: {item["rssi"]}%");
                    Console.WriteLine($"CHANEL: {item["channel"]}");
                    Console.WriteLine("------");
                    name = (string)item["ssid"];
                    power = (int)item["rssi"]; 
                    WifiList.Add(new WifiListTemplate(name, power, false, true));
                }
               
            }
            MessageActivator = false;
            Message = "Wczytano listę sieci dla ESP";
            MessageActivator = true;
        }


        /// <summary>
        /// Conntects RPI to wifi
        /// </summary>
        public async Task ConnectRPI(string ssid, string password = "")
        {

            if (ssid.Contains("ESP")) //jezeli łączenie do esp to zapisz aktualna siec
            {
                LastSsid = CurrentSsid;
                LastPassword = await GetWifiPassword(CurrentSsid);
                Console.WriteLine($"Stara sieć: {LastSsid} stare hasło: {LastPassword}");
            }
           


            Console.WriteLine("NOWE LACZENIE DO SIECII");
            string resultMessage = "";

            using (Process startInfo = new Process())
            {
                startInfo.StartInfo.FileName = "sudo";
                startInfo.StartInfo.Arguments = "wpa_cli -i wlan0";
                startInfo.StartInfo.RedirectStandardInput = true;
                startInfo.StartInfo.RedirectStandardOutput = true;
                startInfo.StartInfo.UseShellExecute = false;
                startInfo.Start();

                ExecuteCommand(startInfo, "set_network 0 ssid \"" + ssid + "\"");
                ExecuteCommand(startInfo, "set_network 0 psk \"" + password + "\"");
                ExecuteCommand(startInfo, "save_config");
                ExecuteCommand(startInfo, "disconnect");
                ExecuteCommand(startInfo, "select_network 0");
                ExecuteCommand(startInfo, "save_config");

                // Zamknij strumienie wejścia i wyjścia
                startInfo.StandardInput.Close();
                startInfo.StandardOutput.Close();


                // Zakończ proces

                startInfo.WaitForExit();
                await Task.Delay(400);
            }
            //Po polaczeniu sprawdz stan siecii 
            Console.WriteLine("polaczono z siecia i sprawdzanie jej nazwy");
            await GetCurrentNetworkInfo();

        }

        public async Task ConnectESPtoWifiAsync(string ssid, string password = "")
        {
            string esp32Url = "http://192.168.4.1/postData";  // Adres URL do ESP32
                                                              // Przygotowanie danych JSON

            // string jsonData = "{\"wifi_ssid\":\"TP-Link_0E21\",\"wifi_pass\":\"66275022\"}";
            string jsonData = $"{{\"wifi_ssid\":\"{ssid}\",\"wifi_pass\":\"{password}\"}}";
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
                    if (responseData == "ESP get WiFi data")
                    {
                        MessageActivator = false;
                        Message = "Przesłano dane do esp - nastąpi automatyczne wznowienie połączenia RPI z poprzednią siecią";
                        MessageActivator = true;
                    }
                    else
                    {
                        MessageActivator = false;
                        Message = "Błąd przesyłania danych do ESP- spróbuj ponownie";
                        MessageActivator = true;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nBłąd: " + e.Message);
                }
                await Task.Delay(3500); //po przesłaiu danych poczekaj 
                await ConnectRPI(LastSsid, LastPassword);

            }
        }


        static void ExecuteCommand(Process process, string command)
        {
            process.StandardInput.WriteLine(command);
            process.StandardInput.Flush();
        }


        static string ExtractSSID(string line)
        {
            Regex regex = new Regex(@"\]\s+(.+)$");
            Match match = regex.Match(line);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        static string ExtractSignalLevel(string line)
        {
            Regex regex = new Regex(@"-(\d+)");
            Match match = regex.Match(line);
            if (match.Success)
            {
                // Grupa w nawiasach () to liczba przy znaku "-"
                string power = match.Groups[1].Value;
                return power;
            }
            else
            {
                // Console.WriteLine("Nie znaleziono dopasowania.");
                return null;
            }
        }

        private bool ExtractRequiresPassword(string data)
        {
            return data.Contains("secured");
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

}


