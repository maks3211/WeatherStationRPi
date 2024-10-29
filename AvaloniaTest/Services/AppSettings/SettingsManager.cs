using AvaloniaTest.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaTest.Services.AppSettings
{
    public class SettingsManager
    {
        private readonly string _settingsFilePath;




        public SettingsManager(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
        }

        public async Task SaveSettingsAsync<T>(string sectionName, T settings) where T : class
        {
            try
            {
                var json = File.Exists(_settingsFilePath) ? await File.ReadAllTextAsync(_settingsFilePath) : "{}";
                var settingsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];
                var updatedSettingsJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                settingsDict[sectionName] = JsonSerializer.Deserialize<JsonElement>(updatedSettingsJson);

                // Zapisz zmieniony JSON
                var updatedJson = JsonSerializer.Serialize(settingsDict, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_settingsFilePath, updatedJson);       //automatyczne zamykanie pliku 
            

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save Settings Async Failed: {ex.ToString()}");
            }
        }


        public async Task LoadSettingsAsync<T>(string sectionName, T settings) where T : class
        {    
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    return;
                }

                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var settingsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (settingsDict != null && settingsDict.TryGetValue(sectionName, out var section))
                {
                    // Deserializuj odpowiednią sekcję do typu T
                    var updatedSettings = JsonSerializer.Deserialize<T>(section.GetRawText());
                    if (updatedSettings != null)
                    {
                        // Załaduj właściwości do istniejącej instancji
                        var properties = typeof(T).GetProperties();
                        foreach (var property in properties)
                        {
                            var ignoreAttribute = property.GetCustomAttribute<IgnoreDuringSerializationAttribute>();
                            //var newValue = property.GetValue(updatedSettings);
                            //property.SetValue(settings, newValue);
                            if (ignoreAttribute == null || !ignoreAttribute.Ignore)
                            {
                                //ZMIENNE PRZYCHOWUJACE PREV TO ZMIENNE TYPU FIELD A NIE PROPERTIES 
                                var newValue = property.GetValue(updatedSettings);
                                property.SetValue(settings, newValue);

                                // Sprawdź, czy pole ma tę samą nazwę, ale z "prev" na początku
                                var prevProperty = properties.FirstOrDefault(p => p.Name == "prev" + property.Name);
                                if (prevProperty != null)
                                {
                                    prevProperty.SetValue(settings, newValue);
                                }
                            }
                        }
                    }
                }
                return;

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Load Settings Async Failed: {ex.ToString()}");
               
            }
        }


    }
}
