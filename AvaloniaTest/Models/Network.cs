using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvaloniaTest.Models
{
    public class Network
    {

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
                        Console.WriteLine(ssid);
                    }
                }
                proc.WaitForExit();
            }
        }
    }
   
}
