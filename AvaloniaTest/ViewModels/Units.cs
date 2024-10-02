using AvaloniaTest.Messages;
using AvaloniaTest.Services.AppSettings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.ViewModels;

/// <summary>
/// Class for managing units used in weather data.
/// </summary>
public partial class Units: ObservableObject
    {
    public static string FormatTemperature(double temp) => $"{temp} C";

    [ObservableProperty]
    private UnitsSettings _ustaw;


    public void SetSettings(UnitsSettings s)
    {
        Ustaw = s;
    }



    private string  currentTempUnit = "°C";
    private string currentWindUnit = "m/s";
    private static Units _instance;


    private Units()
    {
        Console.WriteLine("robie to ");
       // Ustaw.PropertyChanged += Ustaw_PropertyChanged;
    }

    public void SetSub()
    {
        Ustaw.PropertyChanged += Ustaw_PropertyChanged;
    }
    private void Ustaw_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Ustaw.Wind))
        {
            // Tutaj dodaj kod, który ma reagować na zmianę wartości
            Console.WriteLine($"Nowa wartość wiatru: {Ustaw.Wind}");

        }
    }

    


    /// <summary>
    /// Method to get the singleton instance of the Units class.
    /// </summary>
    /// <returns>Metod returns an instance of singleton object</returns>
    public static Units GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Units();
        }
        return _instance;
    }

    /// <summary>
    /// Method to get the current temperature unit.
    /// </summary>
    /// <returns>Method returns current temperature unit</returns>
    public string GetTempUnit()
    {
        return currentTempUnit;
    }

    /// <summary>
    /// Method to get the current wind unit.
    /// </summary>
    /// <returns>Method returns current wind unit</returns>
    public string GetWindUnit()
    {
        return currentWindUnit;
    }

    /// <summary>
    /// Method to calculate temperature based on the current unit.
    /// </summary>
    /// <returns>Method returns calculated temperature acording to current unit.</returns>
    public double CalculateTemp(double temp)
    {    
        if (currentTempUnit == "°F")
        {
            return Math.Round(temp * 1.8 + 32, 2);
        }
        return Math.Round(temp, 2);
    }




    /// <summary>
    /// Method to calculate wind based on the current unit.
    /// </summary>
    /// <returns>Method returns calculated wind acording to current unit.</returns>
    public double CalculatWind(double speed)
    {
        if (currentWindUnit == "m/s")
        { 
            return speed; 
        }
        if (currentWindUnit == "km/h")
        {
            return Math.Round(speed *3.6, 2);
        }
        return Math.Round(speed * 0.514, 2);

    }


    public void SetCelsius(bool celsius)
    {
        if (celsius)
        {
            ChangeTempUnit("C");
        }
        else 
        {
            ChangeTempUnit("F");
        }
        WeakReferenceMessenger.Default.Send(new UnitChangedMessage(true));
    }


    /// <summary>
    /// Method to change the temperature unit.
    /// </summary>
    /// <param name="newUnit">New unit to be set</param>
    public void ChangeTempUnit(string newUnit)
    {
        if (newUnit == "F")
        {
            currentTempUnit = "°F";
        }
        else currentTempUnit = "°C";
    }

    /// <summary>
    /// Method to change the wind unit.
    /// </summary>
    /// <param name="newUnit">New unit to be set</param>
    public void ChangeWindUnit(string newUnit)
    {
        if (newUnit == "km")
        {
            currentWindUnit = "km/h";
        }
        else if (newUnit == "m")
        {
            currentWindUnit = "m/s";
        }
        else
        {
            currentWindUnit = "kt";
        }
    }


}

