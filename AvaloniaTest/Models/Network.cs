using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace AvaloniaTest.Models
{
    public struct wifiStruct
    {
      public  string Ssid;
      public  int PowerLevel;

        public wifiStruct(string name, int power)
        {
            Ssid = name;
            PowerLevel = power; 
        }
    }


    public class Network
    {
        public event EventHandler WifiListUpdated;
        public static List<wifiStruct> wifiList = new List<wifiStruct>();
        //TYLKO DLA WINDOWSA
        public static void TestgetName()
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", "wlan show networks mode=Bssid");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            Process proc = Process.Start(psi);
            if (proc != null)
            {
                string result = proc.StandardOutput.ReadToEnd();
                string[] lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, @"SSID [0-9]+ : (.+)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string ssid = match.Groups[1].Value.Trim();
                    }
                }
                proc.WaitForExit();
            }
        }


        public async Task GetWifiList()
        {
          
            wifiList.Clear();
            Process process = new Process();
            process.StartInfo.FileName = "sudo";
            process.StartInfo.Arguments = "wpa_cli -i wlan0";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;


            DataReceivedEventHandler outputDataReceivedHandler = null;
            outputDataReceivedHandler = (sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    // Wyodrębnij nazwy SSID za pomocą wyrażenia regularnego
                    string ssid = ExtractSSID(e.Data);
                    string level = ExtractSignalLevel(e.Data);
                    if (!String.IsNullOrEmpty(ssid) && !String.IsNullOrEmpty(level))
                    {
                        //Console.WriteLine(ssid); // Wyświetl nazwę SSID
                       // Console.WriteLine(level); // Wyświetl nazwę SSID
                        int le = Int32.Parse(level);
                        wifiList.Add(new wifiStruct(ssid, le));
                        WifiListUpdated?.Invoke(this, EventArgs.Empty);
                        //wifiList.Add(ssid);
                    }
                }
            };


            process.OutputDataReceived += outputDataReceivedHandler;

            // Uruchom proces
            process.Start();
            process.BeginOutputReadLine();

            // Wykonaj polecenia
            ExecuteCommand(process, "scan");
            ExecuteCommand(process, "scan_results");
            
            Timer timer = null; // Deklaracja timera

            timer = new Timer((state) =>
            {
                process.OutputDataReceived -= outputDataReceivedHandler;
                timer.Dispose();
            }, null, 500, Timeout.Infinite);
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


        public void ConnectToWifi(string ssid, string password)
        {
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
            }
            Console.WriteLine($"WYNIK laczenia wifi: {resultMessage}");
        }


        public async Task<String> IsConnected()
        {
            bool isConnected = false;
            string ssid = null;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "sudo";
            startInfo.Arguments = "wpa_cli -i wlan0 status";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            string ssidPattern = @"ssid=(.+)";
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();

                if (output.Contains("ssid="))
                {
                    int start = output.IndexOf("ssid=", 5);
                    int stop = output.IndexOf("id=", start + 5);
                    ssid = output.Substring(start + 5, stop - start - 6);
                    //  ssid = Regex.Match(output, @"ssid=(.+)\r?\n").Groups[1].Value;

                    //ssid = output.Replace("ssid=", "");
                }
                // Sprawdź, czy wyjście zawiera informacje o stanie połączenia
                if (output.Contains("wpa_state=COMPLETED"))
                {
                    isConnected = true;
                }
                //ssid = result;
            }

            return ssid;
        }

         static void ExecuteCommand(Process process, string command)
         {
             process.StandardInput.WriteLine(command);
             process.StandardInput.Flush();
         }

       // private string ExecuteCommand(Process process, string command)
      //  {
       //     process.StandardInput.WriteLine(command); // Wyślij komendę
       ///     
       //     process.StandardInput.Flush();
       //     string output = process.StandardOutput.ReadToEnd(); // Odczytaj odpowiedź
       //     return output;
       // }



    }
   
}
